using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceTest : MonoBehaviour
{
    private Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        transform.TryGetComponent(out rigidbody);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InputForce()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (Input.GetMouseButton(0))
            {
                rigidbody.AddForceAtPosition(-hit.normal * rigidbody.mass, hit.point, ForceMode.Impulse);
                Debug.Log(hit.normal);
            }
            
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //if(other.)
    }
}
