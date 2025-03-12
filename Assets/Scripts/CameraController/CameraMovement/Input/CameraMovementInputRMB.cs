using UnityEngine;

namespace CameraController
{
    [RequireComponent(typeof(Camera))]
    public class CameraMovementInputRMB : CameraMovementInput
    {
        [SerializeField] private LayerMask _groundMask;
        
        private bool _dragEnabled;
        private Camera _camera;
        
        protected override void Awake()
        {
            base.Awake();
            
            _camera = GetComponent<Camera>();
        }

        protected override ICameraMovementHandler CreateCameraMovementHandler()
        {
            return new CameraMovementHandler(_properties);
        }

        protected override Vector3 ReadInputDelta()
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (IsClickedOnGround()) _dragEnabled = true;
                
                return Vector3.zero;
            }

            if (Input.GetMouseButtonUp(1))
            {
                _dragEnabled = false;
                
                return Vector3.zero;
            }

            if (_dragEnabled && Input.GetMouseButton(1))
            {
                // Получаем изменение позиции мыши
                Vector2 mouseDelta = Input.mousePositionDelta;

                // Преобразуем изменение позиции мыши в мировые координаты
                Vector3 moveDelta = _camera.transform.right * mouseDelta.x + _camera.transform.up * mouseDelta.y;

                // Учитываем ортографический размер камеры (если камера ортографическая)
                moveDelta *= _camera.orthographicSize * 0.01f;

                return moveDelta;
            }
            
            return Vector3.zero;
        }

        private bool IsClickedOnGround()
        {
            var pointerScreenPosition = Input.mousePosition;
            var ray = _camera.ScreenPointToRay(pointerScreenPosition);
            var result = Physics.Raycast(ray, out var hitInfo, float.MaxValue, _groundMask.value);

            return result;
        }
    }
}