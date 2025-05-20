using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(NetworkRigidbody2D), typeof(NetworkObject))]
public class NetworkBlockController : NetworkBehaviour
{
    public struct NetworkBlockInputData : INetworkInput
    {
        public int  MoveX;
        public bool Rotate;
        public bool FastDown;
    }

    [Networked] public bool IsPlaced { get; set; }

    [SerializeField] float moveDistance = 1f;
    [SerializeField] float downSpeed    = 2f;

    Rigidbody2D        _rb;
    NetworkRigidbody2D _netRb;
    BoxCollider2D      _trigger;

    public EffectManager  effectManager;
    public SoundManager  soundManager;
    public NetworkManager networkManager;

    public override void Spawned()
    {
        // 1) 물리 예측: 배치 전엔 입력권한자, 배치 후엔 서버 권위자
        bool predict = Object.HasInputAuthority && !IsPlaced;
        bool server  = Object.HasStateAuthority;
        Runner.SetIsSimulated(Object, predict || server);

        // 2) 컴포넌트 캐싱
        _rb      = GetComponent<Rigidbody2D>();
        _netRb   = GetComponent<NetworkRigidbody2D>();
        _trigger = GetComponent<BoxCollider2D>();

        _rb.bodyType     = RigidbodyType2D.Kinematic;
        _rb.gravityScale = 0;

        // 3) 매니저·이펙트 참조 채우기
        networkManager  = FindObjectOfType<NetworkManager>();
        effectManager   = networkManager.spawnHandler.effectManager;

        // 4) 클라이언트 예측 단계에서 블록 할당
        if (Object.HasInputAuthority)
        {
            effectManager.Block         = gameObject;
            effectManager.isBlockChange = true;
        }
    }

    public override void FixedUpdateNetwork()
    {
        // 1) 이미 배치된 블록은 더 이상 이동 안 함
        if (IsPlaced)
            return;

        // 2) 예측(클라이언트) 또는 권위(서버) 둘 중 하나만 입력 처리
        bool isOwner  = Object.HasInputAuthority;
        bool isServer = Object.HasStateAuthority;
        if (!isOwner && !isServer)
            return;

        // 3) 입력이 있으면 적용
        if (GetInput(out NetworkBlockInputData input))
        {
            // 좌우 Discrete 이동
            if (input.MoveX != 0)
            {
                transform.position += Vector3.right * input.MoveX * moveDistance;
                effectManager.isBlockMove = true;
            }

            // 회전
            if (input.Rotate)
                transform.Rotate(0, 0, 90);

            // 빠른/자동 하강
            float speed = input.FastDown ? downSpeed * 5f : downSpeed;
            transform.position += Vector3.down * (speed * Runner.DeltaTime);
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag("Floor") || other.CompareTag("Respawn")) && !IsPlaced )
        {
            IsPlaced = true;

            // 착지 후엔 서버 권위만 시뮬레이션
            Runner.SetIsSimulated(Object, Object.HasStateAuthority);

            _rb.bodyType     = RigidbodyType2D.Dynamic;
            _rb.gravityScale = 1;
            foreach(var c in GetComponents<Collider2D>()) c.isTrigger = false;
            _trigger.isTrigger = false;
            gameObject.tag     = "Floor";

            networkManager.RequestNextBlock(Object.InputAuthority);
            effectManager.IsShake = true;
            if (other.CompareTag("Respawn"))
                Destroy(gameObject);
        }
        else if (other.CompareTag("Respawn") && IsPlaced)
        {
            effectManager.IsShake = true;
            Destroy(gameObject);    
        }

        
    }
}
