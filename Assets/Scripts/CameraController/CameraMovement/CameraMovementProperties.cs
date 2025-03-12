using System;
using UnityEngine;

namespace CameraController
{
    [Serializable]
    public class CameraMovementProperties
    {
        public Transform _pivot;
        public float _moveSpeed = 0.5f;
        public float _smoothingFactor = 0.2f;
    }
}