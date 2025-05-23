using UnityEngine;

public class CloudScroller : MonoBehaviour
{
    public float speed = 4f; // 구름 이동 속도 (px/sec)
    public float resetX = -525f; // 왼쪽으로 순간이동할 위치
    public float maxX = 195f; // 오른쪽 끝 위치

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // 오른쪽으로 이동
        rectTransform.anchoredPosition += Vector2.right * speed * Time.deltaTime;

        // 만약 오른쪽 끝에 도달했다면 다시 왼쪽으로 순간이동
        if (rectTransform.anchoredPosition.x >= maxX)
        {
            rectTransform.anchoredPosition = new Vector2(resetX, rectTransform.anchoredPosition.y);
        }
    }
}