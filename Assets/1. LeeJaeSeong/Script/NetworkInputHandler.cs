// 파일명: NetworkInputHandler.cs

using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class NetworkInputHandler : MonoBehaviour, INetworkRunnerCallbacks
{
    // 이전 틱에 읽은 축 값(−1, 0, +1)
    private int _prevRawX = 0;

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        // ① 지금 축 값 읽기 (−1,0,+1)
        int rawX = (int)Input.GetAxisRaw("Horizontal");

        // ② 0 → ±1 전환된 순간에만 한 칸 이동
        int moveX = 0;
        if (_prevRawX == 0 && rawX != 0)
            moveX = rawX;

        // ③ 다음 틱을 위해 이전 값 갱신
        _prevRawX = rawX;

        // 회전과 빠른 하강은 GetKeyDown/Key 로 처리
        bool rotate   = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W);
        bool fastDown = Input.GetKey(KeyCode.S);

        var data = new NetworkBlockInputData
        {
            MoveX    = moveX,
            Rotate   = rotate,
            FastDown = fastDown
        };

        input.Set(data);
    }



    public void OnConnectedToServer(NetworkRunner runner) {}
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress){}
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList){}
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){}
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){}
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {}
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {}
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {}
    public void OnShutdown(NetworkRunner runner, ShutdownReason reason) {}
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason){}
    public void OnDisconnectedFromServer(NetworkRunner runner) {}
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {}
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason){}
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message){}
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data){}
    public void OnSessionListUpdated(NetworkRunner runner, SessionInfo[] sessionList) {}
    public void OnCustomAuthenticationResponse(NetworkRunner runner, System.Collections.Generic.Dictionary<string, object> data) {}
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken token) {}
    public void OnSceneLoadStart(NetworkRunner runner) {}
    public void OnSceneLoadDone(NetworkRunner runner) {}
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, System.ArraySegment<byte> data) {}
}