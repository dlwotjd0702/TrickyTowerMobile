using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ModeConfig
{
    [Tooltip("Race, Survival, Puzzle 등 모드 이름")]
    public string modeName;

    [Header("Water")]
    public Sprite waterSprite;
    public bool   hideWater;

    [Header("Cloud Up (1~5)")]
    public Sprite[] upCloudSprites   = new Sprite[5];
    public bool     hideUpClouds;

    [Header("Cloud Down (1~5)")]
    public Sprite[] downCloudSprites = new Sprite[5];
    public bool     hideDownClouds;

    [Header("Mountain (1~3)")]
    public Sprite[] mountainSprites  = new Sprite[3];
    public bool     hideMountains;

    [Header("Sky Objects")]
    public bool showSun;
    public bool showMoon;
    public bool showStars;
}

public class ModeBackgroundUIManager : MonoBehaviour
{
    [Header("UI Images (Canvas 아래)")]
    public Image   waterImage;
    public Image[] upCloudImages   = new Image[5];
    public Image[] downCloudImages = new Image[5];
    public Image[] mountainImages  = new Image[3];

    [Header("Sky GameObjects")]
    public GameObject sunObject;
    public GameObject moonObject;
    public GameObject starContainer;

    [Header("모드별 설정 (크기를 모드 수로)")]
    public ModeConfig[] modeConfigs;

    /// <summary>
    /// modeIndex번째 설정만큼 UI만 바꿔 줍니다.
    /// </summary>
    public void SetMode(int modeIndex)
    {
        if (modeConfigs == null ||
            modeIndex < 0 ||
            modeIndex >= modeConfigs.Length)
            return;

        var cfg = modeConfigs[modeIndex];

        // Water
        if (waterImage != null)
        {
            waterImage.gameObject.SetActive(!cfg.hideWater && cfg.waterSprite != null);
            if (!cfg.hideWater) waterImage.sprite = cfg.waterSprite;
        }

        // Cloud Up
        for (int i = 0; i < upCloudImages.Length; i++)
        {
            bool has = !cfg.hideUpClouds
                       && i < cfg.upCloudSprites.Length
                       && cfg.upCloudSprites[i] != null;
            var img = upCloudImages[i];
            if (img != null)
            {
                img.gameObject.SetActive(has);
                if (has) img.sprite = cfg.upCloudSprites[i];
            }
        }

        // Cloud Down
        for (int i = 0; i < downCloudImages.Length; i++)
        {
            bool has = !cfg.hideDownClouds
                       && i < cfg.downCloudSprites.Length
                       && cfg.downCloudSprites[i] != null;
            var img = downCloudImages[i];
            if (img != null)
            {
                img.gameObject.SetActive(has);
                if (has) img.sprite = cfg.downCloudSprites[i];
            }
        }

        // Mountains
        for (int i = 0; i < mountainImages.Length; i++)
        {
            bool has = !cfg.hideMountains
                       && i < cfg.mountainSprites.Length
                       && cfg.mountainSprites[i] != null;
            var img = mountainImages[i];
            if (img != null)
            {
                img.gameObject.SetActive(has);
                if (has) img.sprite = cfg.mountainSprites[i];
            }
        }

        // Sun / Moon / Stars
        if (sunObject     != null) sunObject    .SetActive(cfg.showSun);
        if (moonObject    != null) moonObject   .SetActive(cfg.showMoon);
        if (starContainer != null) starContainer.SetActive(cfg.showStars);
    }
}