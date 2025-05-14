using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSettingsUI : MonoBehaviour
{
    public AudioMixer audioMixer;

    [Header("볼륨 관련")]
    public Image[] sfxBars;
    public Image[] musicBars;
    public Sprite barOn;
    public Sprite barOff;

    private int sfxVolume = 5;   
    private int musicVolume = 5;

    private const int maxVolume = 5; 

    public void ChangeSFXVolume(int delta)
    {
        sfxVolume = Mathf.Clamp(sfxVolume + delta, 0, maxVolume);
    
        // audioMixer.SetFloat("SFX", VolumeToDecibel(sfxVolume)); // 👉 주석 처리
        UpdateBarUI(sfxBars, sfxVolume);
    }

    public void ChangeMusicVolume(int delta)
    {
        musicVolume = Mathf.Clamp(musicVolume + delta, 0, maxVolume);
    
        // audioMixer.SetFloat("BGM", VolumeToDecibel(musicVolume)); // 👉 주석 처리
        UpdateBarUI(musicBars, musicVolume);
    }

    private float VolumeToDecibel(int volume)
    {
        if (volume == 0) return -80f; // 거의 무음
        return Mathf.Lerp(-40f, 0f, volume / (float)maxVolume); // -40dB ~ 0dB
    }

    private void UpdateBarUI(Image[] bars, int currentVolume)
    {
        for (int i = 0; i < bars.Length; i++)
        {
            bars[i].sprite = (i < currentVolume) ? barOn : barOff;
        }
    }
}