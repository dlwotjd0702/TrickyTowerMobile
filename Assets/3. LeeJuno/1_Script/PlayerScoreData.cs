using System.Collections;
using System.Collections.Generic;
using Fusion;

public enum MedalType
{
    None,
    Bronze,
    Silver,
    Gold
}

public class PlayerScoreData
{
    private Dictionary<PlayerRef, int> playerScore = new();
    private Dictionary<PlayerRef, List<MedalType>> playerMedals = new();

    public void AddScore(PlayerRef player, int score)
    {
        if (!playerScore.ContainsKey(player))
            playerScore[player] = 0;

        playerScore[player] += score;
    }

    public void AddMedal(PlayerRef player, MedalType medal)
    {
        if (!playerMedals.ContainsKey(player))
            playerMedals[player] = new List<MedalType>();

        playerMedals[player].Add(medal);
    }

    public List<MedalType> GetMedals(PlayerRef player)
    {
        return playerMedals.TryGetValue(player, out var list) ? new List<MedalType>(list) : new List<MedalType>();
    }

    public Dictionary<PlayerRef, List<MedalType>> GetAllMedals()
    {
        // 깊은 복사하여 반환
        var result = new Dictionary<PlayerRef, List<MedalType>>();
        foreach (var kv in playerMedals)
            result[kv.Key] = new List<MedalType>(kv.Value);
        return result;
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
        playerMedals.Clear();
    }
}