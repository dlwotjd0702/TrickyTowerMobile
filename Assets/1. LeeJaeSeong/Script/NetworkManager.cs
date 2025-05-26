using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fusion;
using Fusion.Sockets;
using Fusion.Addons.Physics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("외부 연결")]
    public NetworkInputHandler inputHandler;
    public NetworkSpawnHandler spawnHandler;
    public GameRuleManager gameRuleManager;
    public CameraManager cameraManager;
    public FirebaseAccountManager firebaseAccountManager;
    public float heartbeatInterval = 10f;
    private Coroutine heartbeatRoutine;

    [Header("네트워크 설정")]
    [SerializeField] public string sessionName;
    public Vector3[] spawnOffsets = new Vector3[4];

    private NetworkRunner runner;
    private bool isGameStarted = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }



    private void OnGUI()
    {
        
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (GUI.Button(new Rect(10, 10, 200, 40), "Host"))
                StartHost();
            if (GUI.Button(new Rect(10, 60, 200, 40), "Join"))
                StartClient();
        }
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (GUI.Button(new Rect(10, 110, 200, 40), "Start Game"))
                HostStartGame();
        }
        
    }

    public async void StartHost()
    {
        SetupRunner();
        var result = await runner.StartGame(new StartGameArgs {
            GameMode     = GameMode.Host,
            SessionName  = sessionName,
            SceneManager = gameObject.GetComponent<NetworkSceneManagerDefault>(),
            Scene        = SceneRef.FromIndex(1),
            PlayerCount  = 4
        });
        if (!result.Ok) Debug.LogError($"Host start failed: {result.ShutdownReason}");
        var targetScene = SceneRef.FromIndex(1);
        await runner.LoadScene(targetScene);
        
        firebaseAccountManager = FindObjectOfType<FirebaseAccountManager>();
        if (heartbeatRoutine != null) StopCoroutine(heartbeatRoutine);
        heartbeatRoutine = StartCoroutine(HeartbeatCoroutine());
        
    }

    public async void StartClient()
    {
        SetupRunner();
        var result = await runner.StartGame(new StartGameArgs {
            GameMode     = GameMode.Client,
            SessionName  = sessionName,
            SceneManager = gameObject.GetComponent<NetworkSceneManagerDefault>(),
            Scene        = SceneRef.FromIndex(1)
        });
        if (!result.Ok) Debug.LogError($"Client join failed: {result.ShutdownReason}");
       
    }

    private void SetupRunner()
    {
        runner = gameObject.AddComponent<NetworkRunner>();
        DontDestroyOnLoad(runner.gameObject);
        runner.ProvideInput = true;
        runner.AddCallbacks(this);
        if (inputHandler != null)  runner.AddCallbacks(inputHandler);
        if (spawnHandler != null)  runner.AddCallbacks(spawnHandler);
        
        var sceneMgr = gameObject.GetComponent<NetworkSceneManagerDefault>()
                        ?? gameObject.AddComponent<NetworkSceneManagerDefault>();
        DontDestroyOnLoad(sceneMgr.gameObject);

        var phys2D = gameObject.GetComponent<RunnerSimulatePhysics2D>()
                     ?? gameObject.AddComponent<RunnerSimulatePhysics2D>();
        phys2D.ClientPhysicsSimulation = ClientPhysicsSimulation.SimulateAlways;

        DontDestroyOnLoad(phys2D.gameObject);
    }

    public async void HostStartGame()
    {
        Debug.Log("Start Game");
        if (runner == null || !runner.IsServer || isGameStarted) return;
        isGameStarted = true;
        
        var targetScene = SceneRef.FromIndex(2);
        await runner.LoadScene(targetScene);
    }
