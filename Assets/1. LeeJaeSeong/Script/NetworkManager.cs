using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using Fusion.Addons.Physics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("외부 연결")]
    public NetworkInputHandler    inputHandler;
    public NetworkSpawnHandler    spawnHandler;
    public CameraManager          cameraManager;
    public FirebaseAccountManager firebaseAccountManager;
    public float                  heartbeatInterval = 10f;

    [Header("네트워크 설정")]
    [SerializeField] public string sessionName;
    public Vector3[]               spawnOffsets = new Vector3[4];

    private NetworkRunner runner;
    private bool          isGameStarted = false;
    private Coroutine     heartbeatRoutine;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        firebaseAccountManager = FindObjectOfType<FirebaseAccountManager>();
    }

    private void OnGUI()
    {
        int idx = SceneManager.GetActiveScene().buildIndex;

        if (idx == 0)
        {
            if (GUI.Button(new Rect(10, 10, 200, 40), "Host"))
                StartHost();
            if (GUI.Button(new Rect(10, 60, 200, 40), "Join"))
                StartClient();
        }
        else if (idx == 1 && runner != null && runner.IsServer && !isGameStarted)
        {
            if (GUI.Button(new Rect(10, 110, 200, 40), "Start Game"))
                HostStartGame();
        }
    }

    public async void StartHost()
    {
        SetupRunner();

        var result = await runner.StartGame(new StartGameArgs {
            GameMode    = GameMode.Host,
            SessionName = sessionName,
            PlayerCount = 4
        });

        if (!result.Ok)
        {
            Debug.LogError($"Host start failed: {result.ShutdownReason}");
            return;
        }

        if (heartbeatRoutine != null) StopCoroutine(heartbeatRoutine);
        heartbeatRoutine = StartCoroutine(HeartbeatCoroutine());

        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public async void StartClient()
    {
        SetupRunner();

        var result = await runner.StartGame(new StartGameArgs {
            GameMode    = GameMode.Client,
            SessionName = sessionName
        });

        if (!result.Ok)
        {
            Debug.LogError($"Client join failed: {result.ShutdownReason}");
            return;
        }

        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    private void SetupRunner()
    {
        if (runner != null) return;

        runner = gameObject.AddComponent<NetworkRunner>();
        DontDestroyOnLoad(runner.gameObject);

        runner.ProvideInput = true;
        runner.AddCallbacks(this);
        if (inputHandler != null) runner.AddCallbacks(inputHandler);
        if (spawnHandler != null) runner.AddCallbacks(spawnHandler);

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
        if (runner == null || !runner.IsServer || isGameStarted)
            return;

        isGameStarted = true;
        await runner.LoadScene(SceneRef.FromIndex(2), LoadSceneMode.Single);
    }

    public void RequestNextBlock(PlayerRef player)
    {
        if (!runner.IsServer) return;

        int idx    = GetPlayerJoinIndex(player);
        Vector3 sp = spawnOffsets[Mathf.Clamp(idx, 0, spawnOffsets.Length - 1)];
        spawnHandler?.SpawnBlockFor(runner, player, sp);
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

    public void OnShutdown(NetworkRunner runner, ShutdownReason reason)
    {
        if (heartbeatRoutine != null)
        {
            StopCoroutine(heartbeatRoutine);
            heartbeatRoutine = null;
        }

        if (runner.IsServer)
            firebaseAccountManager?.DeleteSessionDocument(sessionName);
    }

    // INetworkRunnerCallbacks 빈 메서드들
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken token) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}
