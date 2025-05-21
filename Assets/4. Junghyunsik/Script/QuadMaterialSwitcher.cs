using UnityEngine;

/// <summary>
/// Quad1의 MeshRenderer.material을 모드에 따라 바꿔줍니다.
/// </summary>
public class QuadMaterialSwitcher : MonoBehaviour
{
    [Header("Quad1 MeshRenderer (인스펙터에 드래그)")]
    public MeshRenderer quad1Renderer;

    [Header("Mode별 머테리얼")]
    [Tooltip("0: Race, 1: Survival, 2: Puzzle")]
    public Material[] modeMaterials;

    /// <summary>
    /// 0=Race,1=Survival,2=Puzzle
    /// </summary>
    public void SetModeMaterial(int modeIndex)
    {
        if (quad1Renderer == null || modeMaterials == null) return;
        if (modeIndex < 0 || modeIndex >= modeMaterials.Length) return;
        quad1Renderer.material = modeMaterials[modeIndex];
    }
}