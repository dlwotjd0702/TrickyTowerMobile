using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public bool OnSpawn = true;
    private InputManager inputManager;
    [SerializeField] private GameObject[] blockPrefabs = new GameObject[7];
    public GameObject[] blockPrefabs_ => blockPrefabs;
    
    // Start is called before the first frame update
    void Start()
    {
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Spawn();
    }

    private void Spawn()
    {
        if (OnSpawn)
        {
            GameObject tempBlock = Instantiate(blockPrefabs[Random.Range(0, blockPrefabs.Length)], transform.position, Quaternion.identity);
            inputManager.currentBlock = tempBlock;
            OnSpawn = false;
        }
    }
}
