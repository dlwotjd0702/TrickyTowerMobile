using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using TMPro;
using UnityEngine.UI;

public class FirebaseAccountManager : MonoBehaviour
{
    // Named FirebaseApp 관리
    private Dictionary<string, FirebaseApp>    apps       = new Dictionary<string, FirebaseApp>();
    private Dictionary<string, FirebaseAuth>   auths      = new Dictionary<string, FirebaseAuth>();
    private Dictionary<string, FirebaseFirestore> stores   = new Dictionary<string, FirebaseFirestore>();
    private AppOptions defaultOptions;

    public int ttlMinutes = 1;
    public NetworkManager networkManager;

    [Header("로그인 UI")]
    public TextMeshProUGUI inputLoginEmail;
    public TextMeshProUGUI inputLoginPassword;
    public Button           LoginButton;

    [Header("회원가입 UI")]
    public TextMeshProUGUI inputSignUpEmail;
    public TextMeshProUGUI inputSignUpPassword;
    public TextMeshProUGUI inputSignUpNickname;
    public Button           SigninButton;

    [Header("세션 리스트 UI")]
    public RectTransform  sessionListContent;
    public Button         loadSessionsButton;
    public Button         sessionButtonPrefab;

    [Header("세션 생성 UI")]
    public TextMeshProUGUI SessionNameField;
    public Button         sessionButton;

