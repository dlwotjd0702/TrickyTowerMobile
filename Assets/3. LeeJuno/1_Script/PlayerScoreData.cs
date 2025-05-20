using System.Collections;
using System.Collections.Generic;
using Fusion;

public class PlayerScoreData
{
   
    private Dictionary<PlayerRef, int> playerScore = new Dictionary<PlayerRef, int>();

    public void AddScore(PlayerRef player, int score)
    {
        if (playerScore.ContainsKey(player) == false)
            playerScore[player] = 0;

        playerScore[player] += score;
    }

    public int GetScore(PlayerRef player)
    {
        return playerScore.TryGetValue(player, out var score) ? score : 0;
    }

    public Dictionary<PlayerRef, int> GetAllScore()
    {
        return new Dictionary<PlayerRef, int>(playerScore);
    }

    public void ResetScore()
    {
        playerScore.Clear();
    }
}