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
    private Dictionary<PlayerRef, int> playerBlockCount = new();

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
            Debug.Log("clear");
            enabled = false;
        }

        playerTouched = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Block") == false) return;

        var block = other.GetComponent<NetworkObject>();
        if (block == null) return;

        PlayerRef player = block.InputAuthority;

        if (playerBlockCount.ContainsKey(player) == false)
        {
            playerBlockCount[player] = 0;
        }

        playerTouched = true;
        playerBlockCount[player]++;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Block") == false) return;
        {
            var block = other.GetComponent<NetworkObject>();
            if (block == null) return;

            PlayerRef player = block.InputAuthority;
            if (currentPlayer == null)
            {
                currentPlayer = player;
                timer = 0f;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Block") == false) return;

        var block = other.GetComponent<NetworkObject>();
        if (block == null) return;

        PlayerRef player = block.InputAuthority;

        if (playerBlockCount.ContainsKey(player))
        {
            playerBlockCount[player]--;
            if (playerBlockCount[player] <= 0)
            {
                playerBlockCount.Remove(player);

                if (currentPlayer != null && player == currentPlayer.Value)
                {
                    timer = 0f;
                    currentPlayer = null;
                    playerTouched = false;
                }
            }
        }
    }
}