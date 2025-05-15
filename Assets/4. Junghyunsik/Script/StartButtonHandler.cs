using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartButtonHandler : MonoBehaviour
{
    public GameObject introUI;  // Intro 그룹 (ID/PW)
    public GameObject mainUI;   // Main 그룹 (메뉴 등)
    public Image blackOverlay; // 전체를 덮는 검은 이미지 (alpha 조절용)
    public float fadeTime = 0.5f;

    public void OnStartButtonPressed()
    {
        StartCoroutine(FadeToMain());
    }

    private IEnumerator FadeToMain()
    {
        // 1. 검은 화면 점등 (alpha 0 → 1)
        blackOverlay.gameObject.SetActive(true);
        Color color = blackOverlay.color;
        color.a = 0f;
        blackOverlay.color = color;

        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, t / fadeTime);
            blackOverlay.color = color;
            yield return null;
        }

        // 2. UI 전환
        introUI.SetActive(false);
        mainUI.SetActive(true);

        // 3. 검은 화면 제거 (alpha 1 → 0)
        t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, t / fadeTime);
            blackOverlay.color = color;
            yield return null;
        }

        // ✅ 반드시 마지막에 비활성화 처리
        blackOverlay.gameObject.SetActive(false);
    }
}