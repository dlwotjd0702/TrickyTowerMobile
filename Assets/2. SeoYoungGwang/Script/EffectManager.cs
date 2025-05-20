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
    

    [Header("FastMovement")] 
    public bool IsShadow = false;
    public SpriteRenderer Shadow;
    public int ShadowNum;
    private float MoveDistance = 0.5f;
    public float ShadowTailDistance = 0.5f;
    private Vector3 afterMovePos;
    private Vector3 preMovePos;
    private GameObject[] Shadows;
    public float ShadowDuration;
    public GameObject tempBlock;
    private float multipUseVecter1;
    private bool isRight;
    public float RoatateValue = 45f;

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
        FastMoveInput();
        ChangeLandVisual();
        LandVisualUpdate();
    }

    #region 착지 시 흔들리는 효과

    private void OnShake()
    {
        if (IsShake)
        {
            
            StartCoroutine(Shake());
            IsShake = false;
        }
    }

    private IEnumerator Shake()
    {
        Vector3 camOriginPos = MainCamera.transform.position;
        WaitForSeconds wfs = new WaitForSeconds(shakeTerm);
        float currentTime = 0;
        float shakeTime = 0;
        bool ToOne = true;
        
        while (currentTime < shakeDuration)
        {
            currentTime += Time.deltaTime;
            Debug.Log(MainCamera.transform.position);
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
        if ((Input.GetKeyDown(KeyCode.U) || Input.GetKeyDown(KeyCode.I)) && !IsShadow)
        {
            IsShadow = true;
            preMovePos = tempBlock.transform.position;
            if (Input.GetKeyDown(KeyCode.U))
            {
                tempBlock.transform.position -= new Vector3(MoveDistance, 0, 0);
                isRight = false;
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                tempBlock.transform.position += new Vector3(MoveDistance, 0, 0);
                isRight = true;
            }
            afterMovePos = tempBlock.transform.position;
            multipUseVecter1 = (afterMovePos - preMovePos).normalized.x;
            OnShadow();
        }
        
        
    }

    private void OnShadow()
    {
        float ShadowTerm = ((afterMovePos.x - preMovePos.x) + ShadowTailDistance * multipUseVecter1) / ShadowNum;
        for (int i = 0; i < ShadowNum; i++)
        {
            Shadows[i].transform.TryGetComponent(out SpriteRenderer renderer);
            renderer.sprite = tempBlock.GetComponent<SpriteRenderer>().sprite;
            if (ShadowTerm > 0)
            {
                Shadows[i].transform.position = preMovePos + new Vector3(ShadowTerm * i - ShadowTailDistance, 0, 0);
            }
            else
            {
                Shadows[i].transform.position = preMovePos + new Vector3(ShadowTerm * i + ShadowTailDistance, 0, 0);
            }
                
            Shadows[i].transform.rotation = tempBlock.transform.rotation;
            Shadows[i].SetActive(true);
        }
        OnFastMoveEffect(); 
        StartCoroutine(OffShadow());
    }

    private void OnFastMoveEffect()
    {
        //흔들림
        StartCoroutine(Shake());
        
        //회전
        StartCoroutine(FastMoveRotate());
    }
    
    private IEnumerator FastMoveRotate()
    {

        float currentTime = 0;
        Vector3 blockOriginRot = tempBlock.transform.rotation.eulerAngles;
        Vector3 targetRot = blockOriginRot + Vector3.forward * (RoatateValue * multipUseVecter1);
        
        while (currentTime < ShadowDuration/2)
        {
            currentTime += Time.deltaTime;
            tempBlock.transform.rotation = Quaternion.Lerp(Quaternion.Euler(blockOriginRot), Quaternion.Euler(targetRot), currentTime/(ShadowDuration/2)  );
            yield return null;
        }
        currentTime = 0;
        while (currentTime < ShadowDuration/2)
        {
            currentTime += Time.deltaTime;
            tempBlock.transform.rotation = Quaternion.Lerp(Quaternion.Euler(targetRot), Quaternion.Euler(blockOriginRot), currentTime/(ShadowDuration/2)  );
            yield return null;
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
        IsShadow = false;
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
