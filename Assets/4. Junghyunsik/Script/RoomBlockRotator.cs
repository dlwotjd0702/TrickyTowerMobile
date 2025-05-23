using UnityEngine;
using DG.Tweening;

public class RoomBlockRotator : MonoBehaviour
{
    [Header("회전할 RectTransform")]
    [SerializeField] private RectTransform blockRect;

    [Header("뒤로 튕기는 각도 (°)")]
    [SerializeField] private float overshootAngle = 10f;
    [Header("뒤로 튕기는 시간 (초)")]
    [SerializeField] private float overshootDuration = 0.05f;
    [Header("뒤로 튕긴 후 멈춤 시간 (초)")]
    [SerializeField] private float overshootPause = 0.1f;

    [Header("목표각도로 빠르게 들어오는 시간 (초)")]
    [SerializeField] private float settleDuration = 0.02f;    // 0.02초 = 0.00몇 초
    [Header("도착 직후 떨림(punch) 지속 시간 (초)")]
    [SerializeField] private float punchDuration = 0.05f;
    [Header("도착 직후 떨림 강도 (°)")]
    [SerializeField] private float punchAngle = 5f;
    [Header("도착 직후 떨림 횟수 (vibrato)")]
    [SerializeField] private int punchVibrato = 3;

    [Header("한 스텝 후 전체 대기 시간 (초)")]
    [SerializeField] private float stepInterval = 1f;

    private readonly float[] targets = { 90f, 180f, 270f, 0f };

    void Start()
    {
        if (blockRect == null)
            blockRect = GetComponent<RectTransform>();

        Sequence seq = DOTween.Sequence()
            .SetLoops(-1, LoopType.Restart);

        float prev = blockRect.localEulerAngles.z;
        foreach (var t in targets)
        {
            // 1) 뒤로 튕기며 회전
            seq.Append(
                blockRect.DOLocalRotate(
                    new Vector3(0, 0, prev - overshootAngle),
                    overshootDuration
                ).SetEase(Ease.OutQuad)
            );
            // 2) 잠깐 멈춤
            seq.AppendInterval(overshootPause);

            // 3) 순식간에 목표각도로 진입 (0.02초)
            seq.Append(
                blockRect.DOLocalRotate(
                    new Vector3(0, 0, t),
                    settleDuration
                ).SetEase(Ease.OutQuad)
            );

            // 4) 도착 직후 살짝 떨림(punch)
            seq.Append(
                blockRect.DOPunchRotation(
                    new Vector3(0, 0, punchAngle),
                    punchDuration,
                    punchVibrato,
                    elasticity: 0f
                ).SetEase(Ease.OutQuad)
            );

            // 5) 다음 스텝 전 대기
            seq.AppendInterval(stepInterval);

            prev = t;
        }
    }
}