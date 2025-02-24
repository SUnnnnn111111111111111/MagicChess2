using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private enum Mode
    {
        LookAt,
        LookAtInverted,
        CameraForward,
        CameraForwardInverted
    }

    [SerializeField] private Mode mode;

    private void LateUpdate()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

        switch (mode)
        {
            case Mode.LookAt:
                transform.LookAt(mainCamera.transform);
                break;
            case Mode.LookAtInverted:
                Vector3 dirFromCamera = transform.position - mainCamera.transform.position;
                transform.LookAt(transform.position + dirFromCamera);
                break;
            case Mode.CameraForward:
                Vector3 flatForward = mainCamera.transform.forward;
                flatForward.y = 0; // Убираем наклон по оси Y
                flatForward.Normalize(); // Нормализуем вектор
                transform.forward = flatForward;
                break;
            case Mode.CameraForwardInverted:
                Vector3 flatBackward = -mainCamera.transform.forward;
                flatBackward.y = 0; // Убираем наклон по оси Y
                flatBackward.Normalize(); // Нормализуем вектор
                transform.forward = flatBackward;
                break;
        }
    }
}