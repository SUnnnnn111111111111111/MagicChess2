using UnityEngine;

namespace CameraController
{
    public interface ICameraMovementHandler
    {
        void Move(Vector3 inputDelta);
    }
}
