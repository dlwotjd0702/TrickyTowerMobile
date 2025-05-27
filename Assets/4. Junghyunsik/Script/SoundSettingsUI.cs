using UnityEngine;
using UnityEngine.UI;

public class SoundSettingsUI : MonoBehaviour
{
    [Header("볼륨 관련")]
    public Image[] sfxBars;
    public Image[] musicBars;
    public Sprite barOn;
    public Sprite barOff;

    private int sfxVolume;
    private int musicVolume;
    private const int maxVolume = 5;

    // PlayerPrefs 키
    private const string PREF_SFX   = "SFXVolume";
    private const string PREF_MUSIC = "MusicVolume";

    // 외부에서 읽을 수 있도록
    public int CurrentSfxVolume   => sfxVolume;
    public int CurrentMusicVolume => musicVolume;

    private void Awake()
    {
        // 1) 저장된 설정을 불러와 초기값 세팅
        sfxVolume   = PlayerPrefs.GetInt(PREF_SFX,   maxVolume);
        musicVolume = PlayerPrefs.GetInt(PREF_MUSIC, maxVolume);

        UpdateBarUI(sfxBars,   sfxVolume);
        UpdateBarUI(musicBars, musicVolume);

        // 2) 바로 사운드 매니저에도 적용
        if (OurSoundManager.Instance != null)
            OurSoundManager.Instance.SetMusicVolume(musicVolume);
        // 만약 SFXManager가 별도로 있다면 여기도 호출
        // SoundManager.Instance.SetSFXVolume(sfxVolume);
    }

    /// <summary>
    /// 좌/우 버튼으로 증감시키는 콜백
    /// </summary>
    public void ChangeMusicVolume(int delta)
    {
        SetMusicVolume(musicVolume + delta);
    }

    public void ChangeSFXVolume(int delta)
    {
        SetSfxVolume(sfxVolume + delta);
    }

    /// <summary>
    /// 외부 또는 Awake에서 직접 단계 값을 설정하고 반영
    /// </summary>
    public void SetMusicVolume(int volume)
    {
        musicVolume = Mathf.Clamp(volume, 0, maxVolume);
        UpdateBarUI(musicBars, musicVolume);
        if (OurSoundManager.Instance != null)
            OurSoundManager.Instance.SetMusicVolume(musicVolume);
    }

    public void SetSfxVolume(int volume)
    {
        sfxVolume = Mathf.Clamp(volume, 0, maxVolume);
        UpdateBarUI(sfxBars, sfxVolume);
        // SFX 매니저가 있으면 호출
        // SoundManager.Instance.SetSFXVolume(sfxVolume);
    }

    private void UpdateBarUI(Image[] bars, int currentVolume)
    {
        for (int i = 0; i < bars.Length; i++)
            bars[i].sprite = (i < currentVolume) ? barOn : barOff;
    }

    /// <summary>
    /// Save 버튼에 연결 → PlayerPrefs에 기록
    /// </summary>
    public void SavePreferences()
    {
        PlayerPrefs.SetInt(PREF_SFX,   sfxVolume);
        PlayerPrefs.SetInt(PREF_MUSIC, musicVolume);
        PlayerPrefs.Save();
        Debug.Log("SoundSettingsUI: 사운드 설정 저장됨");
    }

    /// <summary>
    /// Back 버튼에 연결 → 저장된 값으로 롤백
    /// </summary>
    public void DiscardChanges()
    {
        sfxVolume   = PlayerPrefs.GetInt(PREF_SFX,   maxVolume);
        musicVolume = PlayerPrefs.GetInt(PREF_MUSIC, maxVolume);

        UpdateBarUI(sfxBars,   sfxVolume);
        UpdateBarUI(musicBars, musicVolume);

        if (OurSoundManager.Instance != null)
            OurSoundManager.Instance.SetMusicVolume(musicVolume);

        Debug.Log("SoundSettingsUI: 변경사항 취소, 이전 설정으로 복원");
    }
}