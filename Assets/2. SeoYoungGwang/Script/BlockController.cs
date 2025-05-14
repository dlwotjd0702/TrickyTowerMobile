using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    
    [SerializeField] private float moveDistance;
    [SerializeField] private float downSpeed;
    [SerializeField] private float downSpeedMutiple;
    private Rigidbody2D rigidBody;
    // Start is called before the first frame update
    void Start()
    {
        transform.TryGetComponent(out rigidBody);
        rigidBody.isKinematic = true;
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
        transform.position -= new Vector3(0, downSpeed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        rigidBody.isKinematic = false;
        gameObject.transform.TryGetComponent(out BlockController controller);
        gameObject.transform.TryGetComponent(out BoxCollider2D collider);
        collider.isTrigger = false;
        controller.enabled = false;
    }

    private void MoveInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            gameObject.transform.position += Vector3.left * moveDistance;
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
            gameObject.transform.position += Vector3.right * moveDistance;
        }
    }

    private void SpinInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 0, gameObject.transform.rotation.eulerAngles.z + 90);
        }
    }
}
