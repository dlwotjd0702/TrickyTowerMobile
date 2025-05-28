using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fusion;

public enum PlayType
{
    Selection,
    Cup
}

public enum GameType
{
    Race = 0,
    Survival = 1,
    Puzzle = 2,
}

public class GameRuleManager : NetworkBehaviour
{
    [SerializeField]
    private GameObject raceMode;

    [SerializeField]
    private GameObject survivalMode;

    [SerializeField]
    private GameObject puzzleMode;

    [SerializeField]
    private int cupTargetScore = 12;

    [SerializeField]
    private ScoreBoard scoreBoard;

    private NetworkManager networkManager;
    private int roundIndex = 0;
    private bool gameActive = true;
    private bool isInvinsible = false;
    private PlayType playType;
    
    // 몇 번 이겼는지 기록할 맵
    private Dictionary<PlayerRef, int> winCounts = new Dictionary<PlayerRef, int>();

// 최종 승리 조건
    [SerializeField] private int winsToVictory = 3;

    public override void Spawned()
    {
        if (Runner.IsServer == false) return;
        GameClearManager.Instance.RoundCleared += RoundCleared;
    }

    private void OnDestroy()
    {
        if (Runner.IsServer == false) return;
        GameClearManager.Instance.RoundCleared -= RoundCleared;
    }

    public void StartCupGame(GameType type)
    {
        playType = PlayType.Cup;
        GameClearManager.Instance.ResetScore();
        //scoreBoard.ResetSlotsLocal();

        gameActive = true;
        roundIndex = (int)type;
        ActiveMode(type);
    }

    public void StartSelectGame(GameType type)
    {
        playType = PlayType.Selection;
        ActiveMode(type);
    }

    private void ActiveMode(GameType type)
    {
        switch (type)
        {
            case GameType.Race:
                raceMode.SetActive(true);

                survivalMode.SetActive(false);
                puzzleMode.SetActive(false);
                break;
            case GameType.Survival:
                survivalMode.SetActive(true);

                puzzleMode.SetActive(false);
                raceMode.SetActive(false);
                break;
            case GameType.Puzzle:
                puzzleMode.SetActive(true);

                survivalMode.SetActive(false);
                raceMode.SetActive(false);
                break;
        }
    }

    private void RoundCleared(PlayerRef winner, GameType gameType)
    {
        Debug.Log("라운드끝");
        if (Runner.IsServer == false || gameActive == false) return;
        Debug.Log("다음라운드");
        
        //** 플레이어 점수UI 띄우기**
        scoreBoard.ShowScoreBoard();
        ////각자의 스코어보드 제작 및 네트워크 트랜스폼부착, 3초뒤 보드가 꺼지도록 설정
        Debug.Log("1");
       
        int winnerScore = GameClearManager.Instance.GetPlayerScore(winner);

        GameClearManager.Instance.RemoveAllBlocks();
        GameClearManager.Instance.AllowAllBlocks();
        GameClearManager.Instance.ClearPlayers();
        GameClearManager.Instance.ClearFalse();
        
        Debug.Log("2");
    
        if (winnerScore >= cupTargetScore || playType == PlayType.Selection)
        {
            GameClear();
        }
        else
        {
            roundIndex = (roundIndex + 1);
            if (roundIndex >= 3) roundIndex = 0;
            networkManager = FindObjectOfType<NetworkManager>();
            networkManager.GameClear(winner);
        }
    }
    
    public void NextRound()
    {
        ActiveMode((GameType)roundIndex);
    }

    private void GameClear()
    {
        gameActive = false;

        var finalScore =
            GameClearManager.Instance.GetAllScore()
                .OrderByDescending(kv => kv.Value)
                .Select(kv => kv.Key)
                .ToArray();
        //** 최종 점수UI 띄우기 **
    }
}