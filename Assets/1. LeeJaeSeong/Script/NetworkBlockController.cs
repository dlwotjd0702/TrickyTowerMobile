// 파일명: NetworkBlockController.cs
using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

public struct NetworkBlockInputData : INetworkInput
{
    public int  MoveX;     // -1,0,+1
    public bool Rotate;    // 회전 요청
    public bool FastDown;  // 빠른 하강
}

[RequireComponent(typeof(Rigidbody2D), typeof(NetworkRigidbody2D))]
public class NetworkBlockController : NetworkBehaviour
{
    [Networked] public bool IsPlaced { get; set; }
    [SerializeField] float moveDistance = 1f;
    [SerializeField] float downSpeed    = 2f;

    Rigidbody2D        _rb;
    NetworkRigidbody2D _netRb;
    BoxCollider2D _trigger;

    public override void Spawned()
    {
        _rb    = GetComponent<Rigidbody2D>();
        _netRb = GetComponent<NetworkRigidbody2D>();
        _trigger = GetComponent<BoxCollider2D>();
        _rb.bodyType     = RigidbodyType2D.Kinematic;
        _rb.gravityScale = 0;
        IsPlaced         = false;
    }

    public override void FixedUpdateNetwork()
    {
        if (IsPlaced || !Object.HasInputAuthority)
            return;

      
        if (GetInput(out NetworkBlockInputData input))
        {
            // 좌우 Discrete 이동
            if (input.MoveX != 0)
                transform.position += Vector3.right * input.MoveX * moveDistance;

            // 회전
            if (input.Rotate)
                transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + 90);

            // 자동/빠른 하강
            float speed = input.FastDown ? downSpeed * 5 : downSpeed;
            transform.position += Vector3.down * (speed * Runner.DeltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
      //  if (!Object.HasStateAuthority || IsPlaced) 
      //      return;

        if (other.CompareTag("Floor")&&IsPlaced == false)
        {
            IsPlaced = true;
            _rb.bodyType     = RigidbodyType2D.Dynamic;
            _rb.gravityScale = 1;
            foreach (var col in GetComponents<Collider2D>())
                col.isTrigger = false;
            if (_trigger != null)
                _trigger.isTrigger = false;
            gameObject.tag = "Floor";
            NetworkSpawnHandler.Instance.RequestNextBlock(Object.InputAuthority);
            EffectManager.Instance.OnShake = true;
        }

        if (other.CompareTag("Respawn"))
        {
            Destroy(gameObject);
        }
    }
}
