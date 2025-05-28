using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomSettingsUI : MonoBehaviour
{
    [Header("Mode Selection")]
    [SerializeField] private GameObject[] modeImages; // 0:Race,1:Survival,2:Puzzle
    [SerializeField] private Button leftBtn;
    [SerializeField] private Button rightBtn;

    [Header("Room Create")]
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private Button         createButton;

    [Header("Network")]
    [SerializeField] private NetworkManager networkManager; // int gameType, sessionName

    private int currentIndex = 0;

    private void Awake()
    {
        leftBtn.onClick.AddListener(SelectPreviousMode);
        rightBtn.onClick.AddListener(SelectNextMode);
        createButton.onClick.AddListener(OnCreateClicked);
    }

    private void Start()
    {
        UpdateModeDisplay();
    }

    private void SelectPreviousMode()
    {
        currentIndex = (currentIndex - 1 + modeImages.Length) % modeImages.Length;
        UpdateModeDisplay();
    }

    private void SelectNextMode()
    {
        currentIndex = (currentIndex + 1) % modeImages.Length;
        UpdateModeDisplay();
    }

    private void UpdateModeDisplay()
    {
        for (int i = 0; i < modeImages.Length; i++)
            modeImages[i].SetActive(i == currentIndex);
    }

    private void OnCreateClicked()
    {
        var roomName = roomNameInput.text.Trim();
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogWarning("방 이름을 입력해주세요!");
            return;
        }

        // currentIndex를 GameType으로 캐스트해서 전달
        networkManager.gameType    = (GameType)currentIndex;
        networkManager.sessionName = roomName;

        Debug.Log($"[RoomSettingsUI] CreateRoom name={roomName}, gameType={(GameType)currentIndex}");
        
    }
}