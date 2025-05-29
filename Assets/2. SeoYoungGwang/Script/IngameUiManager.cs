// IngameUiManager.cs
using UnityEngine;
using UnityEngine.UI;

public class IngameUiManager : MonoBehaviour
{
    [Header("SpawnHandler 참조")]
    [SerializeField] private NetworkSpawnHandler networkSpawnHandler;

    [Header("다음 블럭 Preview 이미지들")]
    [SerializeField] private Image[] NextBlockImage;

    void Start()
    {
        // 초기에는 모두 숨김
        foreach (var img in NextBlockImage)
            img.gameObject.SetActive(false);
    }

    /// <summary>
    /// NetworkSpawnHandler에서 RPC로 호출됩니다.
    /// 전달받은 인덱스에 해당하는 이미지만 활성화합니다.
    /// </summary>
    public void NewBlockChoice(int index)
    {
        for (int i = 0; i < NextBlockImage.Length; i++)
        {
            NextBlockImage[i].gameObject.SetActive(i == index);
        }
    }
}