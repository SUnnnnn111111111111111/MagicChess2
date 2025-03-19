using UnityEngine;
using System.Collections;

namespace RTS_Cam
{
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("RTS Camera")]
    public class RTS_Camera : MonoBehaviour
    {
        #region Foldouts

#if UNITY_EDITOR

        public int lastTab = 0;
        public bool movementSettingsFoldout;
        public bool zoomingSettingsFoldout;
        public bool rotationSettingsFoldout;
        public bool heightSettingsFoldout;
        public bool mapLimitSettingsFoldout;
        public bool inputSettingsFoldout;

#endif

        #endregion

        private Transform m_Transform;
        private Camera cam;
        public bool useFixedUpdate = false;

        #region Movement

        public float baseMovementSpeed = 5f; // Базовая скорость движения (масштабируется от orthographicSize)
        public float screenEdgeMovementSpeed = 3f;
        public float rotationSpeed = 3f;
        public float panningSpeed = 10f;
        public float mouseRotationSpeed = 10f;

        #endregion

        #region Zoom

        public float minZoom = 5f; 
        public float maxZoom = 20f; 
        public float initialOrthographicSize = 18f;
        public float zoomDampening = 5f;
        public float keyboardZoomingSensitivity = 2f;
        public float scrollWheelZoomingSensitivity = 10f;

        private float zoomPos = 0.5f; 

        #endregion

        #region MapLimits

        public bool limitMap = false;
        public float limitX = 50f;
        public float limitY = 50f;

        #endregion

        #region Input

        public bool useScreenEdgeInput = false;
        public float screenEdgeBorder = 25f;

        public bool useKeyboardInput = true;
        public string horizontalAxis = "Horizontal";
        public string verticalAxis = "Vertical";

        public bool usePanning = true;
        public KeyCode panningKey = KeyCode.Mouse0;

        public bool useKeyboardZooming = false;
        public KeyCode zoomInKey = KeyCode.E;
        public KeyCode zoomOutKey = KeyCode.Q;

        public bool useScrollwheelZooming = true;
        public string zoomingAxis = "Mouse ScrollWheel";

        public bool useKeyboardRotation = false;
        public KeyCode rotateRightKey = KeyCode.X;
        public KeyCode rotateLeftKey = KeyCode.Z;

        public bool useMouseRotation = true;
        public KeyCode mouseRotationKey = KeyCode.Mouse1;

        #endregion

        private Vector2 KeyboardInput
        {
            get { return useKeyboardInput ? new Vector2(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis)) : Vector2.zero; }
        }

        private Vector2 MouseInput
        {
            get { return Input.mousePosition; }
        }

        private float ScrollWheel
        {
            get { return -Input.GetAxis(zoomingAxis); }
        }

        private Vector2 MouseAxis
        {
            get { return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); }
        }

        private int ZoomDirection
        {
            get
            {
                bool zoomIn = Input.GetKey(zoomInKey);
                bool zoomOut = Input.GetKey(zoomOutKey);
                if (zoomIn && zoomOut) return 0;
                else if (!zoomIn && zoomOut) return 1;
                else if (zoomIn && !zoomOut) return -1;
                else return 0;
            }
        }

        private int RotationDirection
        {
            get
            {
                bool rotateRight = Input.GetKey(rotateRightKey);
                bool rotateLeft = Input.GetKey(rotateLeftKey);
                if (rotateLeft && rotateRight) return 0;
                else if (rotateLeft && !rotateRight) return -1;
                else if (!rotateLeft && rotateRight) return 1;
                else return 0;
            }
        }

        private void Start()
        {
            m_Transform = transform;
            cam = GetComponent<Camera>();

            if (cam != null)
            {
                cam.orthographic = true;
                zoomPos = Mathf.InverseLerp(minZoom, maxZoom, initialOrthographicSize);
                cam.orthographicSize = initialOrthographicSize;
            }
        }

        private void Update()
        {
            if (!useFixedUpdate)
                CameraUpdate();
        }

        private void FixedUpdate()
        {
            if (useFixedUpdate)
                CameraUpdate();
        }

        private void CameraUpdate()
        {
            Move();
            AdjustZoom();
            Rotate();
            LimitPosition();
        }

        private void Move()
        {
            float moveSpeed = baseMovementSpeed * (minZoom / cam.orthographicSize);

            if (useKeyboardInput)
            {
                Vector3 desiredMove = new Vector3(KeyboardInput.x, 0, KeyboardInput.y);
                desiredMove *= moveSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * desiredMove;
                desiredMove = m_Transform.InverseTransformDirection(desiredMove);
                m_Transform.Translate(desiredMove, Space.Self);
            }

            if (useScreenEdgeInput)
            {
                Vector3 desiredMove = new Vector3();
                Rect leftRect = new Rect(0, 0, screenEdgeBorder, Screen.height);
                Rect rightRect = new Rect(Screen.width - screenEdgeBorder, 0, screenEdgeBorder, Screen.height);
                Rect upRect = new Rect(0, Screen.height - screenEdgeBorder, Screen.width, screenEdgeBorder);
                Rect downRect = new Rect(0, 0, Screen.width, screenEdgeBorder);

                desiredMove.x = leftRect.Contains(MouseInput) ? -1 : rightRect.Contains(MouseInput) ? 1 : 0;
                desiredMove.z = upRect.Contains(MouseInput) ? 1 : downRect.Contains(MouseInput) ? -1 : 0;

                desiredMove *= moveSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * desiredMove;
                desiredMove = m_Transform.InverseTransformDirection(desiredMove);
                m_Transform.Translate(desiredMove, Space.Self);
            }
            
            if (usePanning && Input.GetKey(panningKey) && MouseAxis != Vector2.zero)
            {
                Vector3 desiredMove = new Vector3(-MouseAxis.x, 0, -MouseAxis.y);
                desiredMove *= panningSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * desiredMove;
                desiredMove = m_Transform.InverseTransformDirection(desiredMove);
                m_Transform.Translate(desiredMove, Space.Self);
            }
        }


        private void AdjustZoom()
        {
            if (useScrollwheelZooming)
                zoomPos += ScrollWheel * Time.deltaTime * scrollWheelZoomingSensitivity;
            if (useKeyboardZooming)
                zoomPos += ZoomDirection * Time.deltaTime * keyboardZoomingSensitivity;

            zoomPos = Mathf.Clamp01(zoomPos);

            float targetZoom = Mathf.Lerp(minZoom, maxZoom, zoomPos);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomDampening);
        }
        
        public void ResetOrthographicSize()
        {
            if (cam != null)
            {
                zoomPos = Mathf.InverseLerp(minZoom, maxZoom, initialOrthographicSize);
                cam.orthographicSize = initialOrthographicSize;
            }
        }

        private void Rotate()
        {
            if (useKeyboardRotation)
                transform.Rotate(Vector3.up, RotationDirection * Time.deltaTime * rotationSpeed, Space.World);

            if (useMouseRotation && Input.GetKey(mouseRotationKey))
                m_Transform.Rotate(Vector3.up, MouseAxis.x * Time.deltaTime * mouseRotationSpeed, Space.World); // Инверсия вращения
        }

        private void LimitPosition()
        {
            if (!limitMap)
                return;

            m_Transform.position = new Vector3(
                Mathf.Clamp(m_Transform.position.x, -limitX, limitX),
                m_Transform.position.y,
                Mathf.Clamp(m_Transform.position.z, -limitY, limitY)
            );
        }
    }
}
