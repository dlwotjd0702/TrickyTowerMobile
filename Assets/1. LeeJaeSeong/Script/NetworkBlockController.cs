using System;
using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(NetworkRigidbody2D), typeof(NetworkObject))]
public class NetworkBlockController : NetworkBehaviour
{
    public struct NetworkBlockInputData : INetworkInput
    {
        public int MoveX;
        public bool Rotate;
        public bool FastDown;
        public bool leftFastMove;
        public bool rightFastMove;
    }

    [Networked]
    public bool IsPlaced { get; set; }

    [SerializeField]
    float moveDistance = 1f;

    [SerializeField]
    float downSpeed = 2f;

    Rigidbody2D _rb;
    NetworkRigidbody2D _netRb;
    public BoxCollider2D[] _trigger;

    public EffectManager effectManager;
    public SoundManager soundManager;
    public NetworkManager networkManager;

    public override void Spawned()
    {
        // 1) 물리 예측: 배치 전엔 입력권한자, 배치 후엔 서버 권위자
        bool predict = Object.HasInputAuthority && !IsPlaced;
        bool server = Object.HasStateAuthority;
        Runner.SetIsSimulated(Object, predict || server);


        // 2) 컴포넌트 캐싱
        _rb = GetComponent<Rigidbody2D>();
        _netRb = GetComponent<NetworkRigidbody2D>();
        _trigger = GetComponents<BoxCollider2D>();

        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.gravityScale = 0;

        // 3) 매니저·이펙트 참조 채우기
        networkManager = FindObjectOfType<NetworkManager>();
        effectManager = FindObjectOfType<EffectManager>();
        soundManager = FindObjectOfType<SoundManager>();

        // 4) 클라이언트 예측 단계에서 블록 할당
        if (Object.HasInputAuthority)
        {
            effectManager.Block = gameObject;
            effectManager.isBlockChange = true;
        }

        // 블록 콜라이더 좀 줄이기
        foreach (var c in GetComponents<BoxCollider2D>()) c.size *= 0.9f;
    }

    public override void FixedUpdateNetwork()
    {
        // 1) 이미 배치된 블록은 더 이상 이동 안 함
        if (IsPlaced)
            return;

        // 2) 예측(클라이언트) 또는 권위(서버) 둘 중 하나만 입력 처리
        bool isOwner = Object.HasInputAuthority;
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
            {
                transform.Rotate(0, 0, 90);
                soundManager.OnRotateSound();
            }

            if (input.leftFastMove)
            {
                Debug.Log("Left Fast Move");
                effectManager.preMovePos = transform.position;
                transform.position -= Vector3.right * moveDistance * 2;
                effectManager.afterMovePos = transform.position;
                effectManager.isBlockMove = true;
                effectManager.OnShadow();
                soundManager.OnMoveSound();
            }

            if (input.rightFastMove)
            {
                effectManager.preMovePos = transform.position;
                transform.position += Vector3.right * moveDistance * 2;
                effectManager.afterMovePos = transform.position;
                effectManager.isBlockMove = true;
                effectManager.OnShadow();
                soundManager.OnMoveSound();
            }

            // 빠른/자동 하강
            if (Runner.IsServer)
            {
                float speed = input.FastDown ? downSpeed * 5f : downSpeed;
                transform.position += Vector3.down * (speed * Runner.DeltaTime);
            }
        }
    }

//클라이언트에서의 충돌을 서버도 인식해서 돌림 if(Object.InputAuthority == Runner.LocalPlayer) 클라만 굴려야하는게 있다면 이 검사 추가
    void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag("Floor") || other.CompareTag("Respawn")) && !IsPlaced)
        {
            IsPlaced = true;

            // 착지 후엔 서버 권위만 시뮬레이션
            Runner.SetIsSimulated(Object, Object.HasStateAuthority);

            _rb.bodyType = RigidbodyType2D.Dynamic;
            _rb.gravityScale = 1;
            foreach (var c in GetComponents<BoxCollider2D>())
            {
                c.isTrigger = false;
                c.size /= 0.9f;
            }

            gameObject.tag = "Floor";
            if (networkManager == null)
            {
                networkManager = FindObjectOfType<NetworkManager>();
            }

            if (Object.InputAuthority == Runner.LocalPlayer) effectManager.IsShake = true;

            networkManager.RequestNextBlock(Object.InputAuthority);
            if (other.CompareTag("Respawn"))
            {
                soundManager.OnFallSound();
                SurvivalHPDown(Object.InputAuthority);
                Destroy(gameObject);
                return;
            }

            if (other.gameObject.layer == LayerMask.NameToLayer("Block") && effectManager.IsShadow)
            {
                other.gameObject.transform.TryGetComponent(out Rigidbody2D placeRigidBody);
                Vector3 forceOrigin = other.ClosestPoint(transform.position);
                Vector3 toDownBlockNormalVector2 = (transform.position - forceOrigin).normalized;
                Vector3 toPlaceBlockNormalVector2 = (other.transform.position - forceOrigin).normalized;

                float force = effectManager.preMovePos.x;

                Vector2 toDownBlockNormalX = new Vector2(toDownBlockNormalVector2.x, 0);
                Vector2 toPlaceBlockNormalX = new Vector2(toPlaceBlockNormalVector2.x, 0);
                _rb.AddForceAtPosition(toDownBlockNormalX * force, forceOrigin, ForceMode2D.Impulse);
                placeRigidBody.AddForceAtPosition(toPlaceBlockNormalVector2 * force, forceOrigin, ForceMode2D.Impulse);
                Debug.Log("Conflict!");
            }

            soundManager.OnLandSound();
        }
        else if (other.CompareTag("Respawn") && IsPlaced)
        {
            soundManager.OnFallSound();
            effectManager.IsShake = true;
            SurvivalHPDown(Object.InputAuthority);
            Destroy(gameObject);
        }
    }

    private void SurvivalHPDown(PlayerRef p)
    {
        Debug.Log("hp 다운 이벤트");
        SurvivalEvents.Destroyed(p);
    }
}