using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Fusion;
using UnityEngine;

public class User
{
    //세션 종료시 해당 정보들이 계속 교체된다
    //Ex)
    //UserData = 로그아웃 할때
    //FusionPlayerRef => Fusion Session(Room In & Out)이 교체될때.

    public FirebaseUser UserData { get; set; }
    public PlayerRef FusionPlayerRef { get; set; }
}

public class Player : MonoBehaviour
{
    
    public static Player Instance;

    public string nickname;
    

    public User user { get; private set; }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        user = new User();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetUserData(FirebaseUser user)
    {
        this.user.UserData = user;
        Debug.Log($"MainSystem ::: {this.user.UserData.DisplayName}");
    }

    public void SetFusionPlayerRef(PlayerRef playerRef)
    {
        user.FusionPlayerRef = playerRef;
    }
    
}
    
    
