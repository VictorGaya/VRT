using _Project.Ray_Tracer.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;
using _Project.Ray_Tracer.Scripts.RT_Scene;

namespace _Project.Scripts
{
    /// <summary>
    /// A camera controller that mimics the controls and behavior of the Unity editor camera. Adapted from
    /// http://wiki.unity3d.com/index.php?title=MouseOrbitZoom.
    /// </summary>
    public class UVCameraController : MonoBehaviour
    {
        [SerializeField]
        private RectTransform inputBlocker;
        public bool InputBlockerHovered { get; set; }
    
        public Transform Target;
        public float InitialDistance = 5.0f;
        public float MaxDistance = 25.0f;
        public float MinDistance = 0.5f;
        public float YMaxLimit = 90.0f;
        public float YMinLimit = -90.0f;
        public float OrbitSpeed = 4.0f;
        public float ZoomSpeed = 1.0f;

        private float xDegrees = 0.0f;
        private float yDegrees = 0.0f;
        private float distance = 0.0f;
    
        private bool zoom = false;
        private bool orbiting = false;

        void Start() { Initialize(); }

        /// <summary>
        /// Initialize this camera controller. Will rotate the camera to look at the provided <see cref="Target"/>. If no
        /// target is provided a new target will be placed <see cref="InitialDistance"/> in front of the camera.
        /// </summary>
        public void Initialize()
        {
            // If there is no target, create a target at the given initial distance from the camera's current viewpoint.
            Target = RTSceneManager.Get().Scene.Meshes[0].transform;

            // Store the distance to the target and camera rotation.
            distance = Vector3.Distance(transform.position, Target.position);
            xDegrees = transform.rotation.eulerAngles.y;
            yDegrees = transform.rotation.eulerAngles.x;

            // Calculate the initial position based on our rotation and the distance to the target.
            transform.position = Target.position - (transform.rotation * Vector3.forward * distance);
        }

        public void SetCursor()
        {
            if(zoom || orbiting)
                return;
            GlobalManager.Get().SetCursor(CursorType.ModeCursor);
        }

        public void ResetCursor()
        {
            if(zoom || orbiting)
                return;
        
            GlobalManager.Get().ResetCursor();
        }

        private void DisableBlocker()
        {
            GlobalManager globalSettings = GlobalManager.Get();
            globalSettings.ResetCursor();
            inputBlocker.gameObject.SetActive(false);
            InputBlockerHovered = false;
        }

        private void OrbitingUpdate()
        {
            float xDistance = 0.0f;
            float yDistance = 0.0f;

            if (Input.GetMouseButton(0))
            {
                xDistance += Input.GetAxis("Mouse X") * OrbitSpeed;
                yDistance -= Input.GetAxis("Mouse Y") * OrbitSpeed;
            }

            xDegrees += xDistance;
            yDegrees += yDistance;

            // Clamp the vertical axis for the orbit.
            yDegrees = ClampAngle(yDegrees, YMinLimit, YMaxLimit);

            // Set desired camera rotation.
            Quaternion desiredRotation = Quaternion.Euler(yDegrees, xDegrees, 0);

            // Linearly interpolate camera rotation to desired rotation. I have no idea why, but just setting the
            // rotation to desired does not work (its not normalization). Lerp seems to produce a quaternion with the
            // signs of the coordinates flipped.
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, 1.0f);

            if (!(Input.GetMouseButton(0)))
            {
                orbiting = false;
                DisableBlocker();
            }

        }
    
        private void OnlyOneInputPicker()
        {

            // if (zoom)
            // {
            //     distance += Input.GetAxis("Mouse Y") * ZoomSpeed * 0.125f * Mathf.Abs(distance);

            //     if (!Input.GetMouseButton(1))
            //     {
            //         zoom = false;
            //         DisableBlocker();
            //     }

            //     return;
            // }

            if (orbiting)
            {
                OrbitingUpdate();
                return;
            }

            if (EventSystem.current.IsPointerOverGameObject() && !InputBlockerHovered)
                return;

            // The inputs are below this line
        
            // If scrollWheel is used change zoom. This one is not exclusive.
            distance -= Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed * Mathf.Abs(distance);

            // The left mouse button or Arrow keys we activate orbiting.
            if (Input.GetMouseButtonDown(0))
            {
                orbiting = true;
                GlobalManager.Get().SetCursor(CursorType.RotateCursor);
                return;
            }
        }

        void Update()
        {        
            // If we are over the ui we don't allow the user to start any of these actions.
            OnlyOneInputPicker();

            // TODO Allow for free zoom movement not clamped and not as a function of distance just a fixed step amount.
            // TODO Add function to focus on objects and have zoom like it is now.
            // check if distance is still within it's bounds
            distance = Mathf.Clamp(distance, MinDistance, MaxDistance);

            // Update the position based on our rotation and the distance to the target.
            transform.position = Target.position - (transform.rotation * Vector3.forward * distance);

        }

        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }
    }
}
