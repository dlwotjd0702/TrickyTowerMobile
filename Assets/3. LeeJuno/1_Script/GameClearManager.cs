using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

//추가해야할 사항 
// 1. 서바이벌 블럭과 hp 카운트 이벤트 블럭스포너에 연결 V
// 2. 블럭생성기에 플레이어 블럭생성을 막는 로직추가 V
// 지정플레이어 막는기능, 모든플레이어 막는기능, V
// 게임오버시에 배치안된 블럭제거하는 기능 V 
// 지정플레이어 제거, 모든 플레이어제거 V
// 3. 서바이벌 모드 판정 쌓인 블럭갯수에서 남은블럭 갯수로 변경
// 4. 게임끝나면 블럭 전부 삭제 V

public class GameClearManager : NetworkBehaviour
{
    //게임종료와 종료후 모드별로 플레이어들의 등수를 판정
    //등수를 판정하기 위해 블럭이 각자 자신의 주인이 누구인지 데이터를 가지고 있어야 함
    public static GameClearManager Instance;

    public event Action<PlayerRef, GameType> RoundCleared;

    private PlayerScoreData scoreData = new PlayerScoreData();
    private HashSet<PlayerRef> failedPlayers = new HashSet<PlayerRef>();
    private PlayerRef[] lastRoundRank;
    private GameType currentGameType;
    public bool clear { get; private set; }
    private bool stopAllSpawns = false;
    private HashSet<PlayerRef> blockedPlayers = new HashSet<PlayerRef>();
    public Vector3 MaxHeight;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(gameObject);
    }

    //-------------블럭 소환 차단 관련로직--------------
    public void StopAllSpawns() => stopAllSpawns = true; //모두 차단
    public void AllowAllBlocks() => stopAllSpawns = false; //모두 허용

    public void StopPlayer(PlayerRef p) => blockedPlayers.Add(p); //지정 차단
    public void UnblockPlayer(PlayerRef p) => blockedPlayers.Remove(p); //지정 차단해제
    public void ClearPlayers() => blockedPlayers.Clear(); //모든 지정 차단해제

    public bool CanSpawn(PlayerRef p) // 스폰가능 검사
        => stopAllSpawns == false && blockedPlayers.Contains(p) == false;

    public void ClearFalse() => clear = false;

    //--------------블럭 삭제 관련로직-----------------
    public void RemoveUnplacedBlock(PlayerRef p) // 지정 플레이어 배치안된 블럭제거
    {
        if (Runner.IsServer == false) return;

        var toRemove = GameObject.FindObjectsOfType<NetworkObject>()
            .Where(no => no.gameObject.layer == LayerMask.NameToLayer("Block"))
            .Where(no => no.InputAuthority == p)
            .Where(no => no.TryGetComponent<NetworkBlockController>(out var ctrl)
                         && ctrl.IsPlaced == false);

        foreach (var obj in toRemove)
        {
            Runner.Despawn(obj);
        }
    }

    public void RemoveUnplacedAllBlocks() // 모든 플레이어 배치안된 블럭제거
    {
        if (Runner.IsServer == false) return;

        var toRemove = GameObject.FindObjectsOfType<NetworkObject>()
            .Where(no => no.gameObject.layer == LayerMask.NameToLayer("Block"))
            .Where(no => no.TryGetComponent<NetworkBlockController>(out var ctrl)
                         && ctrl.IsPlaced == false);

        foreach (var obj in toRemove)
        {
            Runner.Despawn(obj);
        }
    }

    public void RemoveAllBlocks() // 모든 블럭제거
    {
        if (Runner.IsServer == false) return;

        var allBlocks = GameObject.FindObjectsOfType<NetworkObject>()
            .Where(no => no.gameObject.layer == LayerMask.NameToLayer("Block"));

        foreach (var block in allBlocks)
        {
            Runner.Despawn(block);
        }
    }
    //---------------------------------------------

    public void RaceModeClear(PlayerRef winner) //레이스모드 관련로직
    {
        clear = true;
        currentGameType = GameType.Race;
        Debug.Log("clearRace");

        //모든플레이어 블럭생성막힘 추가
        StopAllSpawns();
        
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
        RemoveUnplacedAllBlocks();
    }

    public void PuzzleModeClear() //퍼즐 모드 관련 로직
    {
        currentGameType = GameType.Puzzle;
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

    public void PuzzlePlayerEnd(PlayerRef player) //퍼즐 플레이어 탈락 감지 로직
    {
        if (failedPlayers.Contains(player)) return;
        
        failedPlayers.Add(player);

        // 해당 플레이어 블럭생성중지 
        StopPlayer(player);

        Debug.Log($"{player} 탈락");

        if (failedPlayers.Count >= Runner.ActivePlayers.Count()) //모든 플레이어 종료확인
        {
            PuzzleModeClear();
            RemoveUnplacedBlock(player);
            failedPlayers.Clear();
        }
    }

    public void SurvivalModeClear(PlayerRef winner) // 서바이벌 모드 관련 로직
    {
        currentGameType = GameType.Survival;
        //인자값으로 1등 남은 플레이어는 blockCount가 낮은순으로 등수
        Debug.Log("Survival clear");

        // 모든 플레이어 블럭생성을 막는로직
        StopAllSpawns();

        //** 지금은 쌓여진 블럭갯수로 판정 나중에 남은블럭으로 판정하게 변경 **
        var allBlocks = GameObject.FindObjectsOfType<NetworkObject>()
            .Where(no => no.gameObject.layer == LayerMask.NameToLayer("Block"))
            .Where(no => no.TryGetComponent<NetworkBlockController>(out var controller) && controller.IsPlaced);

        Debug.Log("1");
        var blockCounts = allBlocks
            .GroupBy(no => no.InputAuthority)
            .Select(group => new
            {
                Player = group.Key, Count = group.Count()
            });
        Debug.Log("2");
        PlayerRef[] ranking = blockCounts
            .OrderByDescending(entry => entry.Count)
            .Select(entry => entry.Player)
            .ToArray();
        Debug.Log("3");

        AssignScore(ranking);
        RemoveUnplacedAllBlocks();
    }

    public void SurvivePlayerDie(PlayerRef player) //마지막 생존자 찾는 로직
    {
        if (failedPlayers.Add(player) == false) return;
        Debug.Log(player + "죽음");

       
        // 해당 플레이어 블럭 생성막는 로직
        StopPlayer(player);

        int total = Runner.ActivePlayers.Count();
        Debug.Log("토탈"+total);
        
        if (failedPlayers.Count >= total - 1)
        {
            Debug.Log("실패한 "+failedPlayers.Count);
            PlayerRef winner =
                Runner.ActivePlayers.First(p => failedPlayers.Contains(p) == false);
            SurvivalModeClear(winner);
            RemoveUnplacedBlock(player);
        }
    }

    public void GetBlockCount(int count) //남은 블럭갯수를 가져옴
    {
        //문제 죽지않고 다른 플레이어가 게임을 클리어했을때 블럭 갯수를 어떻게 가져오게 할것인가
    }

    //----------이 밑에는 점수 관련 로직------------

    private void AssignScore(PlayerRef[] ranking)
    {
        Debug.Log("점수정산");
        
        //** 여기서 메달을 주게 해볼까? **
        //그래서 아얘 스코어 데이터가 메달도 가지고있고
        //스코어 박스는 그걸 UI로 연결만해서 보여주는거지
        
        lastRoundRank = ranking;
        for (int i = 0; i < ranking.Length; i++)
        {
            int score
                = i == 0 ? 3
                : i == 1 ? 2
                : i == 2 ? 1
                : 0;
            scoreData.AddScore(ranking[i], score);
        }

        Debug.Log("정산끝");
        ScoreDebug();
        
        RoundCleared?.Invoke(lastRoundRank[0], currentGameType);
        Debug.Log("점수 함수 끝");
    }

    private void ScoreDebug()
    {
        var allScore = GameClearManager.Instance.GetAllScore();
        foreach (var pair in allScore)
        {
            Debug.Log($"플레이어: {pair.Key},점수: {pair.Value}");
        }
    }


    public PlayerRef[] GetLastRoundRanking()
    {
        return lastRoundRank;
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