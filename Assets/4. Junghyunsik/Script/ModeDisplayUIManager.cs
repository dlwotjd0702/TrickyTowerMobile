// ModeDisplayUIManager.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ModeDisplayUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text modeText;
    [SerializeField] private Image    modeIcon;
    [SerializeField] private Sprite   raceSprite;
    [SerializeField] private Sprite   survivalSprite;
    [SerializeField] private Sprite   puzzleSprite;

    void Start()
    {
        var mode = FirebaseAccountManager.Instance.sessionGameType;
        Debug.Log($"[Lobby] Loaded sessionGameType → {mode}");
        UpdateModeUI(mode);
    }

    public void UpdateModeUI(GameType mode)
    {
        modeText.text = mode.ToString();
        switch (mode)
        {
            case GameType.Race:     modeIcon.sprite = raceSprite;    break;
            case GameType.Survival: modeIcon.sprite = survivalSprite;break;
            case GameType.Puzzle:   modeIcon.sprite = puzzleSprite;  break;
        }
    }
}