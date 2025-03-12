using UnityEngine;

namespace CameraController
{
    public class PerspectiveCameraZoomHandler : IZoomHandler
    {
        private readonly Camera _camera;
        private readonly CameraZoomProperties _cameraZoomProperties;
        private float _fov;
        private float _velocity;
 

        public PerspectiveCameraZoomHandler(Camera camera, CameraZoomProperties cameraZoomProperties)
        {
            _camera = camera;
            _cameraZoomProperties = cameraZoomProperties;
            _fov = _camera.fieldOfView;
        }


        public void Zoom(float inputDelta)
        {
            var inputDeltaWithSpeed = inputDelta * _cameraZoomProperties._zoomSpeed;
    
            _fov = Mathf.Clamp(_fov - inputDeltaWithSpeed, _cameraZoomProperties._zoomDistanceMin, _cameraZoomProperties._zoomDistanceMax);
    
            var newFov = Mathf.SmoothDamp(
                _camera.fieldOfView, 
                _fov, 
                ref _velocity, 
                _cameraZoomProperties._smoothingFactor);
    
            _camera.fieldOfView = newFov;
        }
    }
}