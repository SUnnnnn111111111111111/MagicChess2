using System;
using CameraController;
using UnityEngine;


public class OrthographicCameraZoomHandler : IZoomHandler
{
    private readonly Camera _camera;
    private readonly CameraZoomProperties _cameraZoomProperties;
    private float _orthoSize;
    private float _velocity;
     

    public OrthographicCameraZoomHandler(Camera camera, CameraZoomProperties cameraZoomProperties)
    {
        _camera = camera;
        _cameraZoomProperties = cameraZoomProperties;
        _orthoSize = _camera.orthographicSize;
    }
    

    public void Zoom(float inputDelta)
    {
        var inputDeltaWithSpeed = inputDelta * _cameraZoomProperties._zoomSpeed;
        
        _orthoSize = Mathf.Clamp(_orthoSize - inputDeltaWithSpeed, _cameraZoomProperties._zoomDistanceMin, _cameraZoomProperties._zoomDistanceMax);
        
        var newOrthoSize = Mathf.SmoothDamp(
            _camera.orthographicSize, 
            _orthoSize, 
            ref _velocity, 
            _cameraZoomProperties._smoothingFactor);
        
        _camera.orthographicSize = newOrthoSize;
    }
}
