using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource audioSource;
    public GameObject currentBlock;
    private NetworkBlockController networkBlockController;
    
    [Header("AudioClips")]
    public AudioClip BlockLandClip;

    private void Start()
    {
        transform.TryGetComponent(out audioSource);
    }

    private void Update()
    {
        
    }

    public void OnLandSound()
    {
        audioSource.PlayOneShot(BlockLandClip);
    }
}
