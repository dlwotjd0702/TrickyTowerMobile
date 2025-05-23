using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BackgroundMaterialSwitcher : MonoBehaviour
{
    [Tooltip("0: Race, 1: Survival, 2: Puzzle 순서대로 할당")]
    public Material[] modeMaterials;

    Image _image;

    void Awake()
    {
        _image = GetComponent<Image>();
    }

    /// <summary>
    /// modeIndex 에 해당하는 Material 을 Image.material 에 적용합니다.
    /// </summary>
    public void SetModeMaterial(int modeIndex)
    {
        if (modeMaterials == null ||
            modeIndex < 0 ||
            modeIndex >= modeMaterials.Length)
            return;

        _image.material = modeMaterials[modeIndex];
    }
}