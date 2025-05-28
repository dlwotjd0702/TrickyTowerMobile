using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class RaceLine : NetworkBehaviour
{
    [SerializeField]
    private float fallSpeed = 1f;

    private Vector3 startPos;

    public override void Spawned()
    {
        if (!HasStateAuthority) return; // 서버에서만 이동 제어
        startPos = transform.position;
    }


    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        transform.position += Vector3.down * fallSpeed * Runner.DeltaTime;
    }

    public void ResetPosition()
    {
        var networkTransform = GetComponent<NetworkTransform>();
        if (networkTransform != null)
            networkTransform.Teleport(new Vector3(0, 5, 0));
        else
            transform.position = new Vector3(0, 5, 0);
    }
}