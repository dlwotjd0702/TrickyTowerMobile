using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualLandingVisual : MonoBehaviour
{
    private GameObject[] LandVisual = new GameObject[2];
    private NetworkBlockController _blockController;
    private bool flip = false;
    private void Start()
    {
        for (int i = 0; i < LandVisual.Length; i++)
        {
            LandVisual[i] = transform.GetChild(i).gameObject;
        }
        transform.TryGetComponent(out  _blockController);
    }

    private void Update()
    {
    }

    private void OnOffLandVisual()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
        {
            LandVisual[0].SetActive(false);
            LandVisual[1].SetActive(false);
        }
    }
    
    
}
