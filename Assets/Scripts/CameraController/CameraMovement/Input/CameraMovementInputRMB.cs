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
                // Учитываем поворот камеры при преобразовании дельты мыши
                Vector3 mouseDelta = new Vector3(
                    Input.mousePositionDelta.x, 
                    0, 
                    Input.mousePositionDelta.y
                );
        
                float yRotation = _camera.transform.eulerAngles.y;
                Quaternion horizontalRotation = Quaternion.Euler(0, yRotation, 0);
                Vector3 moveDelta = horizontalRotation * mouseDelta;
        
                moveDelta *= _camera.orthographicSize * 0.01f;
                return moveDelta;
            }
    
            return Vector3.zero;
        }

        private bool IsClickedOnGround()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out _, float.MaxValue, _groundMask.value);
        }
    }
}