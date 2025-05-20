using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class FinishLine : MonoBehaviour //NetworkBehaviour
{
    //결승선에 3초간 닿으면 게임종료
    [SerializeField]
    private float timer = 0f;

    private PlayerRef? currentPlayer = null;
    private bool playerTouched = false;
    private int blockCount = 0;

    /*public override void FixedUpdateNetwork() //네트워크용
    {
        if (Runner.IsServer == false || currentPlayer == null || playerTouched == false)
            return;

        timer += Runner.DeltaTime;

        if (timer >= 3f)
        {
            enabled = false;
        }

        playerTouched = false;
    }*/

    private void Update()
    {
        if (playerTouched == false)
            return;

        timer += Time.deltaTime;

        if (timer >= 3f)
        {
            if (currentPlayer != null)
                GameClearManager.Instance.RaceModeClear(currentPlayer.Value);
           
            playerTouched = false;
            enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Block") == false) return;

        var block = other.GetComponent<NetworkObject>();
        if (block == null) return;

        blockCount++;
        playerTouched = true;

        if (currentPlayer == null)
            currentPlayer = block.InputAuthority;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Block") == false) return;

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