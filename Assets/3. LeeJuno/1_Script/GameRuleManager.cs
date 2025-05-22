using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fusion;

public enum PlayType
{
    Selction,
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
    private GameObject[] gameRules = new GameObject[3];

    [SerializeField]
    private int cupTargetScore = 12;

    private int roundIndex = 0;
    private bool gameActive = true;

    public override void Spawned()
    {
        if (Runner.IsServer == false) return;
        GameClearManager.Instance.RoundCleared += RoundCleared;
    }

    private void OnDestroy()
    {
        if (Runner.IsServer)
            GameClearManager.Instance.RoundCleared -= RoundCleared;
    }

    public void StartGame()
    {
        GameClearManager.Instance.ResetScore();
        gameActive = true;

        roundIndex = 0;
        ActiveMode((GameType)roundIndex);
    }

    private void ActiveMode(GameType type)
    {
        foreach (var obj in gameRules)
        {
            obj.SetActive(false);
        }

        gameRules[(int)type].SetActive(true);
    }

    private void RoundCleared(PlayerRef winer, GameType gameType)
    {
        if (gameActive == false) return;
        
        var score = GameClearManager.Instance.GetAllScore();
        //** 플레이어 점수UI 띄우기**
        
        int winnerScore = GameClearManager.Instance.GetPlayerScore(winer);
        if (winnerScore >= cupTargetScore)
        {
            GameClear();
        }
        else
        {
            roundIndex = (roundIndex + 1) % gameRules.Length;
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