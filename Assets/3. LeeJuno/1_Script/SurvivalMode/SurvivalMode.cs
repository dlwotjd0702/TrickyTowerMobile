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
        SurvivalEvents.BlockSpawned += BlockSpawnCheck;
        SurvivalEvents.BlockDestroyed += BlockDestroyCheck;
    }

    private void OnDisable()
    {
        SurvivalEvents.BlockSpawned -= BlockSpawnCheck;
        SurvivalEvents.BlockDestroyed -= BlockDestroyCheck;
    }

    private void BlockSpawnCheck(PlayerRef p)
    {
        if (blockCount.ContainsKey(p) == false)
            blockCount[p] = 22;

        if (--blockCount[p] <= 0)
            GameClearManager.Instance.SurvivalModeClear(p);
    }

    private void BlockDestroyCheck(PlayerRef p)
    {
        if (hp.ContainsKey(p) == false)
            hp[p] = 3;

        if (--hp[p] <= 0)
        {
            GameClearManager.Instance.SurvivePlayerDie(p);
        }
    }
}