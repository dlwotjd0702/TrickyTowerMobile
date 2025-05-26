using UnityEngine;
using TMPro;

public class LobbyUIManager : MonoBehaviour
{
    [System.Serializable]
    public class Slot
    {
        public GameObject playerIn;
        public GameObject playerWaiting;
        public TextMeshProUGUI nameText;
    }

    [SerializeField] private Slot[] slots = new Slot[4];

    // ActivateNext/DeactivateNext 둘 다 이 인덱스를 기준으로 동작
    private int activateIndex = 0;

    private void Start()
    {
        ClearAllSlots();
    }

    /// <summary>다음 슬롯 하나만 활성화 (0→1→2→3)</summary>
    public void ActivateNext()
    {
        if (activateIndex >= slots.Length)
        {
            Debug.Log("모든 슬롯이 이미 활성화되어 있습니다.");
            return;
        }

        slots[activateIndex].playerIn?.SetActive(true);
        slots[activateIndex].playerWaiting?.SetActive(false);
        activateIndex++;
    }

    /// <summary>
    /// 마지막으로 활성화된 슬롯부터 비활성화 (ActivateNext 된 순서의 역순)
    /// 예: ActivateNext 3번 호출 후 activateIndex == 3, DeactivateNext 호출 시 index 2 비활성화
    /// </summary>
    public void DeactivateNext()
    {
        if (activateIndex <= 0)
        {
            Debug.Log("비활성화할 슬롯이 없습니다.");
            return;
        }

        // 방금 켠 슬롯의 인덱스로 이동
        activateIndex--;
        slots[activateIndex].playerIn?.SetActive(false);
        slots[activateIndex].playerWaiting?.SetActive(true);
    }

    public void ClearAllSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].playerIn?.SetActive(false);
            slots[i].playerWaiting?.SetActive(true);
        }
        activateIndex = 0;
    }
}