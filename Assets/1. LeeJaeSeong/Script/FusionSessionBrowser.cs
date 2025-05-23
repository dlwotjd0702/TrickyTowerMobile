/*
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class FusionSessionBrowser : MonoBehaviour, INetworkRunnerCallbacks
{
    NetworkRunner lobbyRunner;
    List<SessionInfo> sessionList = new List<SessionInfo>();
    Vector2 scrollPos;

    async void Start()
    {
        // 1) 이 GameObject에 NetworkRunner 컴포넌트 추가
        lobbyRunner = gameObject.AddComponent<NetworkRunner>();
        lobbyRunner.ProvideInput = false;           // 입력 불필요
        lobbyRunner.AddCallbacks(this);

        // 2) Shared Lobby 접속
        var result = await lobbyRunner.JoinSessionLobby(SessionLobby.Shared, "");
        if (!result.Ok)
            Debug.LogError($"로비 접속 실패: {result.ShutdownReason}");
    }

    // 3) 콜백으로 갱신되는 세션 리스트
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> updated)
    {
        sessionList = updated;
        Debug.Log($"받은 세션 개수: {sessionList.Count}");
    }

    // 4) 간단한 OnGUI 예시
    void OnGUI()
    {
        if (GUI.Button(new Rect(10,10,150,30), "세션 새로고침"))
            lobbyRunner.GetSessionList();       // 수동 갱신 요청

        scrollPos = GUI.BeginScrollView(new Rect(10,50,200,300), scrollPos,
                                        new Rect(0,0,180, sessionList.Count*30));
        for (int i = 0; i < sessionList.Count; i++)
        {
            var info = sessionList[i];
            if (GUI.Button(new Rect(0, i*30, 160,25), info.Name))
            {
                Debug.Log("선택된 세션: " + info.Name);
                // 실제 참가 로직 호출
                NetworkManager.Instance.JoinHostSession(info.Name);
            }
        }
        GUI.EndScrollView();
    }

    // --- 나머지 INetworkRunnerCallbacks는 빈 구현 ---
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {}
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {}
    public void OnInput(NetworkRunner runner, NetworkInput input) {}
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {}
    public void OnShutdown(NetworkRunner runner, ShutdownReason reason) {}
    public void OnConnectedToServer(NetworkRunner runner) {}
    public void OnDisconnectedFromServer(NetworkRunner runner) {}
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {}
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {}
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {}
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) {}
    public void OnSceneLoadDone(NetworkRunner runner) {}
    public void OnSceneLoadStart(NetworkRunner runner) {}
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) {}
}
*/
