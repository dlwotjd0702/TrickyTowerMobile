using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ModeBackgroundSprites
{
    [Tooltip("Inspector에 표시될 모드 이름 (Race, Survival, Puzzle…)")]
    public string modeName;

    [Header("Water")]
    [Tooltip("water 에 사용할 스프라이트")]
    public Sprite waterSprite;
    [Tooltip("water 을 반짝 숨기려면 체크")]
    public bool hideWater = false;

    [Header("Cloud Up (cloudUp1~5)")]
    public Sprite[] upSprites;         // 길이 5
    public bool hideUpClouds = false;

    [Header("Cloud Down (cloudDown1~5)")]
    public Sprite[] downSprites;      // 길이 5
    public bool hideDownClouds = false;

    [Header("Mountain (mountain1~3)")]
    public Sprite[] mountainSprites;  // 길이 3
    public bool hideMountains = false;
}

public class ModeBackgroundSwitcher : MonoBehaviour
{
    [Header("Water Image (1개)")]
    public Image waterImage;

    [Header("Cloud Up Images (순서대로 cloudUp1~5)")]
    public Image[] upCloudImages = new Image[5];

    [Header("Cloud Down Images (순서대로 cloudDown1~5)")]
    public Image[] downCloudImages = new Image[5];

    [Header("Mountain Images (순서대로 mountain1~3)")]
    public Image[] mountainImages = new Image[3];

    [Header("모드별 배경 스프라이트 세트")]
    public ModeBackgroundSprites[] modeSets;

    /// <summary>
    /// modeIndex 번째 모드로 배경 요소 전체를 교체합니다.
    /// </summary>
    public void SetBackground(int modeIndex)
    {
        if (modeSets == null || modeIndex < 0 || modeIndex >= modeSets.Length)
            return;

        var set = modeSets[modeIndex];

        // --- Water ---
        if (waterImage != null)
        {
            waterImage.gameObject.SetActive(!set.hideWater && set.waterSprite != null);
            if (!set.hideWater && set.waterSprite != null)
                waterImage.sprite = set.waterSprite;
        }

        // --- Cloud Up ---
        if (upCloudImages != null)
        {
            if (set.hideUpClouds)
            {
                foreach (var img in upCloudImages)
                    if (img != null) img.gameObject.SetActive(false);
            }
            else
            {
                for (int i = 0; i < upCloudImages.Length; i++)
                {
                    var img = upCloudImages[i];
                    if (img == null) continue;
                    bool has = i < set.upSprites.Length && set.upSprites[i] != null;
                    img.gameObject.SetActive(has);
                    if (has) img.sprite = set.upSprites[i];
                }
            }
        }

        // --- Cloud Down ---
        if (downCloudImages != null)
        {
            if (set.hideDownClouds)
            {
                foreach (var img in downCloudImages)
                    if (img != null) img.gameObject.SetActive(false);
            }
            else
            {
                for (int i = 0; i < downCloudImages.Length; i++)
                {
                    var img = downCloudImages[i];
                    if (img == null) continue;
                    bool has = i < set.downSprites.Length && set.downSprites[i] != null;
                    img.gameObject.SetActive(has);
                    if (has) img.sprite = set.downSprites[i];
                }
            }
        }

        // --- Mountain ---
        if (mountainImages != null)
        {
            if (set.hideMountains)
            {
                foreach (var img in mountainImages)
                    if (img != null) img.gameObject.SetActive(false);
            }
            else
            {
                for (int i = 0; i < mountainImages.Length; i++)
                {
                    var img = mountainImages[i];
                    if (img == null) continue;
                    bool has = i < set.mountainSprites.Length && set.mountainSprites[i] != null;
                    img.gameObject.SetActive(has);
                    if (has) img.sprite = set.mountainSprites[i];
                }
            }
        }
    }
}
