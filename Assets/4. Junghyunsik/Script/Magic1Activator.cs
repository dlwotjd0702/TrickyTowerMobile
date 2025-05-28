using UnityEngine;
using UnityEngine.UI;

public class Magic1Activator : MonoBehaviour
{
    [Header("이 버튼을 클릭하면")]
    [SerializeField] private Button triggerButton;

    [Header("활성화할 Magic1 오브젝트")]
    [SerializeField] private GameObject magic1;

    private void Awake()
    {
        // 클릭 리스너 등록
        triggerButton.onClick.RemoveAllListeners();
        triggerButton.onClick.AddListener(ActivateMagic1);
    }

    private void ActivateMagic1()
    {
        if (magic1 != null)
            magic1.SetActive(true);
    }
}