using UnityEngine;

namespace CameraController
{
    [RequireComponent(typeof(Camera))]
    public class CameraMovementInputKeyboardRMB : CameraMovementInput
    {
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private float _mouseSensitivity = 0.2f;
        [SerializeField] private float _keyboardSensitivity = 0.2f;
        
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
            var mouseInputDelta = ReadRMBInputDelta();
            var keyboardInputDelta = ReadKeyboardInputDelta();
            return (mouseInputDelta + keyboardInputDelta) * _camera.orthographicSize * 0.01f;
        }

        private Vector3 ReadRMBInputDelta()
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
                return new Vector3(-Input.mousePositionDelta.x, 0, -Input.mousePositionDelta.y) * _mouseSensitivity;
            }
            
            return Vector3.zero;
        }

        private Vector3 ReadKeyboardInputDelta()
        {
            // Инвертируем оси для соответствия экранным направлениям
            var vertical = Input.GetAxis("Vertical");
            var horizontal = Input.GetAxis("Horizontal");
            return new Vector3(horizontal, 0, vertical) * _keyboardSensitivity;
        }

        private bool IsClickedOnGround()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out _, float.MaxValue, _groundMask.value);
        }
    }
}