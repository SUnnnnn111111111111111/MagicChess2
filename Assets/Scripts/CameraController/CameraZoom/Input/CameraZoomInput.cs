using UnityEngine;
using UnityEngine.Serialization;

namespace CameraController
{
    public abstract class CameraZoomInput : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [FormerlySerializedAs("_zoomProperties")] [SerializeField] private CameraZoomProperties cameraZoomProperties;

        private IZoomHandler _zoomHandler;

        private void Awake()
        {
            _zoomHandler = _camera.orthographic
                ? new OrthographicCameraZoomHandler(_camera, cameraZoomProperties)
                : new PerspectiveCameraZoomHandler(_camera, cameraZoomProperties);
        }

        private void LateUpdate()
        {
            var inputDelta = ReadInputDelta();

            _zoomHandler.Zoom(inputDelta);
        }
        
        protected abstract float ReadInputDelta();
    }
}