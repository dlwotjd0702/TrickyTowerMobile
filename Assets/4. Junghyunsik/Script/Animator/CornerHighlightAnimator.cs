using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CornerHighlightAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("코너 강조 이미지들 (TopLeft, TopRight, BottomLeft, BottomRight 순서)")]
    [SerializeField] private RectTransform[] cornerImages = new RectTransform[4];

    [Header("움직일 거리 (픽셀)")]
    [SerializeField] private Vector2 moveOffset = new Vector2(5f, 5f);

    [Header("한 번에 이동할 시간 (초)")]
    [SerializeField] private float moveDuration = 0.6f;

    [Header("루프 간격 (한 사이클 후 다음까지 대기 시간)")]
    [SerializeField] private float loopInterval = 0.2f;

    private Vector3[] _origPos;
    private List<Tween> _tweens = new List<Tween>();

    void Awake()
    {
        // 원위치 저장
        _origPos = new Vector3[cornerImages.Length];
        for (int i = 0; i < cornerImages.Length; i++)
            _origPos[i] = cornerImages[i].localPosition;

        // 시작 시엔 숨기기(선택)
        foreach (var img in cornerImages)
            img.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 코너 이미지 활성화 & 애니메이션 시작
        for (int i = 0; i < cornerImages.Length; i++)
        {
            var img = cornerImages[i];
            img.gameObject.SetActive(true);

            // 방향 벡터 결정 (기존과 동일)
            float signX = (i == 0 || i == 2) ? -1f : 1f; // 0,2 = 왼쪽 / 1,3 = 오른쪽
            float signY = (i < 2)          ? 1f  : -1f; // 0,1 = 위쪽 / 2,3 = 아래쪽
            Vector3 target = _origPos[i] + new Vector3(moveOffset.x * signX, moveOffset.y * signY, 0);

            // **SetDelay 제거** → 동시에 시작
            var t = img
                .DOLocalMove(target, moveDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);

            _tweens.Add(t);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 모든 트윈 정리
        foreach (var t in _tweens) t.Kill();
        _tweens.Clear();

        // 위치 원복 & 숨기기
        for (int i = 0; i < cornerImages.Length; i++)
        {
            cornerImages[i].localPosition = _origPos[i];
            cornerImages[i].gameObject.SetActive(false);
        }
    }
}