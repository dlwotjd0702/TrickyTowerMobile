using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class SpriteSwitcher : MonoBehaviour
{
    [Tooltip("0: Race, 1: Survival, 2: Puzzle")]
    public Material[] modeMaterials;

    RawImage _rawImage;

    void Awake()
    {
        _rawImage = GetComponent<RawImage>();
    }

    /// <summary>
    /// RawImage.material만 교체합니다.
    /// </summary>
    public void SetModeMaterial(int modeIndex)
    {
        if (modeMaterials == null 
            || modeIndex < 0 
            || modeIndex >= modeMaterials.Length)
            return;

        _rawImage.material = modeMaterials[modeIndex];
    }
}