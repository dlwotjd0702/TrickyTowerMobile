using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;

public class FirebaseAccountManager : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseFirestore firestore;

    private string email = "";
    private string password = "";
    private string nickname = "";
    // Firebase session management
    private string sessionName = "";

    private string statusMessage = "";

    private bool isInitialized = false;
    private bool isLoggedIn = false;
    private bool isSignUpMode = false;

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                firestore = FirebaseFirestore.DefaultInstance;
                isInitialized = true;
                statusMessage = "Firebase 초기화 완료";
            }
            else
            {
                statusMessage = $"Firebase 초기화 실패: {task.Result}";
            }
        });
    }

    private void CreateAccount(string email, string password, string nickname)
    {
        statusMessage = "회원가입 진행 중...";
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                statusMessage = "회원가입 실패: " + task.Exception?.Message;
                return;
            }

            var result = task.Result;
            FirebaseUser newUser = result.User;
            MainSystem.Instance.SetUserData(newUser);
            statusMessage = $"회원가입 성공: {newUser.Email}";

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
                statusMessage += $"\n닉네임 설정 완료: {nickname}";
            else
                statusMessage += "\n닉네임 설정 실패: " + task.Exception?.Message;
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
                statusMessage += "\nFirestore 사용자 문서 생성 완료";
            else
                statusMessage += "\nFirestore 문서 생성 실패: " + task.Exception?.Message;
        });
    }

    private void SignIn(string email, string password)
    {
        statusMessage = "로그인 시도 중...";
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                statusMessage = "로그인 실패: " + task.Exception?.Message;
                return;
            }

            var result = task.Result;
            FirebaseUser user = result.User;
            MainSystem.Instance.SetUserData(user);
            isLoggedIn = true;
            statusMessage = $"로그인 성공: {user.Email}";
        });
    }

    private void SignOut()
    {
        auth.SignOut();
        isLoggedIn = false;
        statusMessage = "로그아웃 되었습니다.";
        MainSystem.Instance.SetUserData(null);
    }

    // Firebase-only: Create a game session document
    private void CreateSessionDocument(string sessionName)
    {
        if (string.IsNullOrEmpty(sessionName))
        {
            statusMessage = "세션 이름을 입력하세요.";
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
                statusMessage = $"세션 생성 완료: {sessionName}";
            else
                statusMessage = "세션 생성 실패: " + task.Exception?.Message;
        });
    }

    /* Fusion2 호환용: 추후 활성화할 CreateSession, FetchSessions, JoinSession 기능
    private void CreateSession(string sessionName) { }
    private void FetchSessions() { }
    private void JoinSession(string sessionName) { }
    */

    #region 출력

    private void OnGUI()
    {
        float centerX = Screen.width / 2;
        float centerY = Screen.height / 2;
        GUI.Box(new Rect(centerX - 200, centerY - 150, 400, 300), "");

        if (!isInitialized)
        {
            GUI.Label(new Rect(10, 10, 500, 30), "Firebase 초기화 중...");
            return;
        }

        GUI.Label(new Rect(10, 10, 500, 25), statusMessage);

        if (isLoggedIn)
        {
            DrawLoggedInUI(centerX, centerY);
            DrawSessionUI(centerX, centerY + 100);
        }
        else
        {
            if (isSignUpMode) DrawSignUpUI(centerX, centerY);
            else DrawLoginUI(centerX, centerY);
        }
    }

    private void DrawLoginUI(float centerX, float centerY)
    {
        GUI.Label(new Rect(centerX - 160, centerY - 40, 100, 25), "Email:");
        email = GUI.TextField(new Rect(centerX - 50, centerY - 40, 200, 25), email);
        GUI.Label(new Rect(centerX - 160, centerY, 100, 25), "Password:");
        password = GUI.PasswordField(new Rect(centerX - 50, centerY, 200, 25), password, '*');
        if (GUI.Button(new Rect(centerX - 150, centerY + 50, 150, 30), "로그인")) SignIn(email, password);
        if (GUI.Button(new Rect(centerX + 10, centerY + 50, 150, 30), "회원가입")) { isSignUpMode = true; statusMessage = "회원가입 화면으로 전환됨"; }
    }

    private void DrawSignUpUI(float centerX, float centerY)
    {
        GUI.Label(new Rect(centerX - 160, centerY - 60, 100, 25), "Email:");
        email = GUI.TextField(new Rect(centerX - 50, centerY - 60, 200, 25), email);
        GUI.Label(new Rect(centerX - 160, centerY - 20, 100, 25), "Password:");
        password = GUI.PasswordField(new Rect(centerX - 50, centerY - 20, 200, 25), password, '*');
        GUI.Label(new Rect(centerX - 160, centerY + 20, 100, 25), "Nickname:");
        nickname = GUI.TextField(new Rect(centerX - 50, centerY + 20, 200, 25), nickname);
        if (GUI.Button(new Rect(centerX - 150, centerY + 70, 150, 30), "회원가입"))
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(nickname))
            {
                statusMessage = "모든 정보를 입력해주세요.";
                return;
            }
            CreateAccount(email, password, nickname);
        }
        if (GUI.Button(new Rect(centerX + 10, centerY + 70, 150, 30), "뒤로")) { isSignUpMode = false; statusMessage = "로그인 화면으로 전환됨"; }
    }

    private void DrawLoggedInUI(float centerX, float centerY)
    {
        string nick = auth.CurrentUser?.DisplayName ?? "(닉네임 없음)";
        GUI.Label(new Rect(centerX - 200, centerY - 40, 400, 30), $"어서오세요, {nick} 님!");
        if (GUI.Button(new Rect(centerX - 75, centerY, 150, 30), "로그아웃")) SignOut();
    }

    private void DrawSessionUI(float centerX, float centerY)
    {
        GUI.Label(new Rect(centerX - 160, centerY - 20, 100, 25), "Session Name:");
        sessionName = GUI.TextField(new Rect(centerX - 50, centerY - 20, 200, 25), sessionName);
        if (GUI.Button(new Rect(centerX - 75, centerY + 30, 150, 30), "Create Session"))
        {
            CreateSessionDocument(sessionName);
        }
    }

    #endregion
}
