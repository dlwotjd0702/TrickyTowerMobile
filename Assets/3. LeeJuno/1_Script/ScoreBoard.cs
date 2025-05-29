using System.Collections;
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
            int count
                = i == 0 ? 3
                : i == 1 ? 2
                : i == 2 ? 1
                : 0;

            MedalType type
                = i == 0 ? MedalType.Gold
                : i == 1 ? MedalType.Silver
                : i == 2 ? MedalType.Bronze
                : MedalType.None;

            for (int j = 0; j < count; j++)
            {
                Rpc_AddMedal(idx, type); // 모든 클라이언트에서 실행됨
            }
        }

        Rpc_ShowUI();
        StartCoroutine(delaynextround());
    }
    private IEnumerator delaynextround()
    {
        yield return new WaitForSeconds(3f);
        Rpc_HideUI();
        
    }

    public void ReSetBox()
    {
        foreach (var box in boxes)
        {
            box.ResetBox();
        }
    }

 
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_AddMedal(int boxIndex, MedalType medal)
    {
        GameObject prefab = medal switch
        {
            MedalType.Gold => goldMedal,
            MedalType.Silver => silverMedal,
            MedalType.Bronze => bronzeMedal,
            _ => null
        };

        boxes[boxIndex].AddMedal(prefab);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_ShowUI()
    {
        gameObject.SetActive(true);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_HideUI()
    {
        gameObject.SetActive(false);
    }
}