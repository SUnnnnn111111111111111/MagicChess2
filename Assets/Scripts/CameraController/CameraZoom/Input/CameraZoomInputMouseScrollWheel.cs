using UnityEngine;

namespace CameraController
{
    public class CameraZoomInputMouseScrollWheel : CameraZoomInput
    {
        protected override float ReadInputDelta()
        {
            return Input.GetAxis("Mouse ScrollWheel");
        }
    }
}