using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIButtonManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject introUI;
    public GameObject mainUI;
    public GameObject optionUI;
    public GameObject helpUI;
    public GameObject playSetUI;
    public GameObject modSetUI;
    public GameObject readyRoomUI;
    public GameObject exitPopup;
    
    [Header("페이드")]
    public Image blackOverlay;
    public float fadeTime = 0.5f;

    private bool isTransitioning = false;

    public void HandleButton(string buttonId)
    {
        if (isTransitioning) return;

        switch (buttonId)
        {
            case "Start":
                StartCoroutine(FadeToMainUI());
                break;

            case "Option":
                mainUI.SetActive(false);
                optionUI.SetActive(true);
                break;

            case "Help":
                mainUI.SetActive(false);
                helpUI.SetActive(true);
                break;

            case "Back":
                helpUI.SetActive(false);
                optionUI.SetActive(false);
                playSetUI.SetActive(false); 
                mainUI.SetActive(true);
                break;

            case "MultiPlay":
                mainUI.SetActive(false);
                playSetUI.SetActive(true); 
                break;
            
            case "MakeRoom":
                playSetUI.SetActive(false);
                modSetUI.SetActive(true);
                break;

            case "EnterRoom":
                playSetUI.SetActive(false);
                readyRoomUI.SetActive(true);
                break;
            
            case "Exit":
                exitPopup.SetActive(true);
                break;

            case "ExitCancel":
                exitPopup.SetActive(false);
                break;
        }
    }

    private IEnumerator FadeToMainUI()
    {
        isTransitioning = true;

        if (blackOverlay != null)
        {
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
        }

        introUI.SetActive(false);
        mainUI.SetActive(true);

        if (blackOverlay != null)
        {
            float t = 0f;
            Color color = blackOverlay.color;
            while (t < fadeTime)
            {
                t += Time.deltaTime;
                color.a = Mathf.Lerp(1f, 0f, t / fadeTime);
                blackOverlay.color = color;
                yield return null;
            }

            blackOverlay.gameObject.SetActive(false);
        }

        isTransitioning = false;
    }
}