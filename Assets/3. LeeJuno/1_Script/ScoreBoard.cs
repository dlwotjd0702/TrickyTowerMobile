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
    private Transform[][] slotPoints;

    private const int MaxSlots = 9;

    // 각 플레이어가 몇 슬롯 채웠는지
    private Dictionary<PlayerRef, int> filledSlots = new Dictionary<PlayerRef, int>();

    public override void Spawned()
    {
        if (!Runner.IsServer) return;
        // 플레이어별 인덱스 초기화
        foreach (var p in Runner.ActivePlayers)
            filledSlots[p] = 0;
    }

    /// <summary>
    /// 라운드가 끝나고 호출하세요.
    /// ranking 배열은 [1등,2등,3등,4등…] 순입니다.
    /// </summary>
    public void FillMedals(PlayerRef[] ranking)
    {
        // 1등→3개, 2등→2개, 3등→1개, 4등→0개
        for (int i = 0; i < ranking.Length; i++)
        {
            var player = ranking[i];
            int count = i == 0 ? 3 : i == 1 ? 2 : i == 2 ? 1 : 0;
            GameObject prefab = i == 0 ? goldMedalPrefab : i == 1 ? silverMedalPrefab : bronzeMedalPrefab;

            for (int k = 0; k < count; k++)
                AddMedal(player, prefab);
        }
    }

    void AddMedal(PlayerRef player, GameObject prefab)
    {
        int idx = filledSlots[player];
        if (idx >= MaxSlots) return;

        var pt = slotPoints[player.AsIndex][idx];
        Instantiate(prefab, pt.position, Quaternion.identity);
        filledSlots[player]++;
    }

    /// <summary>
    /// 슬롯에 채워진 개수 기준으로 최종 등수를 리턴합니다.
    /// </summary>
    public PlayerRef[] GetFinalRankingBySlots()
    {
        return filledSlots
            .OrderByDescending(kv => kv.Value)
            .Select(kv => kv.Key)
            .ToArray();
    }

    /// <summary>
    /// 세션 재시작 시 호출해서 UI 초기화
    /// </summary>
    public void ResetSlots()
    {
        filledSlots.Clear();
        foreach (var p in Runner.ActivePlayers)
            filledSlots[p] = 0;
        // Scene 상에 남은 메달 프리팹은 직접 Destroy 해 주셔야 합니다.
    }
}