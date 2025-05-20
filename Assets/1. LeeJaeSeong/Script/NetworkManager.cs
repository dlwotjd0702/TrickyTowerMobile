using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using Fusion.Addons.Physics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("외부 연결")]
    public NetworkInputHandler inputHandler;
    public NetworkSpawnHandler spawnHandler;
    public CameraManager       cameraManager;

    [Header("네트워크 설정")]
    [SerializeField] string sessionName = "TrickyTower";
    public Vector3[] spawnOffsets = new Vector3[4];

    NetworkRunner _runner;
    private void OnGUI()
    {
        if (_runner == null)
        {
            if (GUI.Button(new Rect(0,0,200,40), "Host"))
            {
                StartHost();
            }
            if (GUI.Button(new Rect(0,40,200,40), "Join"))
            {
                StartClient();
            }
        }
    }
   

    public async void StartClient()
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;
        _runner.AddCallbacks(this);
        _runner.AddCallbacks(inputHandler);
        _runner.AddCallbacks(spawnHandler);

        var sceneMgr = gameObject.AddComponent<NetworkSceneManagerDefault>();
        var phys2D   = gameObject.AddComponent<RunnerSimulatePhysics2D>();
        phys2D.ClientPhysicsSimulation = ClientPhysicsSimulation.SimulateForward;

        var sceneRef = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        await _runner.StartGame(new StartGameArgs {
            GameMode     = GameMode.Client,
            SessionName  = sessionName,
            Scene        = sceneRef,
            SceneManager = sceneMgr
        });

        
    }


    public async void StartHost()
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;
        _runner.AddCallbacks(this);
        _runner.AddCallbacks(inputHandler);
        _runner.AddCallbacks(spawnHandler);

        var sceneMgr = gameObject.AddComponent<NetworkSceneManagerDefault>();
        // 호스트도 시뮬레이터는 필요하나, 모드는 서버 권위용
        gameObject.AddComponent<RunnerSimulatePhysics2D>();

        var sceneRef = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        await _runner.StartGame(new StartGameArgs {
            GameMode     = GameMode.Host,
            SessionName  = sessionName,
            Scene        = sceneRef,
            SceneManager = sceneMgr,
            PlayerCount  = 4
        });
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // 카메라 세팅만
        int idx = GetPlayerJoinIndex(player);
        Vector3 sp = spawnOffsets[Mathf.Clamp(idx,0,spawnOffsets.Length-1)];
        if (player == runner.LocalPlayer)
            cameraManager.SetupMainCam(sp);
        else
            cameraManager.SetupMiniCam(sp, GetMiniCamIndexForPlayer(player));
        if (runner.IsServer)
        {
            spawnHandler.SpawnBlockFor(runner,player,sp);
        }
    }

    public void RequestNextBlock(PlayerRef player)
    {
        int idx = GetPlayerJoinIndex(player);
        idx = Mathf.Clamp(idx, 0, spawnOffsets.Length - 1);
        Vector3 sp = spawnOffsets[idx];

        if (_runner.IsServer)
        {
            Debug.Log("호스트");
            spawnHandler.SpawnBlockFor(_runner, player, sp);
        }
    }
   
        
        

    public int GetPlayerJoinIndex(PlayerRef player)
    {
        var list = new List<PlayerRef>(_runner.ActivePlayers);
        list.Sort((a,b)=>a.RawEncoded.CompareTo(b.RawEncoded));
        return list.IndexOf(player);
    }

    int GetMiniCamIndexForPlayer(PlayerRef player)
    {
        var others = new List<PlayerRef>();
        foreach (var p in _runner.ActivePlayers)
            if (p != _runner.LocalPlayer) others.Add(p);
        others.Sort((a,b)=>a.RawEncoded.CompareTo(b.RawEncoded));
        int ix = others.IndexOf(player);
        return (ix>=0 && ix<3)? ix : 0;
    }

    // 이하 빈 콜백
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason reason) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken token) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }

    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}
