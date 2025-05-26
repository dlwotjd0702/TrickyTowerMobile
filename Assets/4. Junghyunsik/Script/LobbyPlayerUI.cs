
using UnityEngine;
using TMPro;

public class LobbyPlayerUI : MonoBehaviour
{
    [Header("플레이어 슬롯 (최대 4명)")]
    [Tooltip("플레이어 입장 시 활성화할 오브젝트")]
    public GameObject playerIn;

    [Tooltip("대기 중일 때 활성화할 오브젝트")]
    public GameObject playerWaiting;

    [Tooltip("PlayerIn 안에 있는 이름 표시용 TextMeshProUGUI")]
    public TextMeshProUGUI nameText;

    /// <summary>플레이어가 입장했을 때</summary>
    public void ShowPlayer(string playerName)
    {
        if (playerIn      != null) playerIn     .SetActive(true);
        if (playerWaiting != null) playerWaiting.SetActive(false);
        if (nameText      != null) nameText     .text = playerName;
    }

    /// <summary>아직 빈 슬롯(대기 중)일 때</summary>
    public void ShowWaiting(string waitingText)
    {
        if (playerIn      != null) playerIn     .SetActive(false);
        if (playerWaiting != null) playerWaiting.SetActive(true);
        if (nameText      != null) nameText     .text = waitingText;
    }
}