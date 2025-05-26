using Fusion;
using Fusion.Sockets;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class NetworkSpawnHandler : NetworkBehaviour, INetworkRunnerCallbacks
{
    [Header("블럭 프리팹")]
    public NetworkPrefabRef[] blockPrefabs;

    [Header("외부 참조")]
    public NetworkManager networkManager;

    public EffectManager effectManager;
    public SoundManager soundManager;
    public IngameUiManager ingameUiManager;

    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
    public void RPC_RequestBlockSpawn(RpcInfo info = default)
    {
        var player = info.Source;

        if (GameClearManager.Instance.CanSpawn(player) == false) //블럭 스폰 차단중인지 검사
        {
            Debug.Log($"스폰 차단중 {player}");
            return;
        }

        int idx = networkManager.GetPlayerJoinIndex(player);
        Debug.Log("스폰되기전");
        if (idx < 0 || idx >= networkManager.spawnOffsets.Length) return;

        Vector3 sp = networkManager.spawnOffsets[idx];
        SpawnBlockFor(Runner, player, sp);
        Debug.Log("스폰되기후");
    }

    public void SpawnBlockFor(NetworkRunner runner, PlayerRef player, Vector3 spawnPoint)
    {
        //if(ingameUiManager.preIndex == null)
        //    ingameUiManager.preIndex = Random.Range(0, blockPrefabs.Length);
        //NetworkObject obj = runner.Spawn(blockPrefabs[ingameUiManager.preIndex], spawnPoint, Quaternion.identity, player);

        if (GameClearManager.Instance.CanSpawn(player) == false) //블럭 스폰 차단중인지 검사
        {
            Debug.Log($"스폰 차단중 {player}");
            return;
        }

        int i = Random.Range(0, blockPrefabs.Length);
        var obj = runner.Spawn(blockPrefabs[i], spawnPoint, Quaternion.identity, player);

        //서바이벌 모드 블럭스폰 이벤트
        SurvivalEvents.Spawned(player);

        if (player == runner.LocalPlayer)
        {
            if (obj.TryGetComponent<NetworkBlockController>(out var ctrl))
            {
                ctrl.effectManager = effectManager;
                ctrl.soundManager = soundManager;
                ctrl.networkManager = networkManager;
                effectManager.Block = obj.gameObject;
                effectManager.isBlockChange = true;
                soundManager.currentBlock = obj.gameObject;
            }
            //ingameUiManager.NewBlockChoice();
        }
    }


    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason reason)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key,
        System.ArraySegment<byte> data)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, System.Collections.Generic.List<SessionInfo> sessionList)
    {
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner,
        System.Collections.Generic.Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken token)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }
}