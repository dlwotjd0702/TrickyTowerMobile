using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class UiController : NetworkBehaviour
{
    [SerializeField] private NetworkSpawnHandler networkSpawnHandler;
    
    [SerializeField] private Image[] NextBlockImage;
    public string PlayerNumber;
    public int newIndex;
    public int preIndex;
    
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < NextBlockImage.Length; i++)
        {
            NextBlockImage[i].gameObject.SetActive(false);
        }
        preIndex = Random.Range(0, NextBlockImage.Length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewBlockChoice()
    {
        newIndex = Random.Range(0, networkSpawnHandler.blockPrefabs.Length);
        
        for (int i = 0; i < NextBlockImage.Length; i++)
        {
            NextBlockImage[i].gameObject.SetActive(false);
            if(i == newIndex) NextBlockImage[i].gameObject.SetActive(true);
        }
        
        preIndex = newIndex;
    }
}