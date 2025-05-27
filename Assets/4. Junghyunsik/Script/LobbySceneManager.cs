/*using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LobbySceneManager : MonoBehaviour
{
    public string OutGame = "OutGame";

    public void OnExitLobby()
    {
        // 로비 세션 종료 로직 …
        // 씬 전환 직전에 씬 로드 콜백 등록
        SceneManager.sceneLoaded += OnOutGameSceneLoaded;
        DOTween.KillAll();
        SceneManager.LoadScene(OutGame);
    }

    private void OnOutGameSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != OutGame) return;

        // 이제 새 씬의 모든 Awake/Start가 끝난 뒤이므로 안전하게 찾아서 토글 가능
        var ui = FindObjectOfType<UIButtonManager>();
        if (ui != null)
        {
            ui.introUI.SetActive(false);
            ui.playSetUI.SetActive(true);
        }

        // 한 번만 실행하고 이벤트 해제
        SceneManager.sceneLoaded -= OnOutGameSceneLoaded;
    }
}*/