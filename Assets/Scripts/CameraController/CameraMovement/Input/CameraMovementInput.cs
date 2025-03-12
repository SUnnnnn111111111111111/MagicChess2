using UnityEngine;

namespace CameraController
{
    public abstract class CameraMovementInput : MonoBehaviour
    {
        [SerializeField] protected CameraMovementProperties _properties;
        
        private ICameraMovementHandler _cameraMovementHandler;

        protected virtual void Awake()
        {
            _cameraMovementHandler = CreateCameraMovementHandler();
        }

        private void Update()
        {
            var inputDelta = ReadInputDelta();
            
            _cameraMovementHandler.Move(inputDelta);
        }
        
        protected abstract ICameraMovementHandler CreateCameraMovementHandler();
        protected abstract Vector3 ReadInputDelta();
    }
}