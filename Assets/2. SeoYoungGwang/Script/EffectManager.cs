using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance;
    
    [Header("GroundShake")]
    
    public float shakeTerm;
    public float shakeDuration;
    public float shakeDistance;
    public Transform BackGround;
    public Transform MainCamera;
    public bool OnShake = false;
    private Vector3 camOriginPos;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (OnShake)
        {
            camOriginPos = MainCamera.transform.position;
            StartCoroutine(Shake());
            OnShake = false;
        }
    }

    private IEnumerator Shake()
    {
        WaitForSeconds wfs = new WaitForSeconds(shakeTerm);
        float currentTime = 0;
        float shakeTime = 0;
        bool ToOne = true;
        
        while (currentTime < shakeDuration)
        {
            currentTime += Time.deltaTime;
            MainCamera.transform.position = camOriginPos + new Vector3(Random.Range(-shakeDistance, shakeDistance), Random.Range(-shakeDistance, shakeDistance), 0);
            yield return null;
        }
        MainCamera.transform.position = camOriginPos;
    }
    
    
}
