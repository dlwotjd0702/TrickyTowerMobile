using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public enum GameType
{
    Race,
    Puzzle,
    Survival,
}

public class GameClearManager : NetworkBehaviour
{
    //게임종료와 종료후 모드별로 플레이어들의 등수를 판정
    //등수를 판정하기 위해 블럭이 각자 자신의 주인이 누구인지 데이터를 가지고 있어야 함 
    public static GameClearManager Instance;
    private PlayerScoreData scoreData = new PlayerScoreData();
    private HashSet<PlayerRef> failedPlayers = new HashSet<PlayerRef>();
    private GameType CurrentGameType;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void SetGameType(GameType mode)//** 현재 모드 설정 **
    {
        CurrentGameType = mode;
    }

    public void RaceModeClear(PlayerRef winner) //레이스모드 관련로직
    {
        Debug.Log("clearRace");
        //** 모든플레이어 블럭생성막힘 추가 필요 **

        //레이스모드 종료후 1,2,3,4등 판정
        IEnumerable<NetworkObject> allBlocks = GameObject.FindObjectsOfType<NetworkObject>()
            .Where(no => no.gameObject.layer == LayerMask.NameToLayer("Block"))
            .Where(no => no.TryGetComponent<NetworkBlockController>(out var controller) && controller.IsPlaced);

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
            .Concat(playerHeights.OrderByDescending(ph => ph.MaxHeight)
                .Select(ph => ph.Player))
            .ToArray();

        AssignScore(ranking);
    }

    public void PuzzleModeClear() //퍼즐 모드 관련 로직
    {
        Debug.Log("Puzzle clear");
        //그후에 남은 벽돌 개수를 세서 많은순으로 1,2,3,4 판정

        IEnumerable<NetworkObject> allBlocks = GameObject.FindObjectsOfType<NetworkObject>()
            .Where(no => no.gameObject.layer == LayerMask.NameToLayer("Block"))
            .Where(no => no.TryGetComponent<NetworkBlockController>(out var controller) && controller.IsPlaced);

        var blockCounts = allBlocks
            .GroupBy(no => no.InputAuthority)
            .Select(group => new
            {
                Player = group.Key,
                Count = group.Count()
            });

        PlayerRef[] ranking = blockCounts
            .OrderByDescending(entry => entry.Count)
            .Select(entry => entry.Player)
            .ToArray();

        AssignScore(ranking);
    }

    public void SurvivalModeClear() //** 서바이벌 모드 관련 로직 **
    {
        Debug.Log("Survival clear");
    }
    
    public void PlayerFailed(PlayerRef player) //플레이어 탈락 감지 로직
    {
        if (failedPlayers.Contains(player)) return;

        failedPlayers.Add(player);
        //** 해당 플레이어 블럭생성중지 **
        Debug.Log($"{player} 탈락");

        if (failedPlayers.Count >= Runner.ActivePlayers.Count()) //모든 플레이어 종료확인
        {
            //현재 모드에 맞는 클리어 로직(퍼즐, 서바이벌)
            switch (CurrentGameType)
            {
                case GameType.Puzzle:
                    PuzzleModeClear();
                    break;
                case GameType.Survival:
                    SurvivalModeClear();
                    break;
            }
        }
    }
    
    //----------이 밑에는 점수 관련 로직-------------
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