public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
    }

    public void RequestNextBlock(PlayerRef player)
    {
        if (!runner.IsServer) return; //여기서 서버외 인원들 다 튕김
        int idx = GetPlayerJoinIndex(player);
        Vector3 offset = spawnOffsets[Mathf.Clamp(idx, 0, spawnOffsets.Length - 1)];
        spawnHandler?.SpawnBlockFor(runner, player, offset);
    }

    public int GetPlayerJoinIndex(PlayerRef player)
    {
        var list = new List<PlayerRef>(runner.ActivePlayers);
        list.Sort((a, b) => a.RawEncoded.CompareTo(b.RawEncoded));
        return list.IndexOf(player);
    }

    public int GetMiniCamIndexForPlayer(PlayerRef player)
    {
        var others = new List<PlayerRef>();
        foreach (var p in runner.ActivePlayers)
            if (p != runner.LocalPlayer) others.Add(p);
        others.Sort((a, b) => a.RawEncoded.CompareTo(b.RawEncoded));
        int ix = others.IndexOf(player);
        return (ix >= 0 && ix < 3) ? ix : 0;
    }
    
    private IEnumerator HeartbeatCoroutine()
    {
        while (runner != null && runner.IsServer)
        {
            yield return new WaitForSeconds(heartbeatInterval);
            firebaseAccountManager?.UpdateSessionExpiration(sessionName);
        }
    }

    /*private void StartGame()
    {
             foreach (var player in runner.ActivePlayers) 
             {
            // 1) 씬에 배치된 매니저들 찾아서 참조
            cameraManager   = FindObjectOfType<CameraManager>();
            spawnHandler    = FindObjectOfType<NetworkSpawnHandler>();
            inputHandler    = FindObjectOfType<NetworkInputHandler>();

            if (cameraManager == null) Debug.LogError("CameraManager를 찾을 수 없습니다.");
            if (spawnHandler  == null) Debug.LogError("NetworkSpawnHandler를 찾을 수 없습니다.");
            if (inputHandler  == null) Debug.LogError("NetworkInputHandler를 찾을 수 없습니다.");

            // 2) Runner에 콜백 재등록
            runner.AddCallbacks(spawnHandler);
            runner.AddCallbacks(inputHandler);
            Debug.Log("시작룰");
            
           
                int idx = GetPlayerJoinIndex(player);
                Debug.Log(idx);
                Debug.Log(runner.IsServer);
                Vector3 offset = spawnOffsets[Mathf.Clamp(idx, 0, spawnOffsets.Length - 1)];

                if (player == runner.LocalPlayer)
                    cameraManager?.SetupMainCam(offset);
                else
                    cameraManager?.SetupMiniCam(offset, GetMiniCamIndexForPlayer(player));

                if (runner.IsServer)
                    spawnHandler?.SpawnBlockFor(runner, player, offset);
            }
        
    }*/

    // INetworkRunnerCallbacks stubs
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnConnectedToServer(NetworkRunner runner) { }

    public void OnShutdown(NetworkRunner runner, ShutdownReason reason)
    {
    }

    /*{
        if (heartbeatRoutine != null)
        {
            StopCoroutine(heartbeatRoutine);
            heartbeatRoutine = null;
        }
        if (runner.IsServer)
        {
           firebaseAccountManager = FindObjectOfType<FirebaseAccountManager>();
           firebaseAccountManager.DeleteSessionDocument(sessionName);

        }
    }*/
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        
    }

   
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken token) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSceneLoadDone(NetworkRunner runner) 
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            firebaseAccountManager = FindObjectOfType<FirebaseAccountManager>();
        }

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            
        }
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            StartCoroutine(InitAfterSpawnHandler());
        }
        
    }
    private IEnumerator InitAfterSpawnHandler()
    {
        Debug.Log("찾기");
        yield return new WaitUntil(() => FindObjectOfType<NetworkSpawnHandler>() != null);
        yield return new WaitUntil(() => FindObjectOfType<GameRuleManager>() != null);
        Debug.Log("찾기완료");

        // 이제 안전하게 초기화 코드 실행
        spawnHandler = FindObjectOfType<NetworkSpawnHandler>();
        cameraManager     = FindObjectOfType<CameraManager>();
        inputHandler   = FindObjectOfType<NetworkInputHandler>();
        gameRuleManager   = FindObjectOfType<GameRuleManager>();
  

        runner.AddCallbacks(spawnHandler);
        runner.AddCallbacks(inputHandler);
   
        
        //gameRuleManager.StartCupGame(GameType.Race);

        // 모든 플레이어에 대해 처리
        foreach (var player in runner.ActivePlayers)
        {
            // 1) 인덱스 계산
            int idx = GetPlayerJoinIndex(player);
            Vector3 offset = spawnOffsets[Mathf.Clamp(idx, 0, spawnOffsets.Length - 1)];

            if (player == runner.LocalPlayer)
                cameraManager.SetupMainCam(offset);
            else
                cameraManager.SetupMiniCam(offset, GetMiniCamIndexForPlayer(player));
            // 3) 블록 스폰
            if (runner.IsServer)
            {
                {
                    if (heartbeatRoutine != null)
                    {
                        StopCoroutine(heartbeatRoutine);
                        heartbeatRoutine = null;
                    }
                    if (runner.IsServer)
                    {
                        if(firebaseAccountManager==null) firebaseAccountManager = FindObjectOfType<FirebaseAccountManager>();
                        firebaseAccountManager.DeleteSessionDocument(sessionName);

                    }
                }
                gameRuleManager.StartCupGame(GameType.Survival);
                spawnHandler.SpawnBlockFor(runner, player, offset);
            }
            
        }
        
        
    }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}
