using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NetworkObject))]
public class NetworkSpawnHandler : NetworkBehaviour, INetworkRunnerCallbacks
{
    [Header("블럭 프리팹 풀")]
    public NetworkPrefabRef[] blockPrefabs;

    [Header("외부 참조")]
    public NetworkManager     networkManager;
    public EffectManager      effectManager;
    public SoundManager       soundManager;
    public IngameUiManager    ingameUiManager;
    public NetworkPrefabRef   winnerTrophy;

    // 서버 전용: 플레이어별 “다음 블럭” 매핑
    private readonly Dictionary<PlayerRef, int> nextBlockIndices = new();

    // 플레이어가 접속하면 한 번만 호출되어 다음 블럭을 뽑아둠
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer) return;
        int idx = GetRandomIndex();
        nextBlockIndices[player] = idx;
        RPC_UpdateNextBlock(player, idx);
    }

    // 플레이어가 떠나면 매핑 정리
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer) return;
        nextBlockIndices.Remove(player);
    }

    // 서버에서만 블럭을 스폰
    public void SpawnBlockFor(NetworkRunner runner, PlayerRef player, Vector3 spawnPoint)
    {
        if (!runner.IsServer) return;

        if (!GameClearManager.Instance.CanSpawn(player))
            return;

        // 1) 현재 저장된 인덱스로 스폰
        int currentIdx = nextBlockIndices.ContainsKey(player)
            ? nextBlockIndices[player]
            : GetRandomIndex();
        var prefab = blockPrefabs[currentIdx];
        var obj = runner.Spawn(prefab, spawnPoint, Quaternion.identity, player);

        SurvivalEvents.Spawned(player);

        // 2) 로컬 플레이어라면 효과/사운드 세팅
        if (player == runner.LocalPlayer && obj.TryGetComponent<NetworkBlockController>(out var ctrl))
        {
            ctrl.effectManager   = effectManager;
            ctrl.soundManager    = soundManager;
            ctrl.networkManager  = FindObjectOfType<NetworkManager>();
        }

        // 3) 다음 블럭 인덱스 새로 뽑아서 저장 & UI 전송
        int nextIdx = GetRandomIndex();
        nextBlockIndices[player] = nextIdx;
        RPC_UpdateNextBlock(player, nextIdx);
    }

    // 모든 클라이언트에 브로드캐스트, 로컬 플레이어만 UI 업데이트
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_UpdateNextBlock(PlayerRef targetPlayer, int nextIndex)
    {
        if (targetPlayer != Runner.LocalPlayer) return;
        ingameUiManager.NewBlockChoice(nextIndex);
    }

    private int GetRandomIndex()
    {
        return Random.Range(0, blockPrefabs.Length);
    }

    // INetworkRunnerCallbacks 빈 구현들
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason reason) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken token) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
}
