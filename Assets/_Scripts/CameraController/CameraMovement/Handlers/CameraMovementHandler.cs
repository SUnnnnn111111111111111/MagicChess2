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
            // Учитываем только горизонтальный поворот камеры (ось Y)
            float yRotation = _properties._pivot.eulerAngles.y;
            Quaternion horizontalRotation = Quaternion.Euler(0, yRotation, 0);
    
            // Преобразуем входной вектор в направление, зависящее от поворота камеры
            Vector3 movement = horizontalRotation * new Vector3(inputDelta.x, 0, inputDelta.z) * _properties._moveSpeed;
            _cashedCameraPosition += movement;
    
            _properties._pivot.position = Vector3.Lerp(
                _properties._pivot.position, 
                _cashedCameraPosition, 
                Time.deltaTime / _properties._smoothingFactor
            );
        }
    }
}