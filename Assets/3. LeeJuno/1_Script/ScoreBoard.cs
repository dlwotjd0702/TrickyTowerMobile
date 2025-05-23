using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class ScoreBoard : NetworkBehaviour
{
    [Header("Medal Prefabs")]
    [SerializeField]
    private GameObject goldMedalPrefab;

    [SerializeField]
    private GameObject silverMedalPrefab;

    [SerializeField]
    private GameObject bronzeMedalPrefab;

    [Header("Slot Points")]
    [SerializeField]
    private Transform[][] slotPoints; //이차원 배열

    private const int MaxSlots = 9;

    // 각 플레이어가 몇 슬롯 채웠는지
    private Dictionary<PlayerRef, int> fillSlots = new Dictionary<PlayerRef, int>();

    public override void Spawned()
    {
        if (!Runner.IsServer) return;

        // 플레이어별 인덱스 초기화
        foreach (var p in Runner.ActivePlayers)
            fillSlots[p] = 0;
    }

    public void FillMedals(PlayerRef[] ranking)
    {
        // 1등→3개, 2등→2개, 3등→1개, 4등→0개
        for (int i = 0; i < ranking.Length; i++)
        {
            var player = ranking[i];
            int count
                = i == 0 ? 3
                : i == 1 ? 2
                : i == 2 ? 1
                : 0;
            
            GameObject prefab
                = i == 0 ? goldMedalPrefab
                : i == 1 ? silverMedalPrefab
                : bronzeMedalPrefab;

            for (int k = 0; k < count; k++)
                AddMedal(player, prefab);
        }
    }

    void AddMedal(PlayerRef player, GameObject prefab)
    {
        int idx = fillSlots[player];
        if (idx >= MaxSlots) return;

        var pt = slotPoints[player.AsIndex][idx];
        Instantiate(prefab, pt.position, Quaternion.identity);
        fillSlots[player]++;
    }
    
    public PlayerRef[] GetFinalRankingBySlots()
    {
        return fillSlots
            .OrderByDescending(kv => kv.Value)
            .Select(kv => kv.Key)
            .ToArray();
    }
    
    public void ResetSlots()
    {
        fillSlots.Clear();
        foreach (var p in Runner.ActivePlayers)
            fillSlots[p] = 0;
       
    }
}