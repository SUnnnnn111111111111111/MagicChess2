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
                return Input.mousePositionDelta * _mouseSensitivity;
            }
            
            return Vector3.zero;
        }

        private Vector3 ReadKeyboardInputDelta()
        {
            var vertical = Input.GetAxis("Vertical");
            var horizontal = Input.GetAxis("Horizontal");
            
            return new Vector3(-horizontal, -vertical, 0) * _keyboardSensitivity;
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