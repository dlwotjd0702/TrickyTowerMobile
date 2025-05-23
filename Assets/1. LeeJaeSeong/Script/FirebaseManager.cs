using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class FirebaseAccountManager : MonoBehaviour
{
    public static FirebaseAccountManager Instance { get; private set; }

    // per‐email FirebaseApp/Auth/Firestore
    private Dictionary<string, FirebaseApp>       _apps       = new Dictionary<string, FirebaseApp>();
    private Dictionary<string, FirebaseAuth>      _auths      = new Dictionary<string, FirebaseAuth>();
    private Dictionary<string, FirebaseFirestore> _firestores= new Dictionary<string, FirebaseFirestore>();

    [Header("Session Settings")] 
    public int               ttlMinutes   = 1;
    public NetworkManager    networkManager;
    public string            sessionName  = "";

    [Header("Session List UI")]
    public RectTransform     sessionListContent;
    public Button            loadSessionsButton;
    public Button            sessionButtonPrefab;

    [Header("Login UI")]
    public TextMeshProUGUI   inputemail;
    public TextMeshProUGUI   inputpassword;
    public Button            LoginButton;

    [Header("Signup UI")]
    public TextMeshProUGUI   makeemail;
    public TextMeshProUGUI   makepassword;
    public TextMeshProUGUI   makenick;
    public Button            SigninButton;

    [Header("Create Session Button")]
    public TextMeshProUGUI   SessionName;
    public Button            sessionButton;

    private bool isInitialized = false;
    private string _currentUserKey = null; // email used for auth

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (isInitialized) return;

        // UI callbacks
        LoginButton.onClick.AddListener(OnLoginClicked);
        SigninButton.onClick.AddListener(OnCreateAccountClicked);
        sessionButton.onClick.AddListener(OnCreateSessionDocument);
        loadSessionsButton.onClick.AddListener(FetchValidSessions);

        // Initialize default FirebaseApp
        FirebaseApp.CheckAndFixDependenciesAsync()
            .ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                isInitialized = true;
                Debug.Log("✅ Firebase DefaultInstance initialized");
            }
            else
            {
                Debug.LogError($"❌ Firebase init failed: {task.Result}");
            }
        });
    }

    private void OnLoginClicked()
    {
        var email    = inputemail.text;
        var password = inputpassword.text;
        SignIn(email, password);
    }

    private void OnCreateAccountClicked()
    {
        var email    = makeemail.text;
        var password = makepassword.text;
        var nickname = makenick.text;
        CreateAccount(email, password, nickname);
    }

    private void OnCreateSessionDocument()
    {
        sessionName = SessionName.text;
        if (string.IsNullOrEmpty(_currentUserKey))
        {
            Debug.LogError("세션을 생성하려면 먼저 로그인해야 합니다.");
            return;
        }
        // use logged-in user's auth
        var (auth, fs) = GetAuthAndFirestoreFor(_currentUserKey);
        CreateSessionDocument(sessionName, auth, fs);
        networkManager.sessionName = sessionName;
        networkManager.StartHost();
    }

    // Create or fetch a FirebaseApp/Auth/Firestore triple for this key (email)
    private (FirebaseAuth auth, FirebaseFirestore fs) GetAuthAndFirestoreFor(string key)
    {
        if (!_apps.ContainsKey(key))
        {
            var opts = FirebaseApp.DefaultInstance.Options;
            var app  = FirebaseApp.Create(new AppOptions {
                ApiKey    = opts.ApiKey,
                AppId     = opts.AppId,
                ProjectId = opts.ProjectId,
            }, key);
            _apps[key]        = app;
            _auths[key]       = FirebaseAuth.GetAuth(app);
            _firestores[key]  = FirebaseFirestore.GetInstance(app);
        }
        return (_auths[key], _firestores[key]);
    }

    private void CreateAccount(string email, string password, string nickname)
    {
        Debug.Log("회원가입 중...");
        var (auth, fs) = GetAuthAndFirestoreFor(email);
        auth.CreateUserWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("회원가입 실패: " + task.Exception?.Message);
                return;
            }

            var newUser = task.Result.User;
            Player.Instance.SetUserData(newUser);
            Debug.Log($"회원가입 성공: {newUser.Email}");
            _currentUserKey = email;

            // 프로필 설정
            newUser.UpdateUserProfileAsync(new UserProfile { DisplayName = nickname });
            
            // Firestore에 사용자 문서 생성
            var data = new Dictionary<string, object>
            {
                { "email",     email },
                { "nickname",  nickname },
                { "createdAt", Timestamp.GetCurrentTimestamp() },
                { "role",      "user" }
            };
            fs.Collection("users").Document(newUser.UserId)
              .SetAsync(data).ContinueWithOnMainThread(t =>
            {
                if (t.IsFaulted) Debug.LogError("사용자 문서 생성 실패: " + t.Exception?.Message);
                else             Debug.Log("Firestore 사용자 문서 생성 완료");
            });
        });
    }

    private void SignIn(string email, string password)
    {
        Debug.Log("로그인 시도 중...");
        var (auth, fs) = GetAuthAndFirestoreFor(email);
        auth.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("로그인 실패: " + task.Exception?.Message);
                return;
            }

            var user = task.Result.User;
            Player.Instance.SetUserData(user);
            Debug.Log($"로그인 성공: {user.Email}");
            _currentUserKey = email;
        });
    }

    private void CreateSessionDocument(string sessionName, FirebaseAuth auth, FirebaseFirestore fs)
    {
        var now      = Timestamp.GetCurrentTimestamp();
        var expireTs = Timestamp.FromDateTime(now.ToDateTime().AddMinutes(ttlMinutes));

        var data = new Dictionary<string, object>
        {
            { "host",         auth.CurrentUser.UserId },
            { "createdAt",    now },
            { "expiresAt",    expireTs },
            { "participants", new List<string> { auth.CurrentUser.UserId } }
        };

        fs.Collection("sessions").Document(sessionName)
          .SetAsync(data).ContinueWithOnMainThread(t =>
        {
            if (t.IsFaulted) Debug.LogError("세션 생성 실패: " + t.Exception?.Message);
            else             Debug.Log($"세션 생성 완료: {sessionName}");
        });
    }

    public void DeleteSessionDocument(string sessionName)
    {
        if (string.IsNullOrEmpty(_currentUserKey)) return;
        var (auth, fs) = GetAuthAndFirestoreFor(_currentUserKey);
        fs.Collection("sessions").Document(sessionName)
          .DeleteAsync().ContinueWithOnMainThread(t =>
        {
            if (t.IsFaulted) Debug.LogError("세션 삭제 실패: " + t.Exception?.Message);
            else             Debug.Log($"세션 삭제 완료: {sessionName}");
        });
    }

    public void UpdateSessionExpiration(string sessionName)
    {
        if (string.IsNullOrEmpty(_currentUserKey)) return;
        var (auth, fs) = GetAuthAndFirestoreFor(_currentUserKey);
        var now      = Timestamp.GetCurrentTimestamp();
        var expireTs = Timestamp.FromDateTime(now.ToDateTime().AddMinutes(ttlMinutes));

        fs.Collection("sessions").Document(sessionName)
          .UpdateAsync("expiresAt", expireTs)
          .ContinueWithOnMainThread(t =>
        {
            if (t.IsFaulted) Debug.LogWarning("세션 연장 실패: " + t.Exception?.Message);
            else             Debug.Log($"세션 연장 완료: {sessionName}");
        });
    }

    public void FetchValidSessions()
    {
        var fs  = FirebaseFirestore.DefaultInstance;
        var now = Timestamp.GetCurrentTimestamp();

        fs.Collection("sessions")
          .WhereGreaterThanOrEqualTo("expiresAt", now)
          .GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("세션 목록 불러오기 실패: " + task.Exception);
                return;
            }


            foreach (var doc in task.Result.Documents)
            {
                var id  = doc.Id;
                var btn = Instantiate(sessionButtonPrefab, sessionListContent);
                btn.GetComponentInChildren<TextMeshProUGUI>().text = id;
                btn.onClick.AddListener(() =>
                {
                    networkManager.sessionName = id;
                    networkManager.StartClient();
                });
            }
        });
    }
}
