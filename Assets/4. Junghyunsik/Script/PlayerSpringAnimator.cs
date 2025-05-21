using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class PlayerSpringAnimator : MonoBehaviour
{
    [Header("Animation Targets")]
    [Tooltip("Player0, Player1, Player2, Player3 의 RectTransform을 순서대로 할당")]
    public RectTransform[] playerRects;

    [Header("Spring Settings")]
    [Tooltip("수축할 스케일 (예: 0.9)")]
    public float contractedScale = 0.9f;
    [Tooltip("팽창할 스케일 (예: 1.1)")]
    public float expandedScale   = 1.1f;
    [Tooltip("한 번 Tween에 걸릴 시간")]
    public float tweenDuration   = 0.1f;

    /// <summary>
    /// 지정한 인덱스의 플레이어만
    /// 원래→수축→팽창→원래
    /// 스프링 스케일 애니메이션 재생
    /// </summary>
    public void PlaySpring(int index)
    {
        if (index < 0 || index >= playerRects.Length) return;
        var rt = playerRects[index];
        if (rt == null) return;

        // 원래 스케일
        Vector3 normal = rt.localScale;

        // 시퀀스로 묶어서 순차 재생
        Sequence seq = DOTween.Sequence();

        // 1) 원래 → 수축
        seq.Append(
            rt.DOScale(normal * contractedScale, tweenDuration)
                .SetEase(Ease.OutQuad)
        );

        // 2) 수축 → 팽창
        seq.Append(
            rt.DOScale(normal * expandedScale, tweenDuration)
                .SetEase(Ease.OutElastic)
        );

        // 3) 팽창 → 원래
        seq.Append(
            rt.DOScale(normal, tweenDuration)
                .SetEase(Ease.OutElastic)
        );
    }

    /// <summary>
    /// 모든 플레이어에 동시에 스프링 애니메이션을 적용합니다.
    /// </summary>
    public void PlaySpringAll()
    {
        for (int i = 0; i < playerRects.Length; i++)
            PlaySpring(i);
    }
}