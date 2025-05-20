// 파일명: NetworkInputHandler.cs

using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class NetworkInputHandler : MonoBehaviour, INetworkRunnerCallbacks
{
    private int _prevRawX = 0;

    // 🔸 1프레임 키 입력 저장용
    private bool _rotateQueued = false;

    private void Update()
    {
        // 🔸 키 눌림 체크 → flag 저장
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
            _rotateQueued = true;
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        int rawX = (int)Input.GetAxisRaw("Horizontal");

        int moveX = 0;
        if (_prevRawX == 0 && rawX != 0)
            moveX = rawX;

        _prevRawX = rawX;

        // 🔸 저장된 키입력 사용 후 초기화
        var data = new NetworkBlockController.NetworkBlockInputData
        {
            MoveX    = moveX,
            Rotate   = _rotateQueued,
            FastDown = Input.GetKey(KeyCode.S)
        };

        _rotateQueued = false; // 🔸 초기화

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