using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject roomListUI;
    
    MaterialSwitcher matSwitcher;
    
    [Header("Quad 머티리얼 스위처")]
    [SerializeField] MaterialSwitcher materialSwitcher;

    [Header("배경 스위처")]
    [SerializeField] ModeBackgroundSwitcher backgroundSwitcher;
    
    [Header("UI")]
    public GameObject introUI;
    public GameObject mainUI;
    public GameObject optionUI;
    public GameObject helpUI;
    public GameObject playSetUI;
    public GameObject modSetUI;
    public GameObject signupUI;
    public GameObject eixtGameUI;
    
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
    
    //
    [Header("PlaySet Preview Containers")]
    [Tooltip("PlaySet/PlayerSettings/PlayerSelect 의 부모 (Player1~4)")]
    public Transform playSetPlayerContainer;
    [Tooltip("PlaySet/PlayerSettings/BlockSelect 의 부모 (Block1~4)")]
    public Transform playSetBlockContainer;
    
    [Header("페이드")]
    public Image blackOverlay;
    public float fadeTime = 0.5f;

    private bool isTransitioning = false;
    
    private int savedPlayerIndex = 0;
    private int savedBlockIndex  = 0;
    
    public ModeBackgroundSwitcher BackgroundSwitcher;
    
    void Awake()
    {
        // 씬에 하나만 있다면 FindObjectOfType로도 OK
        matSwitcher = FindObjectOfType<MaterialSwitcher>();
    }
    
    void Start()
    {
        // 페이징 버튼 리스너
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
        
        // 초기 페이지
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
        
        // Quad1 머테리얼 스위치
        FindObjectOfType<ModeBackgroundSwitcher>()
            .SetBackground(modeIndex);
        BackgroundSwitcher.SetBackground(modeIndex);
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
                mainUI.SetActive(true);
                break;

            case "MultiPlay":
                mainUI.SetActive(false);
                playSetUI.SetActive(true); 
                break;
            
            //
            case "MakeRoom":
                // 1) 선택 저장
                savedPlayerIndex = playerIndex;
                savedBlockIndex  = blockIndex;
                // 2) 바로 OFF→ON 토글
                TogglePlaySetSelection();
                // 3) 2초 후 PlaySet → ModSet
                StartCoroutine(DelayedSwitch(playSetUI, modSetUI));
                break;
            
            case "BackToPlaySet":
                modSetUI.SetActive(false);
                roomListUI.SetActive(false);
                playSetUI.SetActive(true);

                // ▶ PlaySet Preview 컨테이너를 OFF 상태로 복원
                playersOnContainer.gameObject.SetActive(false);
                blocksOnContainer .gameObject.SetActive(false);
                playersOffContainer.gameObject.SetActive(true);
                blocksOffContainer .gameObject.SetActive(true);
                // ▶ 페이징 인덱스도 0으로 리셋
                playerIndex = 0;
                blockIndex  = 0;
                ShowPlayerPage(0);
                ShowBlockPage(0);
                break;

            case "CreateRoom":
                SceneManager.LoadScene("lobby");
                break;
            
            case "EnterRoom":
                SceneManager.LoadScene("lobby");
                break;
            
            case "EnterRoomList":
                // 1) 인덱스 저장
                savedPlayerIndex = playerIndex;
                savedBlockIndex  = blockIndex;
                // 2) PlaySet UI 위에서 바로 OFF→ON 자식 토글
                TogglePlaySetSelection();
                // 3) 2초 뒤 전환 코루틴
                StartCoroutine(TransitionAfterDelay(playSetUI, roomListUI));
                break;
            
            case "Signup":
                signupUI.SetActive(true);
                break;
            
            case "SignupBack":
                signupUI.SetActive(false);
                break;
            
            // 게임 나가기 관련
            
            case "ShowLeaveGame":
                eixtGameUI.SetActive(true);
                break;
            
            case "LeaveGameNo":
                eixtGameUI.SetActive(false);
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
    
    //
    // PlaySet 위에서 OFF → ON 자식 토글
    private void PreviewPlaySetSelection()
    {
        // Player1~4 밑 OFF/ON
        for (int i = 0; i < playSetPlayerContainer.childCount; i++)
        {
            var p = playSetPlayerContainer.GetChild(i);
            var off = p.Find("OFF");
            var on  = p.Find("ON");
            if (off != null) off.gameObject.SetActive(false);
            if (on  != null) on .gameObject.SetActive(i == savedPlayerIndex);
        }

        // Block1~4 밑 OFF/ON
        for (int i = 0; i < playSetBlockContainer.childCount; i++)
        {
            var b = playSetBlockContainer.GetChild(i);
            var off = b.Find("OFF");
            var on  = b.Find("ON");
            if (off != null) off.gameObject.SetActive(false);
            if (on  != null) on .gameObject.SetActive(i == savedBlockIndex);
        }
    }
    
    // PlaySet Preview Containers
// (Inspector 에 바인딩 해 놓은 RectTransform 필드들)
    public RectTransform playersOffContainer;
    public RectTransform playersOnContainer;
    public RectTransform blocksOffContainer;
    public RectTransform blocksOnContainer;

    public void OnModeSelected(int modeIndex)
    {
        // 오로지 BackGround RawImage 머티리얼만 바꿉니다.
        matSwitcher.SetModeMaterial(modeIndex);
    }
    
    private void TogglePlaySetSelection()
    {
        // OFF 컨테이너는 꺼주고…
        playersOffContainer.gameObject.SetActive(false);
        blocksOffContainer .gameObject.SetActive(false);
        // ON 컨테이너는 켜 줍니다.
        playersOnContainer .gameObject.SetActive(true);
        blocksOnContainer .gameObject.SetActive(true);
    
        // 만약 LightImage가 항상 맨 앞(Child 0)에 들어온다면…
        for(int i = 0; i < playersOnContainer.childCount - 1; i++)
        {
            // 실제 플레이어 오브젝트는 GetChild(i+1)
            var playerGO = playersOnContainer.GetChild(i + 1).gameObject;
            playerGO.SetActive(i == savedPlayerIndex);
        }
        
        // PlaySet BlockON 자식 중 savedBlockIndex만 켜기 (LightImage 제외)
        for (int i = 0; i < blocksOnContainer.childCount - 1; i++)
        {
            // 실제 Block 오브젝트는 GetChild(i+1)
            var blockGO = blocksOnContainer.GetChild(i + 1).gameObject;
            blockGO.SetActive(i == savedBlockIndex);
        }
        
    }


// 2) 2초 딜레이 후 전환하는 코루틴 추가
    private IEnumerator DelayedSwitch(GameObject from, GameObject to)
    {
        // (원한다면) 디버그 로그
        Debug.Log($"[UI] Switching from {from.name} to {to.name} after 2s");
        yield return new WaitForSeconds(2f);
        from.SetActive(false);
        to  .SetActive(true);
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
    
    // 1) UIButtonManager 클래스 맨 아래에, 기존 FadeToMainUI 아래에 추가
    private IEnumerator TransitionAfterDelay(GameObject from, GameObject to)
    {
        // 디버깅 로그: 저장된 인덱스 확인
        Debug.Log($"[Transition] savedPlayerIndex={savedPlayerIndex}, savedBlockIndex={savedBlockIndex}");

        // PlaySet 위 Preview
        PreviewPlaySetSelection();
        Debug.Log("[Transition] PreviewPlaySetSelection called, waiting 2 seconds...");

        // 2초 대기
        yield return new WaitForSeconds(2f);

        Debug.Log($"[Transition] Switching UI: {from.name} → {to.name}");
        from.SetActive(false);
        to.SetActive(true);
    }

    
}