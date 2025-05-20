using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;

public class SurvivalModeManager : NetworkBehaviour
{
    private Dictionary<PlayerRef, SurvivePlayerData> playerStates = new();

    public void RegisterPlayer(PlayerRef player)
    {
        if (!playerStates.ContainsKey(player))
            playerStates[player] = new SurvivePlayerData(player);
    }

    public void OnBlockSpawned(PlayerRef player)
    {
        playerStates[player].OnBlockSpawned();
        CheckGameOver();
    }

    public void OnBlockDestroyed(PlayerRef player)
    {
        playerStates[player].OnBlockDestroyed(Time.time);
        CheckGameOver();
    }

    private void CheckGameOver()
    {
        var alive = playerStates.Values.Where(p => !p.IsDead).ToList();
        if (alive.Count == 0 || alive.Any(p => p.BrickCount <= 0))
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        var ranked = playerStates
            .OrderByDescending(p => p.Value.BrickCount)
            .Select(p => p.Key)
            .ToArray();

        GameClearManager.Instance.SurvivalModeClear(ranked[0]);
        // 등수 정보 활용해 점수 부여 등
    }
}