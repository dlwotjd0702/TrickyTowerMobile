using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("고정 카메라 연결")]
    [SerializeField] private Camera mainCam;
    [SerializeField] private Camera[] miniCams;

    [Header("카메라 위치 고정값")]
    [SerializeField] private float fixedY = 15f;
    [SerializeField] private float fixedZ = -30f;

    /// <summary>
    /// 로컬 플레이어 전용 메인 카메라 세팅
    /// </summary>
    public void SetupMainCam(Vector3 spawnPoint)
    {
        if (mainCam == null) return;

        mainCam.enabled = true;
        mainCam.rect = new Rect(0f, 0f, 1f, 2f / 3f); // ⬇️ 아래 2/3 영역
        mainCam.transform.position = new Vector3(spawnPoint.x, fixedY, fixedZ);
    }

    /// <summary>
    /// 상대 플레이어용 미니 카메라 세팅
    /// </summary>
    public void SetupMiniCam(Vector3 spawnPoint, int index)
    {
        if (index < 0 || index >= miniCams.Length) return;

        Camera cam = miniCams[index];
        cam.enabled = true;
        cam.rect = new Rect(index * (1f / 3f), 2f / 3f, 1f / 3f, 1f / 3f); // ⬆️ 위 1/3을 3등분
        cam.transform.position = new Vector3(spawnPoint.x, fixedY, fixedZ);
    }
}