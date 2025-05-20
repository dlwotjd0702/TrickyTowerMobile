using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class MultiPlayHoverSwap : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("마우스 아웃 상태 이미지")]
    [SerializeField] private GameObject offImage;
    [Tooltip("마우스 오버 시 켤 이미지")]
    [SerializeField] private GameObject onImage;

    public void OnPointerEnter(PointerEventData eventData)
    {
        offImage.SetActive(false);
        onImage.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onImage.SetActive(false);
        offImage.SetActive(true);
    }
}