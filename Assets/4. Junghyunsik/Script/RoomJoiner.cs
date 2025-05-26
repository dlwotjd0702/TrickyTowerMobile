using UnityEngine;

public class RoomJoiner : MonoBehaviour
{
    [Header("NetworkManager 참조")]
    [SerializeField] private NetworkManager networkManager;

    /// <summary>
    /// Button OnClick 에 연결 → 인자로 들어온 세션명(sessionName) 으로 Join
    /// </summary>
    /// <param name="sessionName">클릭한 룸의 이름</param>
    public void JoinRoom(string sessionName)
    {
        // 네트워크 매니저에 세션명 세팅
        networkManager.sessionName = sessionName;
        // 클라이언트로 조인 시도
        networkManager.StartClient();
    }
}