using System;
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
        public bool leftFastMove;
        public bool rightFastMove;
    }

    [Networked] public bool IsPlaced { get; set; }
    public bool biggerTrigger = false;
    public bool noRotateTrigger = false;
    public bool fastFallTrigger = false;
    

    [SerializeField] float moveDistance = 1f;
    [SerializeField] float downSpeed    = 2f;

    Rigidbody2D        _rb;
    NetworkRigidbody2D _netRb;
    public BoxCollider2D[]     _trigger;

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
        _trigger = GetComponents<BoxCollider2D>();

        _rb.bodyType     = RigidbodyType2D.Kinematic;
        _rb.gravityScale = 0;

        // 3) 매니저·이펙트 참조 채우기
        networkManager  = FindObjectOfType<NetworkManager>();
        effectManager   = FindObjectOfType<EffectManager>();
        soundManager    = FindObjectOfType<SoundManager>();

        // 4) 클라이언트 예측 단계에서 블록 할당
        if (Object.HasInputAuthority)
        {
            effectManager.Block         = gameObject;
            effectManager.isBlockChange = true;
        }
        
        // 블록 콜라이더 좀 줄이기
        foreach (var c in GetComponents<BoxCollider2D>()) c.size *= 0.8f;

    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    private void RPC_TriggerShake(RpcInfo info = default)
    {
        effectManager.IsShake = true;
    }

    private void bigger()
    {
        if (biggerTrigger == true)
        {
            Debug.Log("[NetworkBlockController] biggerTrigger 작동! 블록 크기 2배.");
            gameObject.transform.localScale *= 2;
            biggerTrigger = false;
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
        
        if (isOwner)
        {
            effectManager.Block = gameObject;
        }

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
                if (!noRotateTrigger)
                {
                    transform.Rotate(0, 0, 90);
                    soundManager.OnRotateSound();
                }
                else
                {
                    if (input.Rotate && noRotateTrigger)
                        Debug.Log("[NetworkBlockController] noRotateTrigger로 회전 차단.");
                }
            }

            if (input.leftFastMove)
            {
                Debug.Log("Left Fast Move");
                effectManager.preMovePos = transform.position;
                transform.position -= Vector3.right * moveDistance * 2;
                effectManager.afterMovePos  = transform.position;
                effectManager.isBlockMove = true;
                if (Object.HasInputAuthority)
                {
                    effectManager.OnShadow(); 
                    soundManager.OnMoveSound();
                }
            }
            
            if (input.rightFastMove)
            {
                effectManager.preMovePos = transform.position;
                transform.position += Vector3.right * moveDistance * 2;
                effectManager.afterMovePos  = transform.position;
                effectManager.isBlockMove = true;
                if (Object.HasInputAuthority)
                {
                    effectManager.OnShadow(); 
                    soundManager.OnMoveSound();
                }
            }

            // 빠른/자동 하강
            if (Runner.IsServer)
            {

                // fastFall 플래그가 켜져 있으면 무조건 빠르게, 아니면 input.FastDown 에 따라
                float speed = (fastFallTrigger || input.FastDown) 
                    ? downSpeed * 5f 
                    : downSpeed;

                if (fastFallTrigger)
                    Debug.Log($"[NetworkBlockController] fastFall 적용, 속도 = {speed}");
                
                transform.position += Vector3.down * (speed * Runner.DeltaTime);
                
            }
            
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag("Floor") || other.CompareTag("Respawn")) && !IsPlaced )
        {
            // 착지 처리
            IsPlaced = true;
            Runner.SetIsSimulated(Object, Object.HasStateAuthority);

            // 착지 후엔 noRotateTrigger, fastFall 리셋
            noRotateTrigger = false;
            fastFallTrigger = false;
           
      

            _rb.bodyType     = RigidbodyType2D.Dynamic;
            _rb.gravityScale = 1;
            foreach (var c in GetComponents<BoxCollider2D>())
            {
                c.isTrigger = false;
                c.size /= 0.8f;
            }

            gameObject.tag = "Floor";
            if (Runner.IsServer)
            {
                RPC_TriggerShake();
            }
            if(networkManager==null) networkManager = FindObjectOfType<NetworkManager>();
            networkManager.RequestNextBlock(Object.InputAuthority);

            if (other.CompareTag("Respawn"))
            {
                // Respawn도 동일하게
                if (Runner.IsServer)
                {
                    RPC_TriggerShake();
                }
                Destroy(gameObject);
                return;
            }
            soundManager.OnLandSound();
        }
        
        if (other.CompareTag("Respawn") && IsPlaced)
        {
            soundManager.OnFallSound();
            effectManager.IsShake = true;
            if (effectManager.Block == gameObject)
            {
                effectManager.Block         = null;
                effectManager.isBlockChange = false;
                effectManager.isBlockMove   = false;
                effectManager.IsShadow      = false;
                effectManager.IsShake       = false;
            }
            Destroy(gameObject);    
        }
    }
}
