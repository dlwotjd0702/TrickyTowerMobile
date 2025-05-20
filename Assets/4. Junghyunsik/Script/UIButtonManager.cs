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
    public GameObject leaveGameUI;
    
    [Header("Help Paging")]
    [Tooltip("Help 패널들 (Help1, Help2, Help3, Help4)")]
    public GameObject[] helpPages;         
    [Tooltip("상단 페이지 표시용 Image들 (panel1…panel4)")]
    public Image[] pageIndicators;          
    public Sprite indicatorActive;         
    public Sprite indicatorInactive;       
    private int helpPageIndex = 0;
    
    [Header("Player Paging")]
    [Tooltip("Player1~4 패널")]
    public GameObject[] playerPages;      
    public Button playerPrevBtn;
    public Button playerNextBtn;
    private int playerIndex = 0;

    [Header("Block Paging")]
    [Tooltip("Block1~4 패널")]
    public GameObject[] blockPages;       
    public Button blockPrevBtn;
    public Button blockNextBtn;
    private int blockIndex = 0;
    
    [Header("Mode Paging (Room Settings)")]
    [Tooltip("Race, Survival, Random 등 모드 패널들")]
    public GameObject[] modePages;     
    public Button modePrevBtn;         
    public Button modeNextBtn;         
    private int modeIndex = 0;

    [System.Serializable]
    public class Pager
    {
        public GameObject[] pages;    // Easy, Normal, Special, Random
        public Button prevButton;     
        public Button nextButton;     
        [HideInInspector] public int index;
    }

    [Header("Race Level Paging")]
    public Pager racePager;

    [Header("Survival Level Paging")]
    public Pager survivalPager;

    [Header("Puzzle Level Paging")]
    public Pager puzzlePager;

    [Header("Random Level Paging")]
    public Pager randomPager;
    
    [Header("Match Level Paging")]
    public Pager matchPager;
    
    [Header("페이드")]
    public Image blackOverlay;
    public float fadeTime = 0.5f;

    private bool isTransitioning = false;

    void Start()
    {
        // 버튼 클릭에 메소드 연결
        playerPrevBtn.onClick.AddListener(() => ShowPlayerPage(playerIndex - 1));
        playerNextBtn.onClick.AddListener(() => ShowPlayerPage(playerIndex + 1));

        blockPrevBtn.onClick.AddListener(() => ShowBlockPage(blockIndex - 1));
        blockNextBtn.onClick.AddListener(() => ShowBlockPage(blockIndex + 1));

        modePrevBtn.onClick.AddListener(() => ShowModePage(modeIndex - 1));
        modeNextBtn.onClick.AddListener(() => ShowModePage(modeIndex + 1));

        SetupPager(racePager);
        SetupPager(survivalPager);
        SetupPager(puzzlePager);
        SetupPager(randomPager);
        SetupPager(matchPager);
        
        // 초기화
        ShowPlayerPage(0);
        ShowBlockPage(0);
        ShowModePage(0);
    }
    
    private void ShowPlayerPage(int newIndex)
    {
        // 순환
        playerIndex = (newIndex + playerPages.Length) % playerPages.Length;
        for (int i = 0; i < playerPages.Length; i++)
            playerPages[i].SetActive(i == playerIndex);
    }

    private void ShowBlockPage(int newIndex)
    {
        blockIndex = (newIndex + blockPages.Length) % blockPages.Length;
        for (int i = 0; i < blockPages.Length; i++)
            blockPages[i].SetActive(i == blockIndex);
    }
    
    private void ShowModePage(int newIndex)
    {
        // 순환 인덱스 계산
        modeIndex = (newIndex + modePages.Length) % modePages.Length;

        // 활성화/비활성화
        for (int i = 0; i < modePages.Length; i++)
            modePages[i].SetActive(i == modeIndex);
    }
    
    void SetupPager(Pager pager)
    {
        // 버튼 클립 연결
        pager.prevButton.onClick.AddListener(() => ShowPage(pager, pager.index - 1));
        pager.nextButton.onClick.AddListener(() => ShowPage(pager, pager.index + 1));
        // 초기 페이지
        ShowPage(pager, 0);
    }
    
    void ShowPage(Pager pager, int newIndex)
    {
        pager.index = (newIndex + pager.pages.Length) % pager.pages.Length;
        for (int i = 0; i < pager.pages.Length; i++)
            pager.pages[i].SetActive(i == pager.index);
    }
    
    
    
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
                // Help 열 때 항상 첫 페이지로
                helpPageIndex = 0;
                RefreshHelpPages();
                mainUI.SetActive(false);
                helpUI.SetActive(true);
                break;

            case "HelpNext":
                helpPageIndex = (helpPageIndex + 1) % helpPages.Length;
                RefreshHelpPages();
                break;
            
            case "HelpPrev":
                helpPageIndex = (helpPageIndex - 1 + helpPages.Length) % helpPages.Length;
                RefreshHelpPages();
                break;
            
            case "Back":
                helpUI.SetActive(false);
                optionUI.SetActive(false);
                playSetUI.SetActive(false);
                leaveGameUI.SetActive(false);
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

            case "BackToPlaySet":
                modSetUI.SetActive(false);
                readyRoomUI.SetActive(false);
                playSetUI.SetActive(true);
                break;

            case "CreateRoom":
                modSetUI.SetActive(false);
                readyRoomUI.SetActive(true);
                break;
            
            case "EnterRoom":
                playSetUI.SetActive(false);
                readyRoomUI.SetActive(true);
                break;
            
            // 게임 나가기 관련
            
            case "ShowLeaveGame":
                leaveGameUI.SetActive(true);
                break;
            
            case "LeaveGameNo":
                leaveGameUI.SetActive(false);
                break;
            
            case "LeaveGameYes":
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                break;
        }
    }

    private void RefreshHelpPages()
    {
        // 1) 각 Help 패널 활성/비활성
        for (int i = 0; i < helpPages.Length; i++)
            helpPages[i].SetActive(i == helpPageIndex);

        // 2) 상단 인디케이터 스프라이트 교체
        for (int i = 0; i < pageIndicators.Length; i++)
            pageIndicators[i].sprite = (i == helpPageIndex) 
                ? indicatorActive 
                : indicatorInactive;
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