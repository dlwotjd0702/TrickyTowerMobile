using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EffectManager : MonoBehaviour
{
    public GameObject Block;
    private string BlockTag;
    private NetworkBlockController networkBlockController;
    public bool isBlockChange = false;
    public bool isBlockMove = false;
    
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
    private float MoveDistance = 0.5f;
    private Vector3 afterMovePos;
    private Vector3 preMovePos;
    private GameObject[] Shadows;
    public float ShadowDuration;

    [Header("LandVisual")] 
    private GameObject currentLandVisual;
    private GameObject[] LandVisuals = new GameObject[4];
    private GameObject VerticalLandVisual;
    private GameObject HorizontalLandVisual;
    private bool isHorizontal = true;
    

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

        for (int i = 0; i < LandVisuals.Length; i++)
        {
            LandVisuals[i] = transform.GetChild(i).gameObject;
            LandVisuals[i].SetActive(false);
        }
    }

    private void Update()
    {
        OnShake();
        //FastMoveInput();
        //OnShadow();
        ChangeLandVisual();
        LandVisualUpdate();
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

    private void FastMoveInput()
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

    #region 블록 예상 착지 지점 시각화 효과

    private void ChangeLandVisual()
    {
        if (isBlockChange)
        {
            for (int i = 0; i < LandVisuals.Length; i++)
            {
                LandVisuals[i].SetActive(false);
            }
            
            isHorizontal = true;
            BlockTag = Block.tag;
            switch (BlockTag)
            {
                case"OBlock":
                    VerticalLandVisual = LandVisuals[1];
                    HorizontalLandVisual = LandVisuals[1];
                    break;
                case"SBlock":
                case"ZBlock":
                case"TBlock":
                case"LBlock":
                case"JBlock":
                    VerticalLandVisual = LandVisuals[1];
                    HorizontalLandVisual = LandVisuals[2];
                    break;
                case"IBlock":
                    VerticalLandVisual = LandVisuals[0];
                    HorizontalLandVisual = LandVisuals[3];
                    break;
            }
            VerticalLandVisual.SetActive(false);
            currentLandVisual = HorizontalLandVisual;
            HorizontalLandVisual.SetActive(true);
            transform.position = Block.transform.position;
            isBlockChange = false;
        }
    }

    private void LandVisualUpdate()
    {
        if (isBlockMove)
        {
            transform.position = Block.transform.position;
            isBlockMove = false;
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
        {
            isHorizontal = !isHorizontal;
            if (isHorizontal)
            {
                VerticalLandVisual.SetActive(false);
                currentLandVisual = HorizontalLandVisual;
            }
            else
            {
                HorizontalLandVisual.SetActive(false);
                currentLandVisual = VerticalLandVisual;
            }
            
            currentLandVisual.SetActive(true);
        }
    }
    
    
    #endregion

    
    
    
}
