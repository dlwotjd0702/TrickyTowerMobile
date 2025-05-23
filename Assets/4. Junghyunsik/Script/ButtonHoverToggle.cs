using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class ButtonHoverToggle : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [Tooltip("마우스 아웃 상태 이미지")]
    [SerializeField] private GameObject offImage;
    [Tooltip("마우스 오버 시 켤 이미지")]
    [SerializeField] private GameObject onImage;

    // 모든 토글 버튼을 추적
    private static readonly List<ButtonHoverToggle> _allToggles = new List<ButtonHoverToggle>();

    private void Awake()
    {
        _allToggles.Add(this);
    }

    private void OnDestroy()
    {
        _allToggles.Remove(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 1) 다른 모든 버튼들 OFF
        foreach (var btn in _allToggles)
            btn.SetIcon(false);

        // 2) 지금 자신만 ON
        SetIcon(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 자신만 OFF
        SetIcon(false);
    }

    // helper: on==true → onImage 켜고 offImage 끄기
    private void SetIcon(bool on)
    {
        offImage.SetActive(!on);
        onImage.SetActive(on);
    }
}