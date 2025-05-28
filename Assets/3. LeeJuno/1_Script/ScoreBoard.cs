using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class ScoreBoard : NetworkBehaviour
{
    [SerializeField] private ScoreBoardBox[] boxes;
    [SerializeField] private GameObject goldMedal, silverMedal, bronzeMedal;

    // 서버 권한에서 호출
    public void ShowScoreBoard()
    {
        if (!Runner.IsServer) return;

        var ranking = GameClearManager.Instance.GetLastRoundRanking();

        // 초기화
        foreach (var b in boxes)
            b.ResetBox();

        for (int rank = 0; rank < ranking.Length; rank++)
        {
            var p = ranking[rank];
            int medals = rank == 0 ? 3 : rank == 1 ? 2 : rank == 2 ? 1 : 0;
            var prefab = rank == 0 ? goldMedal : rank == 1 ? silverMedal : bronzeMedal;

            // PlayerRef.AsIndex 로 바로 박스 선택
            int idx = p.AsIndex-1;
            boxes[idx].UpdateBox(medals, prefab);
        }

        Rpc_ShowUI();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_ShowUI()
    {
        gameObject.SetActive(true);
    }
}
