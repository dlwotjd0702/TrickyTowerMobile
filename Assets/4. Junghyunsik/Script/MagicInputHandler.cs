using UnityEngine;

public class MagicInputHandler : MonoBehaviour
{
    [SerializeField] private MagicManager magicManager;

    private void Awake()
    {
        if (magicManager == null)
            magicManager = FindObjectOfType<MagicManager>();
    }

    private void Update()
    {
        // 1번 키 누른 건 무조건 처리 (예: 로그, 다른 게임 로직)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Alpha1 pressed");
            // MagicManager 쪽 버튼이 떠 있지 않을 때만 ActivateMagic 호출
            if (!magicManager.HasActiveMagic())
                magicManager.ActivateMagic(0);
        }

        // 2번 키
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Alpha2 pressed");
            if (!magicManager.HasActiveMagic())
                magicManager.ActivateMagic(1);
        }

        // 3번 키
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Alpha3 pressed");
            if (!magicManager.HasActiveMagic())
                magicManager.ActivateMagic(2);
        }

        // ...나중에 다른 곳에서 쓰실 키 입력 처리도 여기에 추가하시면
        // 버튼 활성 중에도 정상적으로 입력 감지는 됩니다.
    }
}