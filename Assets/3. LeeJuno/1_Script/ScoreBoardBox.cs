using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardBox : MonoBehaviour
{
    [SerializeField]
    private Transform[] medalSlots;
    
    public void ResetBox()
    {
        foreach (var slot in medalSlots)
        {
            foreach (Transform child in slot)
                Destroy(child.gameObject);
        }
    }

    public void AddMedal(GameObject medalPrefab)
    {
        if (medalPrefab == null) return;

        // 첫 번째로 비어있는 슬롯을 찾아 메달 생성
        foreach (var slot in medalSlots)
        {
            if (slot.childCount == 0)
            {
                Instantiate(medalPrefab, slot);
                break;
            }
        }
    }
}