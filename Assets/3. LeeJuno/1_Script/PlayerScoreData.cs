using System.Collections;
using System.Collections.Generic;
using Fusion;

public class PlayerScoreData
{
    //게임매니저 등수에 따라 점수를 기록 다음 스테이지에 가도 유지
    //12점 플레이어가 나오고 게임이 완전히 종료되면 초기화
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