// 파일명: NetworkSpawnHandler.cs

using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class NetworkSpawnHandler : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetworkSpawnHandler Instance { get; private set; }

    [SerializeField] private NetworkPrefabRef[] blockPrefabs = new NetworkPrefabRef[7];
    [SerializeField] private Vector3 spawnPosition;
    public EffectManager effectManager;
    private NetworkRunner _runner;
    
    

    private void Awake()
    {
        // 싱글턴 초기화
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        _runner = runner;
        if (runner.IsServer)
            SpawnBlockFor(player);
    }

 
    public void RequestNextBlock(PlayerRef player)
    {
        if (!_runner.IsServer) return;
        SpawnBlockFor(player);
    }

    public void SpawnBlockFor(PlayerRef player)
    {
        if (blockPrefabs == null || blockPrefabs.Length == 0)
        {
            Debug.LogWarning("Block prefabs array is empty!");
            return;
        }
        int idx = UnityEngine.Random.Range(0, blockPrefabs.Length);

        NetworkPrefabRef chosenPrefab = blockPrefabs[idx];
        

        NetworkObject temp = _runner.Spawn(
            prefabRef      : chosenPrefab,
            position       : spawnPosition,
            rotation       : Quaternion.identity,
            inputAuthority : player
        );
        
        temp.transform.TryGetComponent(out NetworkBlockController blockController);
        blockController.effectManager = effectManager;
        effectManager.Block = temp.gameObject;
        effectManager.isBlockChange = true;


    }

    
    
    
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnConnectedToServer(NetworkRunner runner) {}
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {}
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) {}
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {}
    public void OnShutdown(NetworkRunner runner, ShutdownReason reason) {}
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) {}
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {}
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnSessionListUpdated(NetworkRunner runner, SessionInfo[] sessionList) {}
    public void OnCustomAuthenticationResponse(NetworkRunner runner, System.Collections.Generic.Dictionary<string, object> data) {}
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken token) {}
    public void OnSceneLoadStart(NetworkRunner runner) {}
    public void OnSceneLoadDone(NetworkRunner runner) {}
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, System.ArraySegment<byte> data) {}
}