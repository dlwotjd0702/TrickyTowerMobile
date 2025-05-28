using UnityEngine;
using UnityEngine.UI;
using TMPro; // if you use TMP_InputField

public class RoomSettingsUI : MonoBehaviour
{
    [Header("Mode Selection")]
    [Tooltip("Race, Survival, Puzzle 순서대로 할당")]
    [SerializeField] private GameObject[] modeImages;
    [SerializeField] private Button     leftBtn;
    [SerializeField] private Button     rightBtn;

    [Header("Room Create")]
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private Button         createButton;

    private int currentIndex = 0;

    private void Awake()
    {
        leftBtn.onClick.AddListener(SelectPreviousMode);
        rightBtn.onClick.AddListener(SelectNextMode);
        createButton.onClick.AddListener(OnCreateClicked);
    }

    private void Start()
    {
        // 처음에 Race만 보이도록
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
        // 선택된 모드 이미지만 활성화
        for (int i = 0; i < modeImages.Length; i++)
            modeImages[i].SetActive(i == currentIndex);

        // GameType enum 순서와 인덱스가 0:Race, 1:Survival, 2:Puzzle 이라고 가정
        FirebaseAccountManager.Instance.sessionGameType = (GameType)currentIndex;
        Debug.Log($"[RoomSettingsUI] mode → {FirebaseAccountManager.Instance.sessionGameType}");
        
        var mode = (GameType)currentIndex;
        FirebaseAccountManager.Instance.sessionGameType = mode;
        Debug.Log($"[RoomSettingsUI] Selected mode → {mode}");
        
    }

    private void OnCreateClicked()
    {
        var roomName = roomNameInput.text.Trim();
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogWarning("방 이름을 입력해주세요!");
            return;
        }
        FirebaseAccountManager.Instance.CreateRoom(roomName);
    }
}