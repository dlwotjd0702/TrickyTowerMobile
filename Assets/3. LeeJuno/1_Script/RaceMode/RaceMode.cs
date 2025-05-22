using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class RaceMode : MonoBehaviour //NetworkBehaviour
{
    //결승선에 3초간 닿으면 게임종료
    private float timer = 0f;
    private PlayerRef? currentPlayer = null;
    private bool playerTouched = false;
    private int blockCount = 0;

    /*public override void FixedUpdateNetwork() //네트워크용
    {
        if (playerTouched == false)
            return;

        timer += Runner.DeltaTime;

        if (timer >= 3f)
        {
        if (currentPlayer != null)
                GameClearManager.Instance.RaceModeClear(currentPlayer.Value);
            enabled = false;
        }

        playerTouched = false;
    }*/

    private void Update()
    {
        #region DebugKey

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (currentPlayer != null)
                Debug.Log(GameClearManager.Instance.GetPlayerScore(currentPlayer.Value));
            else
            {
                Debug.Log("currentPlayer is null");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            var allScore = GameClearManager.Instance.GetAllScore();
            foreach (var pair in allScore)
            {
                Debug.Log($"플레이어: {pair.Key},점수: {pair.Value}");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GameClearManager.Instance.ResetScore();
        }

        #endregion

        if (playerTouched == false)
            return;

        timer += Time.deltaTime;

        if (timer >= 3f)
        {
            if (currentPlayer != null)
                GameClearManager.Instance.RaceModeClear(currentPlayer.Value);

            playerTouched = false;
        }
    }

    //** 시간비례 해서 선내려옴 **
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Block")) return;

        var block = other.GetComponent<NetworkObject>();
        if (block == null) return;

        // var blockCon = other.GetComponent<NetworkBlockController>();
        // if (blockCon.IsPlaced == false) return;

        blockCount++;
        playerTouched = true;

        if (currentPlayer == null)
            currentPlayer = block.InputAuthority;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Block")) return;

        var block = other.GetComponent<NetworkObject>();
        if (block == null) return;

        blockCount--;
        if (blockCount <= 0)
        {
            playerTouched = false;
            timer = 0f;
        }
    }
}