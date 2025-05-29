using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SurvivalMode : MonoBehaviour
{
    Dictionary<PlayerRef, int> blockCount = new Dictionary<PlayerRef, int>();
    Dictionary<PlayerRef, int> hp = new Dictionary<PlayerRef, int>();
    Dictionary<PlayerRef, bool> isInvincible = new Dictionary<PlayerRef, bool>();

    [SerializeField]
    private int clearBlock = 220;

    [SerializeField]
    private float invincibleTime = 1.5f;

    private bool survivalClear;

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

        if (blockCount[p]-- <= 0)
        {
            survivalClear = true;
            GameClearManager.Instance.SurvivalModeClear(p);
            blockCount.Clear();
        }

        Debug.Log("블럭수" + blockCount[p]);
    }

    private void BlockDestroyCheck(PlayerRef p)
    {
        if (isInvincible.TryGetValue(p, out bool inv) && inv)
        {
            return;
        }

        isInvincible[p] = true;
        StartCoroutine(IvincibleCooldown(p));

        if (hp.ContainsKey(p) == false)

            hp[p] = 3;

        if (hp[p]-- <= 1)
        {
            //블럭스폰이 막혀야하는데
            GameClearManager.Instance.SurvivePlayerDie(p);
            hp[p] = 3;
        }

        Debug.Log("hp수" + hp[p]);
    }

    private IEnumerator IvincibleCooldown(PlayerRef p)
    {
        yield return new WaitForSeconds(invincibleTime);
        isInvincible[p] = false;
    }
}