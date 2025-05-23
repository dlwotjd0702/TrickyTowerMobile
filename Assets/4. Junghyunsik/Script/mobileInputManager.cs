using UnityEngine;

public class mobileInputManager : MonoBehaviour
{
    public GameObject currentBlock;
    private SpawnManager spawnManager;

    public float moveDistance = 1f;
    public float swipeThreshold = 50f;

    private Vector2 touchStartPos;

    private void Start()
    {
        spawnManager = FindObjectOfType<SpawnManager>();
    }

    private void Update()
    {
        if (currentBlock == null)
        {
            //currentBlock = spawnManager.CurrentBlock;
            if (currentBlock == null) return;
        }

        #if UNITY_EDITOR || UNITY_STANDALONE
        HandleKeyboard();
        #endif

        HandleTouch();
    }

    private void HandleKeyboard()
    {
        if (Input.GetKey(KeyCode.A)) MoveLeft(1);
        if (Input.GetKey(KeyCode.D)) MoveRight(1);

        // 회전(Q 또는 E)
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E))
            Rotate90();
    }

    private void HandleTouch()
    {
        if (Input.touchCount == 0) return;
        var t = Input.GetTouch(0);
        switch (t.phase)
        {
            case TouchPhase.Began:
                touchStartPos = t.position;
                break;
            case TouchPhase.Ended:
                var delta = t.position - touchStartPos;
                if (Mathf.Abs(delta.x) > swipeThreshold &&
                    Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                {
                    if (delta.x > 0) MoveRight(2);
                    else              MoveLeft (2);
                }
                else
                {
                    if (t.position.x < Screen.width * .5f) MoveLeft(1);
                    else                                   MoveRight(1);
                }
                break;
        }
    }

    private void MoveLeft(int mul) =>
        currentBlock.transform.position += Vector3.left  * moveDistance * mul;
    private void MoveRight(int mul) =>
        currentBlock.transform.position += Vector3.right * moveDistance * mul;

    // UI 버튼으로도 호출 가능
    public void Rotate90() =>
        currentBlock.transform.Rotate(0, 0, 90);
}