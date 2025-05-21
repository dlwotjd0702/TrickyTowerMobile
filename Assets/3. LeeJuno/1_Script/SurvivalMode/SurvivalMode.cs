using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SurvivalMode : MonoBehaviour
{
    Dictionary<PlayerRef, int> blockCount = new Dictionary<PlayerRef, int>();
    Dictionary<PlayerRef, int> hp = new Dictionary<PlayerRef, int>();

    private void OnEnable()
    {
        SurvivalEvents.BlockSpawned += SpawnCheck;
        SurvivalEvents.BlockDestroyed += DestroyCheck;
    }

    private void OnDisable()
    {
        SurvivalEvents.BlockSpawned -= SpawnCheck;
        SurvivalEvents.BlockDestroyed -= DestroyCheck;
    }

    private void SpawnCheck(PlayerRef p)
    {
        if (blockCount.ContainsKey(p) == false)
            blockCount[p] = 22;

        if (--blockCount[p] <= 0)
            GameClearManager.Instance.SurvivalModeClear(p);
    }

    private void DestroyCheck(PlayerRef p)
    {
        if (hp.ContainsKey(p) == false)
            hp[p] = 3;

        if (--hp[p] <= 0)
            Die(p);
    }

    private void Die(PlayerRef p)
    {
    }
}