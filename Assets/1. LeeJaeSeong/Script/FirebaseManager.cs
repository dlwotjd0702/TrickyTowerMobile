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
    private FirebaseAuth auth;
    private FirebaseFirestore firestore;
    public int ttlMinutes = 1; 
    public NetworkManager networkManager;

    private string email = "";
    private string password = "";
    private string nickname = "";
    // Firebase session management
    public string sessionName = "";
    
    [Header("세션 리스트 UI")]
    public RectTransform sessionListContent; 
    public Button loadSessionsButton;// Scroll View → Content
    public Button      sessionButtonPrefab;

  
    
    public TextMeshProUGUI inputemail;
    public TextMeshProUGUI inputpassword;
    
    public TextMeshProUGUI SessionName;
    public Button LoginButton;
    
    public TextMeshProUGUI makenick;
    public TextMeshProUGUI makeemail;
    public TextMeshProUGUI makepassword;
    public Button SigninButton;
    public Button sessionButton;


    private bool isInitialized = false;
    private bool isLoggedIn = false;
    private bool isSignUpMode = false;
    
   
    private void Start()
    {
        LoginButton.onClick.AddListener(OnLoginClicked);
        SigninButton.onClick.AddListener(OnCreateAccountClicked);
        sessionButton.onClick.AddListener(OnCreateSessionDocument);
        
        loadSessionsButton.onClick.AddListener(FetchValidSessions);
        
        
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                firestore = FirebaseFirestore.DefaultInstance;
                isInitialized = true;
                Debug.Log( "Firebase 초기화 완료");
            }
            else
            {
                Debug.Log( $"Firebase 초기화 실패: {task.Result}");
            }
        });
    }

    private void CreateAccount(string email, string password, string nickname)
    {
        Debug.Log( "회원가입 진행 중...");
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.Log( "회원가입 실패: " + task.Exception?.Message);
                return;
            }

            var result = task.Result;
            FirebaseUser newUser = result.User;
            Player.Instance.SetUserData(newUser);
            Debug.Log( $"회원가입 성공: {newUser.Email}");

            UpdateUserNickname(newUser, nickname);
            CreateUserDocument(newUser.UserId, email, nickname);
            isSignUpMode = false;
            isLoggedIn = true;
        });
    }

    private void UpdateUserNickname(FirebaseUser user, string nickname)
    {
        UserProfile profile = new UserProfile { DisplayName = nickname };
        user.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
                Debug.Log( $"\n닉네임 설정 완료: {nickname}");
            else
                Debug.Log( "\n닉네임 설정 실패: " + task.Exception?.Message);
        });
    }

    private void CreateUserDocument(string uid, string email, string nickname)
    {
        var userDoc = firestore.Collection("users").Document(uid);
        var userData = new
        {
            email = email,
            nickname = nickname,
            createdAt = Timestamp.GetCurrentTimestamp(),
            role = "user"
        };
        userDoc.SetAsync(userData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
                Debug.Log( "\nFirestore 사용자 문서 생성 완료");
            else
                Debug.Log( "\nFirestore 문서 생성 실패: " + task.Exception?.Message);
        });
    }

    private void SignIn(string email, string password)
    {
        Debug.Log( "로그인 시도 중...");
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.Log( "로그인 실패: " + task.Exception?.Message);
                return;
            }

            var result = task.Result;
            FirebaseUser user = result.User;
            Player.Instance.SetUserData(user);
            isLoggedIn = true;
            Debug.Log( $"로그인 성공: {user.Email}");
        });
    }
    void OnLoginClicked()
    {
        email = inputemail.text;
        password = inputpassword.text;
        Debug.Log("login");
        SignIn(email, password);
    }
    void OnCreateAccountClicked()
    {
        email = makeemail.text;
        password = makepassword.text;
        nickname = makenick.text;
        Debug.Log("login");
        CreateAccount(email, password, nickname);
    }
    void OnCreateSessionDocument()
    {
        sessionName = SessionName.text;
        
        Debug.Log("session");
        CreateSessionDocument(sessionName);
        networkManager.sessionName = sessionName;
        networkManager.StartHost();
        
    }

    private void SignOut()
    {
        auth.SignOut();
        isLoggedIn = false;
        Debug.Log( "로그아웃 되었습니다.");
        Player.Instance.SetUserData(null);
    }
    
    

    // Firebase-only: Create a game session document
    public void CreateSessionDocument(string sessionName)
    {
        var now = Timestamp.GetCurrentTimestamp();
        var expireTs = Timestamp.FromDateTime(now.ToDateTime().AddMinutes(ttlMinutes));

        var data = new Dictionary<string, object>()
        {
            { "host", FirebaseAuth.DefaultInstance.CurrentUser.UserId },
            { "createdAt", now },
            { "expiresAt", expireTs },
            { "participants", new List<string> { FirebaseAuth.DefaultInstance.CurrentUser.UserId } }
        };
        firestore.Collection("sessions")
                 .Document(sessionName)
                 .SetAsync(data)
                 .ContinueWithOnMainThread(t =>
        {
            if (t.IsFaulted) Debug.LogError("세션 생성 실패: " + t.Exception?.Message);
        });
    }
    
    

    public void DeleteSessionDocument(string sessionName)
    {
        if (string.IsNullOrEmpty(sessionName))
        {
            Debug.LogWarning("삭제할 세션 이름이 비어 있습니다.");
            return;
        }

        var sessionDoc = firestore.Collection("sessions").Document(sessionName);
        sessionDoc.DeleteAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
                Debug.Log($"세션 삭제 완료: {sessionName}");
            else
                Debug.LogError($"세션 삭제 실패: {task.Exception?.Message}");
        });
    }
    
    public void UpdateSessionExpiration(string sessionName)
    {
        var now = Timestamp.GetCurrentTimestamp();
        var expireTs = Timestamp.FromDateTime(now.ToDateTime().AddMinutes(ttlMinutes));

        firestore.Collection("sessions")
            .Document(sessionName)
            .UpdateAsync("expiresAt", expireTs)
            .ContinueWithOnMainThread(t =>
            {
                if (t.IsFaulted) Debug.LogWarning("세션 연장 실패: " + t.Exception?.Message);
            });
    }
    
    public void FetchValidSessions()
    {
        Debug.Log("1");
        var now = Timestamp.GetCurrentTimestamp();
        Debug.Log("2");
        firestore.Collection("sessions")
            .WhereGreaterThanOrEqualTo("expiresAt", now)
            .GetSnapshotAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("세션 목록 불러오기 실패: " + task.Exception);
                    return;
                }
                Debug.Log("3");

                foreach (var doc in task.Result.Documents)
                {
                    // 2. 로컬 변수로 캡처
                    string sessionId = doc.Id;
                
                    // 3. Button 인스턴스 생성
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
   


    // 예: 로비 씬이 로드되면 한 번 호출
    
        
   

}
