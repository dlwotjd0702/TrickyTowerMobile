using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioSource BGMaudioSource;
    public GameObject currentBlock;
    private NetworkBlockController networkBlockController;
    
    [Header("AudioClips")]
    public AudioClip blockLandClip;
    public AudioClip blockRotateClip;
    public AudioClip blockFallClip;
    public AudioClip blockMoveClip;
    public AudioClip backGroundMusicClip;

    private void Start()
    {
        transform.TryGetComponent(out audioSource);
        BGMaudioSource = transform.GetComponentInChildren<AudioSource>();
        BGMaudioSource.clip = backGroundMusicClip;
        BGMaudioSource.loop = true;
        BGMaudioSource.Play();
    }

    private void Update()
    {
        
    }

    public void OnLandSound()
    {
        audioSource.PlayOneShot(blockLandClip);
    }

    public void OnRotateSound()
    {
        audioSource.PlayOneShot(blockRotateClip);
    }

    public void OnFallSound()
    {
        audioSource.PlayOneShot(blockFallClip);
    }
    
    public void OnMoveSound()
    {
        audioSource.PlayOneShot(blockMoveClip);
    }
}
