using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class FirebaseAccountManager : MonoBehaviour
{
    public static FirebaseAccountManager Instance { get; private set; }

    // per‐email FirebaseApp/Auth/Firestore
    private Dictionary<string, FirebaseApp>       _apps       = new();
    private Dictionary<string, FirebaseAuth>      _auths      = new();
    private Dictionary<string, FirebaseFirestore> _firestores= new();

    [Header("Session Settings")]
    public int            ttlMinutes     = 1;
    public NetworkManager networkManager;
    public string         sessionName    = "";

    [Header("Session List UI")]
    public RectTransform  sessionListContent;
    public Button         loadSessionsButton;
    public Button         sessionButtonPrefab;

    [Header("Login UI")]
    public TMP_InputField inputemail;
    public TMP_InputField inputpassword;
    public Button         LoginButton;

    [Header("Signup UI")]
    public TMP_InputField makeemail;
    public TMP_InputField makepassword;
    public TMP_InputField makenick;
    public Button         SigninButton;

    [Header("Create Session Button")]
    public TMP_InputField SessionName;
    public Button         sessionButton;

    private bool   isInitialized     = false;
    private string _currentUserKey   = null; // email used for auth
    public RoomSettingsUI roomSettingsUI;

    private void Awake()
    {
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

        LoginButton.onClick.AddListener(OnLoginClicked);
        SigninButton.onClick.AddListener(OnCreateAccountClicked);
        sessionButton.onClick.AddListener(OnCreateSessionDocument);
        loadSessionsButton.onClick.AddListener(FetchValidSessions);

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
        SignIn(inputemail.text, inputpassword.text);
    }

    private void OnCreateAccountClicked()
    {
        CreateAccount(makeemail.text, makepassword.text, makenick.text);
    }

    private void OnCreateSessionDocument()
    {
        sessionName = SessionName.text;
        if (string.IsNullOrEmpty(_currentUserKey))
        {
            Debug.LogError("세션을 생성하려면 먼저 로그인해야 합니다.");
            return;
        }

        // 네트워크매니저에 이미 설정된 gameType 읽어서 세션문서에 포함
        networkManager.sessionName = sessionName;
        var (auth, fs) = GetAuthAndFirestoreFor(_currentUserKey);
        CreateSessionDocument(sessionName, auth, fs, roomSettingsUI.currentIndex);
        Debug.Log(roomSettingsUI.currentIndex);
        networkManager.StartHost();
    }

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
                var uiMgr = FindObjectOfType<UIButtonManager>();
                uiMgr?.reenterUI?.SetActive(true);
                uiMgr?.signupUI?.SetActive(true);
                return;
            }

            var newUser = task.Result.User;
            FindObjectOfType<Player>()?.SetUserData(newUser);
            Debug.Log($"회원가입 성공: {newUser.Email}");
            _currentUserKey = email;

            newUser.UpdateUserProfileAsync(new UserProfile { DisplayName = nickname });

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

            FindObjectOfType<UIButtonManager>()?.successSignupUI?.SetActive(true);
        });
    }

    private void SignIn(string email, string password)
    {
        Debug.Log("로그인 시도 중...");
        var (auth, _) = GetAuthAndFirestoreFor(email);
        auth.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("로그인 실패: " + task.Exception?.Message);
                FindObjectOfType<UIButtonManager>()?.reenterUI?.SetActive(true);
                return;
            }

            var user = task.Result.User;
            var player = FindObjectOfType<Player>();
            player?.SetUserData(user);
            player.nickname = user.DisplayName;
            Debug.Log($"로그인 성공: {user.Email}");
            _currentUserKey = email;
            var uiMgr = FindObjectOfType<UIButtonManager>();
            uiMgr?.introUI?.SetActive(false);
            uiMgr?.mainUI?.SetActive(true);
        });
    }

    private void CreateSessionDocument(string sessionName, FirebaseAuth auth, FirebaseFirestore fs, int gameType)
    {
        var now      = Timestamp.GetCurrentTimestamp();
        var expireTs = Timestamp.FromDateTime(now.ToDateTime().AddMinutes(ttlMinutes));

        // 여기에서 gameType 저장
        var data = new Dictionary<string, object>
        {
            { "host",         auth.CurrentUser.UserId },
            { "createdAt",    now },
            { "expiresAt",    expireTs },
            { "participants", new List<string> { auth.CurrentUser.UserId } },
            { "gameType",     gameType }
        };

        fs.Collection("sessions").Document(sessionName)
          .SetAsync(data).ContinueWithOnMainThread(t =>
        {
            if (t.IsFaulted) Debug.LogError("세션 생성 실패: " + t.Exception?.Message);
            else
            {
                Debug.Log($"세션 생성 완료: {sessionName} (type={gameType})");
                DOTween.KillAll();
            }
        });
    }

    public void FetchValidSessions()
{
    var fs  = FirebaseFirestore.DefaultInstance;
    var now = Timestamp.GetCurrentTimestamp();

    fs.Collection("sessions")
      .WhereGreaterThanOrEqualTo("expiresAt", now)
      .GetSnapshotAsync()
      .ContinueWithOnMainThread(task =>
    {
        if (task.IsFaulted)
        {
            Debug.LogError("세션 목록 불러오기 실패: " + task.Exception);
            return;
        }

        // 버튼 위치용 카운터와 간격 계산
        int i = -1;
        var prefabRt  = sessionButtonPrefab.GetComponent<RectTransform>();
        float spacing = prefabRt.rect.height + 10f; // 버튼 높이 + 10px 여백

        foreach (var doc in task.Result.Documents)
        {
            string id = doc.Id;

            // 참가자 수
            int count = 0;
            if (doc.TryGetValue("participants", out List<string> plist))
                count = plist.Count;

            // 저장된 gameType 읽기
            int    storedTypeInt = doc.TryGetValue("gameType", out long gi) ? (int)gi : 0;
            GameType storedType   = (GameType)storedTypeInt;

            // 버튼 생성
            var btn = Instantiate(sessionButtonPrefab, sessionListContent);
            btn.transform.localScale = Vector3.one;

            // Y 위치만 조정 (위에서 i * spacing 만큼 내려감)
            var rt = btn.GetComponent<RectTransform>();
            var pos = rt.anchoredPosition;
            pos.y = -i * spacing;
            rt.anchoredPosition = pos;

            // 텍스트 설정
            var label = btn.GetComponentInChildren<TextMeshProUGUI>();
            label.text = $"{id} ({count}/4)\nMode: {storedType}";

            // 클릭 리스너
            string sessionId = id;
            GameType gameType = storedType;
            btn.onClick.AddListener(() =>
            {
                var (auth, firestore) = GetAuthAndFirestoreFor(_currentUserKey);
                firestore.Collection("sessions")
                         .Document(sessionId)
                         .UpdateAsync("participants", FieldValue.ArrayUnion(auth.CurrentUser.UserId))
                         .ContinueWithOnMainThread(t2 =>
                {
                    if (t2.IsFaulted)
                        Debug.LogError("참가자 추가 실패: " + t2.Exception);
                    else
                        Debug.Log($"[{sessionId}] 참가자 추가 완료");
                });

                networkManager.sessionName = sessionId;
                networkManager.gameType    = gameType;
                networkManager.StartClient();
            });

            i++; // 다음 버튼을 위해 인덱스 증가
        }
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
                else            
                {
                    Debug.Log($"세션 삭제 완료: {sessionName}");

                }
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
}
