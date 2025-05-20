using UnityEngine;

public class LightImageRotator : MonoBehaviour
{
    [System.Serializable]
    public struct LightConfig
    {
        public RectTransform image;   // 회전시킬 UI 이미지
        public float speed;           // 초당 회전 속도 (절댓값)
        public bool clockwise;        // true 면 +Z(시계), false 면 –Z(반시계)
    }

    [Header("회전할 LightImage 설정 (1,2,3,4 순서)")]
    [SerializeField] private LightConfig[] lights = new LightConfig[4];

    void Update()
    {
        float dt = Time.deltaTime;
        for (int i = 0; i < lights.Length; i++)
        {
            var cfg = lights[i];
            if (cfg.image == null) continue;

            // 방향에 따라 부호 결정
            float dir = cfg.clockwise ? +1f : -1f;
            // Z축 회전
            cfg.image.Rotate(0, 0, cfg.speed * dir * dt, Space.Self);
        }
    }
}