    private bool isInitialized = false;

    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                defaultOptions = FirebaseApp.DefaultInstance.Options;
                isInitialized = true;
                Debug.Log("✅ Firebase 기본 앱 초기화 완료");
                LoginButton.interactable = true;
                SigninButton.interactable = true;
                loadSessionsButton.interactable = true;
                sessionButton.interactable = true;
            }
            else
            {
                Debug.LogError($"❌ Firebase 초기화 실패: {task.Result}");
            }
        });
    }

    // key로 Named App/Auth/Firestore 인스턴스 반환
    private (FirebaseAuth auth, FirebaseFirestore store) GetServices(string key)
    {
        if (!apps.TryGetValue(key, out var app))
        {
            var opts = new AppOptions
            {
                ApiKey         = defaultOptions.ApiKey,
                AppId          = defaultOptions.AppId,
                ProjectId      = defaultOptions.ProjectId,
                DatabaseUrl    = defaultOptions.DatabaseUrl,
                StorageBucket  = defaultOptions.StorageBucket,
                MessageSenderId= defaultOptions.MessageSenderId,
            };
            app = FirebaseApp.Create(opts, key);
            apps[key] = app;
            auths[key]  = FirebaseAuth.GetAuth(app);
            stores[key] = FirebaseFirestore.GetInstance(app);
            Debug.Log($"🔹 FirebaseApp 생성: {key}");
        }
        return (auths[key], stores[key]);
    }

    private void Start()
    {
        LoginButton.onClick.AddListener(OnLoginClicked);
        SigninButton.onClick.AddListener(OnCreateAccountClicked);
        sessionButton.onClick.AddListener(OnCreateSessionDocument);
        loadSessionsButton.onClick.AddListener(FetchValidSessions);
    }

    private void OnLoginClicked()
    {
        if (!isInitialized) return;
        string email = inputLoginEmail.text.Trim();
        string password = inputLoginPassword.text;
        Debug.Log($"로그인 시도: {email}");
        SignIn(email, password);
    }

    private void OnCreateAccountClicked()
    {
        if (!isInitialized) return;
        string email = inputSignUpEmail.text.Trim();
        string password = inputSignUpPassword.text;
        string nickname = inputSignUpNickname.text.Trim();
        Debug.Log($"회원가입 시도: {email}, 닉네임: {nickname}");
        CreateAccount(email, password, nickname);
    }

    private void OnCreateSessionDocument()
    {
        if (!isInitialized) return;
        string sessionName = SessionNameField.text.Trim();
        Debug.Log($"세션 생성: {sessionName}");
        CreateSessionDocument(sessionName);
        networkManager.sessionName = sessionName;
        networkManager.StartHost();
    }

    private void CreateAccount(string email, string password, string nickname)
    {
        var (playerAuth, _) = GetServices(email);
        playerAuth.CreateUserWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError($"회원가입 실패: {task.Exception?.Message}");
                return;
            }
            var user = task.Result.User;
            Debug.Log($"✅ 회원가입 성공: {user.Email}");
            UpdateUserNickname(user, nickname);
            CreateUserDocument(email, user.UserId, nickname);
        });
    }

    private void SignIn(string email, string password)
    {
        var (playerAuth, _) = GetServices(email);
        playerAuth.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError($"로그인 실패: {task.Exception?.Message}");
                return;
            }
            var user = task.Result.User;
            Debug.Log($"✅ 로그인 성공: {user.Email}");
            // 로그인된 사용자 키로 서비스 재사용 가능
        });
    }

    private void UpdateUserNickname(FirebaseUser user, string nickname)
    {
        var profile = new UserProfile { DisplayName = nickname };
        user.UpdateUserProfileAsync(profile)
            .ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
                Debug.Log($"닉네임 설정 완료: {nickname}");
            else
                Debug.LogError($"닉네임 설정 실패: {task.Exception?.Message}");
        });
    }

    private void CreateUserDocument(string email, string uid, string nickname)
    {
        var (_, playerStore) = GetServices(email);
        var data = new Dictionary<string, object>
        {
            { "email", email },
            { "nickname", nickname },
            { "createdAt", Timestamp.GetCurrentTimestamp() },
            { "role", "user" }
        };
        playerStore.Collection("users").Document(uid)
            .SetAsync(data)
            .ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
                Debug.Log("Firestore 사용자 문서 생성 완료");
            else
                Debug.LogError($"Firestore 문서 생성 실패: {task.Exception?.Message}");
        });
    }

    // 세션 관련도 Named App으로 처리
    public void CreateSessionDocument(string sessionName)
    {
        var currentUser = FirebaseAuth.DefaultInstance.CurrentUser;
        if (currentUser == null)
        {
            Debug.LogError("세션 생성 실패: 로그인된 사용자가 없습니다.");
            return;
        }
        string key = currentUser.UserId;
        var (_, playerStore) = GetServices(key);

        var now = Timestamp.GetCurrentTimestamp();
        var expireTs = Timestamp.FromDateTime(now.ToDateTime().AddMinutes(ttlMinutes));
        var data = new Dictionary<string, object>
        {
            { "host", key },
            { "createdAt", now },
            { "expiresAt", expireTs },
            { "participants", new List<string> { key } }
        };
        playerStore.Collection("sessions").Document(sessionName)
            .SetAsync(data)
            .ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
                Debug.LogError($"세션 생성 실패: {task.Exception?.Message}");
        });
    }

    public void DeleteSessionDocument(string sessionName)
    {
        if (string.IsNullOrEmpty(sessionName))
        {
            Debug.LogWarning("삭제할 세션 이름이 비어 있습니다.");
            return;
        }
        var currentUser = FirebaseAuth.DefaultInstance.CurrentUser;
        if (currentUser == null) return;
        string key = currentUser.UserId;
        var (_, playerStore) = GetServices(key);

        playerStore.Collection("sessions").Document(sessionName)
            .DeleteAsync()
            .ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
                Debug.LogError($"세션 삭제 실패: {task.Exception?.Message}");
            else
                Debug.Log($"세션 삭제 완료: {sessionName}");
        });
    }

    public void UpdateSessionExpiration(string sessionName)
    {
        var currentUser = FirebaseAuth.DefaultInstance.CurrentUser;
        if (currentUser == null) return;
        string key = currentUser.UserId;
        var (_, playerStore) = GetServices(key);

        var now = Timestamp.GetCurrentTimestamp();
        var expireTs = Timestamp.FromDateTime(now.ToDateTime().AddMinutes(ttlMinutes));
        playerStore.Collection("sessions").Document(sessionName)
            .UpdateAsync("expiresAt", expireTs)
            .ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
                Debug.LogWarning($"세션 연장 실패: {task.Exception?.Message}");
        });
    }

    public void FetchValidSessions()
    {
        if (!isInitialized) return;
        var currentUser = FirebaseAuth.DefaultInstance.CurrentUser;
        if (currentUser == null) return;
        string key = currentUser.UserId;
        var (_, playerStore) = GetServices(key);

        var now = Timestamp.GetCurrentTimestamp();
        playerStore.Collection("sessions")
            .WhereGreaterThanOrEqualTo("expiresAt", now)
            .GetSnapshotAsync()
            .ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"세션 목록 불러오기 실패: {task.Exception}");
                return;
            }
            foreach (Transform child in sessionListContent)
                Destroy(child.gameObject);

            foreach (var doc in task.Result.Documents)
            {
                string sessionId = doc.Id;
                Button btn = Instantiate(sessionButtonPrefab, sessionListContent);
                btn.GetComponentInChildren<TextMeshProUGUI>().text = sessionId;
                btn.onClick.AddListener(() =>
                {
                    networkManager.sessionName = sessionId;
                    networkManager.StartClient();
                });
            }
        });
    }
}
