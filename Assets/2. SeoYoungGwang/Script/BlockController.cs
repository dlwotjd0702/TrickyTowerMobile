using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    private InputManager inputManager;
    private SpawnManager spawnManager;
    private BoxCollider2D[] boxCollider;

    private Rigidbody2D rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = gameObject.GetComponents<BoxCollider2D>();
        transform.TryGetComponent(out rigidBody);
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        rigidBody.isKinematic = true;
    }

    // Update is called once per frame

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("FinishLine")) return;
        rigidBody.isKinematic = false;
        gameObject.transform.TryGetComponent(out BlockController controller);
        spawnManager.OnSpawn = true;
        foreach (BoxCollider2D box in boxCollider)
        {
            box.isTrigger = false;
        }

        controller.enabled = false;
    }
}