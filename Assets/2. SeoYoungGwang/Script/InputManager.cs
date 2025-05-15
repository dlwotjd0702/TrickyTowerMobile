using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class InputManager : MonoBehaviour
{
    public GameObject currentBlock;
    private SpawnManager spawnManager;
    
    [SerializeField] private float moveDistance;
    [SerializeField] private float downSpeed;
    [SerializeField] private float downSpeedMutiple;
    
    private void Awake()
    {
        
    }

    void Start()
    {
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();

    }

    // Update is called once per frame
    void Update()
    {
        MoveInput();
        SpinInput();
        DownMove();
    }
    
    private void DownMove()
    {
        currentBlock.transform.position -= new Vector3(0, downSpeed * Time.deltaTime, 0);
    }
    
    private void MoveInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            currentBlock.transform.position += Vector3.left * moveDistance;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            downSpeed *= downSpeedMutiple;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            downSpeed /= downSpeedMutiple;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            currentBlock.transform.position += Vector3.right * moveDistance;
        }
    }
    
    private void SpinInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
        {
            currentBlock.transform.rotation = Quaternion.Euler(0, 0, currentBlock.transform.rotation.eulerAngles.z + 90);
        }
    }
}
