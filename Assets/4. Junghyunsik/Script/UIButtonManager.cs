using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIButtonManager : MonoBehaviour
{
    [Header("RoomList UI")]
    [SerializeField] private GameObject roomListUI;

    [Header("Background Switchers")]
    [SerializeField] private BackgroundMaterialSwitcher backgroundMaterialSwitcher;
    [SerializeField] private ModeBackgroundUIManager   modeBackgroundUIManager;

    [Header("Main UI Panels")]
    public GameObject introUI;
    public GameObject mainUI;
    public GameObject optionUI;
    public GameObject helpUI;
    public GameObject playSetUI;
    public GameObject modSetUI;
    public GameObject signupUI;
    public GameObject exitGameUI;
    public GameObject reenterUI;
    public GameObject successSignupUI;
    
    [Header("Help Paging")]
    public GameObject[] helpPages;
    public Image[]     pageIndicators;
    public Sprite      indicatorActive;
    public Sprite      indicatorInactive;
    private int        helpPageIndex = 0;

    [Header("Player Paging")]
    public GameObject[] playerPages;
    public Button       playerPrevBtn;
    public Button       playerNextBtn;
    private int         playerIndex = 0;

    [Header("Block Paging")]
    public GameObject[] blockPages;
    public Button       blockPrevBtn;
    public Button       blockNextBtn;
    private int         blockIndex = 0;

    [Header("Mode Paging (Room Settings)")]
    public GameObject[] modePages;
    public Button       modePrevBtn;
    public Button       modeNextBtn;
    private int         modeIndex = 0;

    [System.Serializable]
    public class Pager
    {
        public GameObject[] pages;
        public Button       prevButton;
        public Button       nextButton;
        [HideInInspector] public int index;
    }
    [Header("Race Level Paging")]    public Pager racePager;
    [Header("Survival Level Paging")]public Pager survivalPager;
    [Header("Puzzle Level Paging")]  public Pager puzzlePager;
    [Header("Random Level Paging")]  public Pager randomPager;
    [Header("Match Level Paging")]   public Pager matchPager;

    [Header("PlaySet Preview Containers")]
    public RectTransform playersOffContainer;
    public RectTransform playersOnContainer;
    public RectTransform blocksOffContainer;
    public RectTransform blocksOnContainer;
    private int savedPlayerIndex = 0;
    private int savedBlockIndex  = 0;

    [Header("Fade Overlay")]
    public Image  blackOverlay;
    public float  fadeTime = 0.5f;
    private bool  isTransitioning = false;

    void Start()
    {
        // 페이징 버튼 연결
        playerPrevBtn.onClick.AddListener(() => ShowPlayerPage(playerIndex - 1));
        playerNextBtn.onClick.AddListener(() => ShowPlayerPage(playerIndex + 1));
        blockPrevBtn.onClick.AddListener(() => ShowBlockPage(blockIndex - 1));
        blockNextBtn.onClick.AddListener(() => ShowBlockPage(blockIndex + 1));
        modePrevBtn.onClick .AddListener(() => ShowModePage(modeIndex - 1));
        modeNextBtn.onClick .AddListener(() => ShowModePage(modeIndex + 1));

        // 레벨 페이저들 초기화
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

        // UI 페이징
        for (int i = 0; i < modePages.Length; i++)
            modePages[i].SetActive(i == modeIndex);

        // **여기서 머티리얼 & 배경 UI 전환 호출**
        if (backgroundMaterialSwitcher != null)
            backgroundMaterialSwitcher.SetModeMaterial(modeIndex);

        if (modeBackgroundUIManager != null)
            modeBackgroundUIManager.SetMode(modeIndex);
    }

    private void SetupPager(Pager pager)
    {
        pager.prevButton.onClick.AddListener(() => ShowPage(pager, pager.index - 1));
        pager.nextButton.onClick.AddListener(() => ShowPage(pager, pager.index + 1));
        ShowPage(pager, 0);
    }

    private void ShowPage(Pager pager, int newIndex)
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

            case "MakeRoom":
                savedPlayerIndex = playerIndex;
                savedBlockIndex  = blockIndex;
                TogglePlaySetSelection();
                StartCoroutine(FadeSwitch(playSetUI, modSetUI));
                break;

            case "BackToPlaySet":
                modSetUI.SetActive(false);
                roomListUI.SetActive(false);
                playSetUI.SetActive(true);
                // PlaySet Preview 복원
                playersOnContainer .gameObject.SetActive(false);
                blocksOnContainer  .gameObject.SetActive(false);
                playersOffContainer.gameObject.SetActive(true);
                blocksOffContainer .gameObject.SetActive(true);
                playerIndex = 0;
                blockIndex  = 0;
                ShowPlayerPage(0);
                ShowBlockPage(0);
                break;

            case "CreateRoom":
            case "EnterRoom":
                DOTween.KillAll();
                SceneManager.LoadScene("lobby");
                break;

            case "EnterRoomList":
                savedPlayerIndex = playerIndex;
                savedBlockIndex  = blockIndex;
                TogglePlaySetSelection();
                StartCoroutine(FadeSwitch(playSetUI, roomListUI));
                break;

            case "Signup":
                signupUI.SetActive(true);
                break;

            case "SignupreenterUI":
                reenterUI.SetActive(false);
                break;
            
            case "successSignupUI":
                successSignupUI.SetActive(false);
                break;
            
            case "SignupBack":
                signupUI.SetActive(false);
                break;

            case "ShowLeaveGame":
                exitGameUI.SetActive(true);
                break;

            case "LeaveGameNo":
                exitGameUI.SetActive(false);
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
        for (int i = 0; i < helpPages.Length; i++)
            helpPages[i].SetActive(i == helpPageIndex);

        for (int i = 0; i < pageIndicators.Length; i++)
            pageIndicators[i].sprite = 
              (i == helpPageIndex) ? indicatorActive : indicatorInactive;
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

    private IEnumerator FadeSwitch(GameObject from, GameObject to)
    {
        // 1) 전환 잠금
        isTransitioning = true;

        // 2) 2초 대기 (키·클릭 모두 무시)
        yield return new WaitForSeconds(2f);

        // 3) 페이드 아웃 준비: blackOverlay 켜고 RaycastTarget 활성화
        blackOverlay.gameObject.SetActive(true);
        blackOverlay.raycastTarget = true;    // <-- 여기서 마우스도 차단합니다

        Color c = blackOverlay.color;
        c.a = 0f;
        blackOverlay.color = c;

        // 4) 페이드 아웃
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / fadeTime);
            blackOverlay.color = c;
            yield return null;
        }

        // 5) UI 전환
        if (from != null) from.SetActive(false);
        if (to   != null) to  .SetActive(true);

        // 6) 페이드 인
        t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / fadeTime);
            blackOverlay.color = c;
            yield return null;
        }

        // 7) 페이드 끝나면 RaycastTarget 비활성화 후 overlay 끄기
        blackOverlay.raycastTarget = false;
        blackOverlay.gameObject.SetActive(false);

        // 8) 전환 해제
        isTransitioning = false;
    }

    private void TogglePlaySetSelection()
    {
        playersOffContainer.gameObject.SetActive(false);
        blocksOffContainer .gameObject.SetActive(false);
        playersOnContainer .gameObject.SetActive(true);
        blocksOnContainer  .gameObject.SetActive(true);

        for (int i = 0; i < playersOnContainer.childCount - 1; i++)
        {
            var go = playersOnContainer.GetChild(i + 1).gameObject;
            go.SetActive(i == savedPlayerIndex);
        }
        for (int i = 0; i < blocksOnContainer.childCount - 1; i++)
        {
            var go = blocksOnContainer.GetChild(i + 1).gameObject;
            go.SetActive(i == savedBlockIndex);
        }
    }
}