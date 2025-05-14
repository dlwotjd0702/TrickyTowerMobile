using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCotroller : MonoBehaviour
{
    
    [SerializeField] private float moveDistance;
    private Rigidbody2D rigidBody;
    // Start is called before the first frame update
    void Start()
    {
        transform.TryGetComponent(out rigidBody);
    }

    // Update is called once per frame
    void Update()
    {
        MoveInput();
        SpinInput();
    }

    private void MoveInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            gameObject.transform.position += Vector3.up *  moveDistance;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            gameObject.transform.position += Vector3.left * moveDistance;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            gameObject.transform.position += Vector3.down * moveDistance;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            gameObject.transform.position += Vector3.right * moveDistance;
        }
    }

    private void SpinInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 0, gameObject.transform.rotation.eulerAngles.z + 90);
        }
    }
}
