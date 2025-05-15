using UnityEngine;

public class MountainFloat : MonoBehaviour
{
    public float floatSpeed = 0.5f;      // 흔들리는 속도
    public float floatHeight = 4f;    // 위아래 진폭
    
    private Vector2 startPos;
    private float offset;              // 위상 오프셋 (랜덤 타이밍)

    void Start()
    {
        startPos = GetComponent<RectTransform>().anchoredPosition;
        offset = Random.Range(0f, Mathf.PI * 2f); // 0 ~ 2π 랜덤 위상
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed + offset) * floatHeight;
        GetComponent<RectTransform>().anchoredPosition = new Vector2(startPos.x, newY);
    }
}