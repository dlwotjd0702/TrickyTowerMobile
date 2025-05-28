using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class ScoreBoard : NetworkBehaviour
{
    [SerializeField]
    private ScoreBoardBox[] boxes;

    [SerializeField]
    private GameObject goldMedal, silverMedal, bronzeMedal;

    // 서버 권한에서 호출
    public void ShowScoreBoard()
    {
        if (!Runner.IsServer) return;

        var ranking = GameClearManager.Instance.GetLastRoundRanking();
        var allMedals = GameClearManager.Instance.GetPlayerMedals();

        for (int i = 0; i < ranking.Length; i++)
        {
            int idx = ranking[i].AsIndex - 1;

            GameObject prefab = i switch
            {
                0 => goldMedal,
                1 => silverMedal,
                2 => bronzeMedal,
                _ => null
            };

            int count = i switch
            {
                0 => 3, // 1등은 3개
                1 => 2, // 2등은 2개
                2 => 1, // 3등은 1개
                _ => 0
            };

            for (int j = 0; j < count; j++)
            {
                boxes[idx].AddMedal(prefab);
            }
        }

        Rpc_ShowUI();
    }

    public void ReSetBox()
    {
        foreach (var box in boxes)
        {
            box.ResetBox();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_ShowUI()
    {
        gameObject.SetActive(true);
    }
}