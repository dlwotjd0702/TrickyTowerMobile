using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class StarPulseAnimator : MonoBehaviour
{
    [Header("별들이 들어있는 부모")]
    public Transform starContainer;

    [Header("펄스 배율의 최소/최대")]
    public float minScale = 0.9f;
    public float maxScale = 1.1f;

    [Header("한 번 사이클(축소→확대→원상) 걸리는 시간 범위")]
    public Vector2 durationRange = new Vector2(1f, 2f);

    void Start()
    {
        if (starContainer == null) starContainer = transform;
        foreach (Transform star in starContainer)
        {
            // 각 별마다 랜덤한 딜레이와 지속시간을 줘서 비동기 펄스
            float delay    = Random.Range(0f, 1f);
            float duration = Random.Range(durationRange.x, durationRange.y);

            // 시퀀스 생성
            Sequence seq = DOTween.Sequence()
                .AppendInterval(delay)
                // 축소
                .Append(star.DOScale(minScale, duration * 0.5f).SetEase(Ease.InOutSine))
                // 확대
                .Append(star.DOScale(maxScale, duration * 0.5f).SetEase(Ease.InOutSine))
                // 원상복귀
                .Append(star.DOScale(1f, duration * 0.5f).SetEase(Ease.InOutSine))
                // 반복 (무한)
                .SetLoops(-1, LoopType.Restart);
        }
    }
}