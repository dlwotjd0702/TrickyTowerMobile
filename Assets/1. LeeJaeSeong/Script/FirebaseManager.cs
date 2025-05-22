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

    private string email = "";
    private string password = "";
    private string nickname = "";
    // Firebase session management
    private string sessionName = "";

  
    
    public TextMeshProUGUI inputemail;
    public TextMeshProUGUI inputpassword;
    public TextMeshProUGUI inputnick;
    public TextMeshProUGUI SessionName;
    public Button LoginButton;
    public Button SigninButton;


    private bool isInitialized = false;
    private bool isLoggedIn = false;
    private bool isSignUpMode = false;

    private void Start()
    {
        LoginButton.onClick.AddListener(OnLoginClicked);
        SigninButton.onClick.AddListener(OnCreateAccountClicked);
        
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
        email = inputemail.text;
        password = inputpassword.text;
        nickname = inputnick.text;
        Debug.Log("login");
        CreateAccount(email, password, nickname);
    }

    private void SignOut()
    {
        auth.SignOut();
        isLoggedIn = false;
        Debug.Log( "로그아웃 되었습니다.");
        Player.Instance.SetUserData(null);
    }

    // Firebase-only: Create a game session document
    private void CreateSessionDocument(string sessionName)
    {
        if (string.IsNullOrEmpty(sessionName))
        {
            Debug.Log( "세션 이름을 입력하세요.");
            return;
        }
        var sessionDoc = firestore.Collection("sessions").Document(sessionName);
        var data = new Dictionary<string, object>
        {
            { "host", auth.CurrentUser.UserId },
            { "createdAt", Timestamp.GetCurrentTimestamp() },
            { "participants", new List<string> { auth.CurrentUser.UserId } }
        };
        sessionDoc.SetAsync(data).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
                Debug.Log( $"세션 생성 완료: {sessionName}");
            else
                Debug.Log( "세션 생성 실패: " + task.Exception?.Message);
        });
    }

  
  

   

}
