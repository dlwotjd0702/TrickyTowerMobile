using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class SurvivalHeart : MonoBehaviour
{
    [SerializeField]
    private GameObject[] XIcon;

    public PlayerRef Owner { get; private set; }

    public void SetOwner(PlayerRef p)
    {
        Owner = p;
    }

    public void HeartUpdate(int hp)
    {
        XIcon[hp].SetActive(true);
    }

    public void Reset()
    {
        foreach (var x in XIcon)
        {
            x.SetActive(false);
        }
    }
}