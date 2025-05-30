using UnityEngine;
using UnityEngine.UI;

public class RawImageManager : MonoBehaviour
{
    [Header("메인 RawImage")]
    [SerializeField] private RawImage mainRaw;

    [Header("미니 RawImages")]
    [SerializeField] private RawImage[] miniRaws;

    /// <summary>
    /// 카메라 매니저의 SetupMainCam과 동일한 분할(아래 2/3)에 RawImage를 세팅
    /// </summary>
    public void SetupMainRawImage()
    {
        if (mainRaw == null) return;
        mainRaw.enabled = true;

        var rt = mainRaw.rectTransform;
        // 화면 아래 2/3 영역
        rt.anchorMin   = new Vector2(0f,    0f);
        rt.anchorMax   = new Vector2(1f, 2f/3f);
        rt.offsetMin   = Vector2.zero;
        rt.offsetMax   = Vector2.zero;
    }

    /// <summary>
    /// 카메라 매니저의 SetupMiniCam과 동일한 분할(위 1/3을 N등분)에 RawImage를 세팅
    /// </summary>
    public void SetupMiniRawImage(int index)
    {
        if (miniRaws == null || index < 0 || index >= miniRaws.Length) return;
        var raw = miniRaws[index];
        raw.enabled = true;

        int count = miniRaws.Length;
        float w    = 1f / count;
        float yMin = 2f / 3f;
        float yMax = 1f;

        var rt = raw.rectTransform;
        // 위 1/3을 count 등분
        rt.anchorMin   = new Vector2(index * w,    yMin);
        rt.anchorMax   = new Vector2((index + 1) * w, yMax);
        rt.offsetMin   = Vector2.zero;
        rt.offsetMax   = Vector2.zero;
    }
}