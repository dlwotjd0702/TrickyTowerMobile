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

    private int roundIndex = 0;
    private bool gameActive = true;
    private bool isInvinsible = false;
    private PlayType playType;

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
        scoreBoard.ResetSlots();

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
        if (gameActive == false) return;

        var ranking = GameClearManager.Instance.GetLastRoundRanking();
        //** 플레이어 점수UI 띄우기**
        scoreBoard.FillMedals(ranking);

        int winnerScore = GameClearManager.Instance.GetPlayerScore(winner);
        if (winnerScore >= cupTargetScore || playType == PlayType.Selection)
        {
            GameClear();
        }
        else
        {
            roundIndex = (roundIndex + 1);
            if (roundIndex >= 3) roundIndex = 0;
            Invoke(nameof(NextRound), 5f);
        }
    }

    private void NextRound()
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