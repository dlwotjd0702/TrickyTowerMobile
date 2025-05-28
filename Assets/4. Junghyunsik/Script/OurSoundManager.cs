using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class OurSoundManager : MonoBehaviour
{
    public static OurSoundManager Instance { get; private set; }

    [Header("Inspector 에 드래그할 AudioClip")]
    [SerializeField] private AudioClip theme;

    private AudioSource src;
    private bool hasLoggedPlaying = false;  // 로그 한 번만 띄우기용
    private const int MAX_VOLUME = 5;

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        src = GetComponent<AudioSource>();
        src.clip = theme;
        src.loop = true;
        src.playOnAwake = false;
    }

    private void Start()
    {
        src.Play();
        Debug.Log($"[OurSoundManager] Play() 호출됨 — 클립: {theme.name}");
    }

    private void Update()
    {
        // 한 번만 재생 상태 로그
        if (!hasLoggedPlaying && src.isPlaying)
        {
            Debug.Log("[OurSoundManager] 현재 음악 재생 중입니다.");
            hasLoggedPlaying = true;
        }
    }

    /// <summary>
    /// 0~MAX_VOLUME 값을 받아 0~1 볼륨으로 변환해 적용
    /// </summary>
    public void SetMusicVolume(int v)
    {
        float norm = Mathf.Clamp01(v / (float)MAX_VOLUME);
        src.volume = norm;
        Debug.Log($"[OurSoundManager] 볼륨 변경: 단계 {v} → volume {norm:F2}");
    }
}