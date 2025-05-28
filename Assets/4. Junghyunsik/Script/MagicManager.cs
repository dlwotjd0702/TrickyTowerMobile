using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MagicManager : MonoBehaviour
{
    [Header("Magic 버튼들 (순서대로 magic0, magic1, magic2)")]
    [SerializeField] private List<Button> magicButtons = new List<Button>(3);

    [Header("현재 조작 중인 블록 정보가 있는 매니저")]
    [SerializeField] private EffectManager effectManager;

    private void Awake()
    {
        if (effectManager == null)
            effectManager = FindObjectOfType<EffectManager>();

        for (int i = 0; i < magicButtons.Count; i++)
        {
            int idx = i;
            magicButtons[idx].onClick.RemoveAllListeners();
            magicButtons[idx].onClick.AddListener(() => UseMagic(idx));
            magicButtons[idx].gameObject.SetActive(false);
        }
    }

    public void ActivateMagic(int index)
    {
        if (index < 0 || index >= magicButtons.Count) return;
        Debug.Log($"[MagicManager] ActivateMagic({index}) 호출");
        magicButtons[index].gameObject.SetActive(true);
    }

    private void UseMagic(int index)
    {
        Debug.Log($"[MagicManager] UseMagic({index}) 버튼 클릭");
        var blockGO = effectManager.Block;
        if (blockGO == null)
        {
            Debug.LogWarning("[MagicManager] 현재 블록이 없습니다!");
        }
        else
        {
            var nbc = blockGO.GetComponent<NetworkBlockController>();
            if (nbc == null)
            {
                Debug.LogWarning("[MagicManager] NetworkBlockController를 찾을 수 없습니다!");
            }
            else
            {
                switch (index)
                {
                    case 0:
                        nbc.fastFallTrigger = true;
                        Debug.Log("[MagicManager] fastFall 트리거 설정!");
                        break;
                    case 1:
                        nbc.noRotateTrigger = true;
                        Debug.Log("[MagicManager] noRotateTrigger 트리거 설정!");
                        break;
                    case 2:
                        nbc.biggerTrigger = true;
                        Debug.Log("[MagicManager] biggerTrigger 트리거 설정!");
                        break;
                }
            }
        }
        magicButtons[index].gameObject.SetActive(false);
    }

    public bool HasActiveMagic()
    {
        for (int i = 0; i < magicButtons.Count; i++)
            if (magicButtons[i].gameObject.activeSelf)
                return true;
        return false;
    }
}