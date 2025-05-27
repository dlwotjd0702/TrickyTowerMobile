// 파일명: NetworkInputHandler.cs

using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NetworkInputHandler : MonoBehaviour, INetworkRunnerCallbacks,IPointerDownHandler,IPointerUpHandler 
{
    
    private int _prevRawX = 0;
    private int _prevRawX2 = 0;
    private int rawX2 = 0;

    // 🔸 1프레임 키 입력 저장용
    private bool _rotateQueued = false;
    private bool isLeftFastMove = false;
    private bool isRightFastMove = false;

    
    
    [SerializeField] private EffectManager effectManager;
    [SerializeField] private ButtonController leftMoveButton;
    [SerializeField] private ButtonController rightMoveButton;
    [SerializeField] private ButtonController leftFastMoveButton;
    [SerializeField] private ButtonController rightFastMoveButton;
    [SerializeField] private ButtonController downButton;
    [SerializeField] private ButtonController leftRotateButton;
    [SerializeField] private ButtonController rightRotateButton;

    private void Update()
    {
        
        // 🔸 키 눌림 체크 → flag 저장
        KeyBoardInput();
        RotateOnClick();
        LeftMoveOnClick();
        RightMoveOnClick();
        LeftFastMoveOnClick();
        RightFastMoveOnClick();

    }

    private void KeyBoardInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
        {
            _rotateQueued = true;
            effectManager.RotateSetLandVisual();
        }
        if (Input.GetKeyDown(KeyCode.U) && !effectManager.IsShadow)
        {
            effectManager.IsShadow = true;
            effectManager.isRight = false;
            isLeftFastMove = true;
            Debug.Log("isLeftFastMove");
        }

        if (Input.GetKeyDown(KeyCode.I) && !effectManager.IsShadow)
        {
            effectManager.IsShadow = true;
            effectManager.isRight = true;
            isRightFastMove = true;
        }
    }

    private void RotateOnClick()
    {
        if (leftRotateButton.onClick || rightRotateButton.onClick)
        {
            _rotateQueued = true;
            effectManager.RotateSetLandVisual();
            leftRotateButton.onClick =  false;
            rightRotateButton.onClick = false;
        }
    }
    
    private void LeftMoveOnClick()
    {
        if (leftMoveButton.onClick)
        {
            rawX2 = -1;
            leftMoveButton.onClick = false;
        }
    }
    
    private void RightMoveOnClick()
    {
        if (rightMoveButton.onClick)
        {
            rawX2 = 1;
            rightMoveButton.onClick = false;
        }
    }

    private void LeftFastMoveOnClick()
    {
        if (leftFastMoveButton.onClick)
        {
            effectManager.IsShadow = true;
            effectManager.isRight = false;
            isLeftFastMove = true;
            Debug.Log("isLeftFastMove");
            leftFastMoveButton.onClick = false;
        }
    }

    private void RightFastMoveOnClick()
    {
        if (rightFastMoveButton.onClick)
        {
            effectManager.IsShadow = true;
            effectManager.isRight = true;
            isRightFastMove = true;
            rightFastMoveButton.onClick = false;
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        int rawX = (int)Input.GetAxisRaw("Horizontal");
        
        bool isDown = downButton.isClick || Input.GetKey(KeyCode.S);

        int moveX = 0;
        if (_prevRawX == 0 && rawX != 0)
            moveX = rawX;
        if(_prevRawX2 == 0 && rawX2 != 0)
            moveX =  rawX2;

        _prevRawX = rawX;
        _prevRawX2 = rawX2;

        // 🔸 저장된 키입력 사용 후 초기화
        var data = new NetworkBlockController.NetworkBlockInputData
        {
            MoveX    = moveX,
            Rotate   = _rotateQueued,
            FastDown = isDown,
            leftFastMove = isLeftFastMove,
            rightFastMove = isRightFastMove
        };

        _rotateQueued = false; // 🔸 초기화
        isLeftFastMove = false;
        isRightFastMove = false;
        rawX2 = 0;

        input.Set(data);
    }
    
    



    public void OnConnectedToServer(NetworkRunner runner) {}
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress){}
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList){}
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){}
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){}
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {}
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {}
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {}
    public void OnShutdown(NetworkRunner runner, ShutdownReason reason) {}
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason){}
    public void OnDisconnectedFromServer(NetworkRunner runner) {}
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {}
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason){}
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message){}
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data){}
    public void OnSessionListUpdated(NetworkRunner runner, SessionInfo[] sessionList) {}
    public void OnCustomAuthenticationResponse(NetworkRunner runner, System.Collections.Generic.Dictionary<string, object> data) {}
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken token) {}
    public void OnSceneLoadStart(NetworkRunner runner) {}
    public void OnSceneLoadDone(NetworkRunner runner) {}
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, System.ArraySegment<byte> data) {}
    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }
}