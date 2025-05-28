using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LobbyUIManager : MonoBehaviour
{
    [Serializable]
    public class Slot
    {
        public GameObject playerIn;
        public GameObject playerWaiting;
        public TextMeshProUGUI nameText;
    }

    [SerializeField]
    private Slot[] slots = new Slot[4];

    public string nickname;

    /// <summary>
    /// 주어진 인덱스 목록에 따라 각 슬롯을 활성화/비활성화합니다.
    /// 0~3 범위의 인덱스를 사용하십시오.
    /// </summary>
    /// <param name="activeIndices">활성화할 슬롯 인덱스 목록</param>
    public void UpdateSlots(IEnumerable<int> activeIndices)
    {
        // 모든 슬롯 초기화
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].playerIn?.SetActive(false);
            slots[i].playerWaiting?.SetActive(true);
        }

        Player.Instance.nickname = nickname;//이거 사용
        // 전달된 인덱스만 활성화
        foreach (var index in activeIndices)
        {
            if (index < 0 || index >= slots.Length)
            {
                Debug.LogWarning($"Invalid slot index: {index}");
                continue;
            }
            slots[index].playerIn?.SetActive(true);
            slots[index].playerWaiting?.SetActive(false);
        }
    }

    /// <summary>
    /// 특정 슬롯 하나만 토글합니다.
    /// </summary>
    /// <param name="index">토글할 슬롯 인덱스</param>
    /// <param name="isActive">true면 활성화, false면 비활성화</param>
    public void ToggleSlot(int index, bool isActive)
    {
        if (index < 0 || index >= slots.Length)
        {
            Debug.LogWarning($"Invalid slot index: {index}");
            return;
        }
        slots[index].playerIn?.SetActive(isActive);
        slots[index].playerWaiting?.SetActive(!isActive);
    }

    /// <summary>
    /// 모든 슬롯을 기본 상태(모두 대기)로 초기화합니다.
    /// </summary>
    public void ClearAllSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].playerIn?.SetActive(false);
            slots[i].playerWaiting?.SetActive(true);
        }
    }
}