using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class BlockSpringAnimator : MonoBehaviour
{
    [Header("Animation Targets")]
    [Tooltip("Block0, Block1, Block2, Block3 의 RectTransform을 순서대로 할당")]
    public RectTransform[] blockRects;

    [Header("Spring Settings")]
    [Tooltip("수축할 너비 (예: 130)")]
    public float contractedWidth = 130f;
    [Tooltip("팽창할 너비 (예: 150)")]
    public float expandedWidth   = 150f;
    [Tooltip("한 번 Tween에 걸릴 시간")]
    public float tweenDuration   = 0.1f;

    // 각 block 의 원래 너비 저장
    private float[] normalWidths;

    void Awake()
    {
        // 배열 크기만큼 원래 크기 저장
        normalWidths = new float[blockRects.Length];
        for (int i = 0; i < blockRects.Length; i++)
        {
            if (blockRects[i] != null)
                normalWidths[i] = blockRects[i].sizeDelta.x;
        }
    }

    /// <summary>
    /// 지정한 인덱스의 block만
    /// 원래→수축→팽창→원래
    /// 스프링 애니메이션 재생
    /// </summary>
    public void PlaySpring(int index)
    {
        if (index < 0 || index >= blockRects.Length) return;
        var rt = blockRects[index];
        if (rt == null) return;

        float normalW = normalWidths[index];
        float ypos    = rt.sizeDelta.y; // 세로 크기는 그대로

        // 시퀀스로 묶어서 순차 재생
        Sequence seq = DOTween.Sequence();

        // 1) 원래 → 수축
        seq.Append(
            rt.DOSizeDelta(new Vector2(contractedWidth, ypos), tweenDuration)
              .SetEase(Ease.OutQuad)
        );

        // 2) 수축 → 팽창
        seq.Append(
            rt.DOSizeDelta(new Vector2(expandedWidth, ypos), tweenDuration)
              .SetEase(Ease.OutElastic)
        );

        // 3) 팽창 → 원래
        seq.Append(
            rt.DOSizeDelta(new Vector2(normalW, ypos), tweenDuration)
              .SetEase(Ease.OutElastic)
        );
    }

    /// <summary>
    /// 혹시 모든 블록에 동시에 효과를 주고 싶으면 이 메서드를 호출하세요.
    /// </summary>
    public void PlaySpringAll()
    {
        for (int i = 0; i < blockRects.Length; i++)
            PlaySpring(i);
    }
}