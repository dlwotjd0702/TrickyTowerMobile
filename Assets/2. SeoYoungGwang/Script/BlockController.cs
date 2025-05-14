using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    private InputManager inputManager;
    private SpawnManager spawnManager;
    
    private Rigidbody2D rigidBody;
    // Start is called before the first frame update
    void Start()
    {
        transform.TryGetComponent(out rigidBody);
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        rigidBody.isKinematic = true;
    }

    // Update is called once per frame

    private void OnTriggerEnter2D(Collider2D other)
    {
        rigidBody.isKinematic = false;
        gameObject.transform.TryGetComponent(out BlockController controller);
        gameObject.transform.TryGetComponent(out BoxCollider2D collider);
        spawnManager.OnSpawn = true;
        collider.isTrigger = false;
        controller.enabled = false;
    }
}
