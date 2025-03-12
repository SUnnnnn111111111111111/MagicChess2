using System;

namespace CameraController
{
    [Serializable]
    public class CameraZoomProperties
    {
        public float _zoomSpeed = 7.5f;
        public float _smoothingFactor = 0.2f;
        public float _zoomDistanceMin = 5f;
        public float _zoomDistanceMax = 25f; 
    }
}