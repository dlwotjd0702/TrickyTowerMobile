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
    }

    [SerializeField] private Slot[] slots = new Slot[4];

    /// <summary>
    /// 특정 슬롯 하나만 토글하고, 플레이어 이름을 세팅합니다.
    /// </summary>
    /// <param name="index">토글할 슬롯 인덱스</param>
    /// <param name="isActive">true면 활성화, false면 비활성화</param>
    /// <param name="playerName">활성화할 때 표시할 플레이어 이름</param>
    public void ToggleSlot(int index, bool isActive, string playerName = "")
    {
        if (index < 0 || index >= slots.Length) return;

        slots[index].playerIn.SetActive(isActive);
        slots[index].playerWaiting.SetActive(!isActive);
     
    }

    /// <summary>
    /// 주어진 인덱스 목록에 따라 각 슬롯을 활성화/비활성화하고 이름을 세팅합니다.
    /// </summary>
    public void UpdateSlots(Dictionary<int,string> activeWithNames)
    {
        // 모두 리셋
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].playerIn.SetActive(false);
            slots[i].playerWaiting.SetActive(true);
           
        }

        // 전달된 인덱스 + 이름 정보만 활성화
        foreach (var kv in activeWithNames)
        {
            int idx = kv.Key;
            string name = kv.Value;
            if (idx < 0 || idx >= slots.Length) continue;

            slots[idx].playerIn.SetActive(true);
            slots[idx].playerWaiting.SetActive(false);
          
        }
    }

    /// <summary>
    /// 모든 슬롯을 기본 상태(모두 대기)로 초기화합니다.
    /// </summary>
    public void ClearAllSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].playerIn.SetActive(false);
            slots[i].playerWaiting.SetActive(true);
        
        }
    }
}