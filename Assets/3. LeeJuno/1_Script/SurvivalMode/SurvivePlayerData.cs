using System.Collections;
using System.Collections.Generic;
using Fusion;


public class SurvivePlayerData
{
    public PlayerRef Player { get; private set; }
    public int HP { get; private set; } = 3;
    public int BrickCount { get; private set; } = 22;
    public bool IsDead => HP <= 0 || BrickCount <= 0;

    private float invincibleTime = 1.5f;
    private float lastHitTime = -999f;

    public SurvivePlayerData(PlayerRef player)
    {
        Player = player;
    }

    public void OnBlockSpawned()
    {
        BrickCount--;
    }

    public void OnBlockDestroyed(float currentTime)
    {
        if (currentTime - lastHitTime < invincibleTime) return;
        lastHitTime = currentTime;

        HP--;
    }
}