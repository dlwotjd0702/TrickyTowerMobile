using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SurvivalMode : MonoBehaviour
{
    Dictionary<PlayerRef, int> blockCount = new Dictionary<PlayerRef, int>();
    Dictionary<PlayerRef, int> hp = new Dictionary<PlayerRef, int>();
    private bool survivalClear;

    [SerializeField]
    private int clearBlock = 22;

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
        if (survivalClear) return;

        if (blockCount.ContainsKey(p) == false)
            blockCount[p] = clearBlock;

        Debug.Log("블럭수" + blockCount[p]);

        if (--blockCount[p] <= 0)
        {
            survivalClear = true;
            GameClearManager.Instance.SurvivalModeClear(p);
        }
    }

    private void BlockDestroyCheck(PlayerRef p)
    {
        Debug.Log("발동");
        if (hp.ContainsKey(p) == false)
            hp[p] = 3;

        Debug.Log("hp수" + hp[p]);

        if (--hp[p] <= 0)
        {
            GameClearManager.Instance.SurvivePlayerDie(p);
        }
    }
}