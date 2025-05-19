using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EffectManager : MonoBehaviour
{
    [Header("GroundShake")]
    
    public float shakeTerm;
    public float shakeDuration;
    public float shakeDistance;
    public Transform BackGround;
    public Transform MainCamera;
    public bool IsShake = false;
    public bool IsShadow = false;
    private Vector3 camOriginPos;
    
    [Header("FastMovement")]
    public SpriteRenderer Shadow;
    public int ShadowNum;
    public GameObject Block;
    private float MoveDistance = 0.5f;
    private Vector3 afterMovePos;
    private Vector3 preMovePos;
    private GameObject[] Shadows;
    public float ShadowDuration;

    private void Awake()
    {
        
    }

    private void Start()
    {
        Shadows = new GameObject[ShadowNum];
        for (int i = 0; i < ShadowNum; i++)
        {
            Shadows[i] = Instantiate(Shadow.gameObject, transform.position, Quaternion.identity);
            Shadows[i].SetActive(false);
        }
    }

    private void Update()
    {
        OnShake();
        TempInput();
        OnShadow();
    }

    #region 착지 시 흔들리는 효과

    private void OnShake()
    {
        if (IsShake)
        {
            camOriginPos = MainCamera.transform.position;
            StartCoroutine(Shake());
            IsShake = false;
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
            //BackGround.transform.position = camOriginPos + new Vector3(Random.Range(-shakeDistance, shakeDistance), Random.Range(-shakeDistance, shakeDistance), 0);
            yield return null;
        }
        MainCamera.transform.position = camOriginPos;
    }

    #endregion

    #region 좌우 고속 이동 시 잔상 효과

    private void TempInput()
    {
        preMovePos = Block.transform.position;
        if (Input.GetKeyDown(KeyCode.U))
        {
            Block.transform.position -= new Vector3(MoveDistance, 0, 0);
            IsShadow = true;
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            Block.transform.position += new Vector3(MoveDistance, 0, 0);
            IsShadow = true;
        }
        afterMovePos = Block.transform.position;
        
        
        
    }

    private void OnShadow()
    {
        if (IsShadow)
        {
            float ShadowTerm = MoveDistance / ShadowNum;
            for (int i = 0; i < ShadowNum; i++)
            {
                Shadows[i].transform.TryGetComponent(out SpriteRenderer renderer);
                renderer.sprite = Block.GetComponent<SpriteRenderer>().sprite;
                Shadows[i].transform.position = preMovePos + new Vector3(ShadowTerm * i, 0, 0);
                Shadows[i].transform.rotation = Block.transform.rotation;
                Shadows[i].SetActive(true);
            }

            StartCoroutine(OffShadow());
            IsShadow = false;
        }
    }

    private IEnumerator OffShadow()
    {
        WaitForSeconds wfs = new WaitForSeconds(ShadowDuration);
        yield return wfs;
        for (int i = 0; i < ShadowNum; i++)
        {
            Shadows[i].SetActive(false);
        }
    }

    #endregion

    
    
    
}
