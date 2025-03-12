using UnityEngine;

namespace CameraController
{
    public class CameraMovementHandler : ICameraMovementHandler
    {
        private readonly CameraMovementProperties _properties;
        private Vector3 _cashedCameraPosition;

        public CameraMovementHandler(CameraMovementProperties properties)
        {
            _properties = properties;
            _cashedCameraPosition = properties._pivot.position;
        }
        
        public void Move(Vector3 inputDelta)
        {
            _cashedCameraPosition -= new Vector3(inputDelta.x, 0, inputDelta.y) * _properties._moveSpeed;
            _properties._pivot.position = Vector3.Lerp(_properties._pivot.position, _cashedCameraPosition, 
                Time.deltaTime / _properties._smoothingFactor);
        }
    }
}