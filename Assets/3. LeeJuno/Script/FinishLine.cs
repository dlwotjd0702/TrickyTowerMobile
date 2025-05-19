using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{//결승선에 3초간 닿으면 게임종료
    private float timer = 0f;
    
    void Start()
    {
    }
    
    void Update()
    {
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Block"))
        {
            timer += Time.deltaTime;
            if (timer >= 3f)
            {
                //게임매니저(레이스 모드) 게임종료
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Block"))
        {
            timer = 0f;
        }
    }
}