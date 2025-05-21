using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleMode : MonoBehaviour
{
    //플레이어의 벽돌감지 배치된 벽돌이 선에 닿으면 게임 종료 판정 

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Block")) return;

        var block = other.GetComponent<NetworkBlockController>();
        if (block.IsPlaced == false) return;
        
        Destroy(block.gameObject);
        GameClearManager.Instance.PlayerFailed(block.Object.InputAuthority);
    }
}