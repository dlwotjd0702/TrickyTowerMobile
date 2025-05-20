using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class GameClearManager : MonoBehaviour
{
    //게임종료와 종료후 모드별로 플레이어들의 등수를 판정
    //등수를 판정하기 위해 블럭이 각자 자신의 주인이 누구인지 데이터를 가지고 있어야 함 
    public static GameClearManager Instance;
    private PlayerScoreData scoreData = new PlayerScoreData();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void RaceModeClear(PlayerRef winner)
    {
        Debug.Log("clear");
        //레이스모드 종료후 1,2,3,4등 판정
        IEnumerable<NetworkObject> allBlocks = GameObject.FindGameObjectsWithTag("Block")
            .Select(go => go.GetComponent<NetworkObject>()) //변환
            .Where(no => no != null); //필터링

        //익명 타입 var
        var playerHeights = allBlocks
            .GroupBy(no => no.InputAuthority) //그룹묶기
            .Where(group => group.Key != winner)
            .Select(group => new
            {
                Player = group.Key,
                MaxHeight = group.Max(no => no.transform.position.y)
            });

        PlayerRef[] ranking = new[] { winner }
            .Concat(playerHeights.OrderByDescending
                    (ph => ph.MaxHeight)
                .Select(ph => ph.Player))
            .ToArray();

        AssignScore(ranking);
    }

    private void AssignScore(PlayerRef[] ranking)
    {
        for (int i = 0; i < ranking.Length; i++)
        {
            int score
                = i == 0 ? 3
                : i == 1 ? 2
                : i == 2 ? 1
                : 0;
            scoreData.AddScore(ranking[i], score);
        }
    }

    public Dictionary<PlayerRef, int> GetAllScore()
    {
        return scoreData.GetAllScore();
    }

    public int GetPlayerScore(PlayerRef player)
    {
        return scoreData.GetScore(player);
    }

    public void ResetScore()
    {
        scoreData.ResetScore();
    }
}