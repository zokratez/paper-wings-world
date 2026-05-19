using UnityEngine;
using UnityEngine.EventSystems;

namespace PaperWings.Folding
{
    /// <summary>
    /// Handles intuitive touch orbit + pinch zoom on the 3D paper model.
    /// Designed for tablets (iPad + high-end Android).
    /// 
    /// Controls:
    /// - One finger drag: Orbit horizontally and vertically
    /// - Two finger pinch: Zoom in/out
    /// - Two finger twist (optional): Roll (can be disabled)
    /// </summary>
    public class PaperModelOrbitController : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;           // The paper model root
        [SerializeField] private Transform cameraPivot;      // Empty object the camera orbits around

        [Header("Orbit Settings")]
        [SerializeField] private float orbitSpeed = 0.4f;
        [SerializeField] private float verticalSpeed = 0.35f;
        [SerializeField] private float minVerticalAngle = -60f;
        [SerializeField] private float maxVerticalAngle = 75f;

        [Header("Zoom Settings (Tablet Friendly)")]
        [SerializeField] private float zoomSpeed = 0.08f;
        [SerializeField] private float minDistance = 1.2f;
        [SerializeField] private float maxDistance = 5.5f;
        [SerializeField] private float defaultDistance = 2.8f;

        private Camera cam;
        private float currentX = 0f;
        private float currentY = 25f;
        private float currentDistance;

        private Vector2 lastSingleTouchPosition;
        private float lastPinchDistance;

        private void Awake()
        {
            cam = GetComponent<Camera>();
            if (cam == null) cam = Camera.main;

            if (cameraPivot == null)
                cameraPivot = transform.parent;

            currentDistance = defaultDistance;
        }

        private void LateUpdate()
        {
            if (target == null || cameraPivot == null) return;

            HandleTouchInput();
            UpdateCameraPosition();
        }

        private void HandleTouchInput()
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                // Ignore if touching UI
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    return;

                if (touch.phase == TouchPhase.Began)
                {
                    lastSingleTouchPosition = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    Vector2 delta = touch.position - lastSingleTouchPosition;

                    currentX += delta.x * orbitSpeed;
                    currentY -= delta.y * verticalSpeed;   // Invert Y for natural feel

                    currentY = Mathf.Clamp(currentY, minVerticalAngle, maxVerticalAngle);

                    lastSingleTouchPosition = touch.position;
                }
            }
            else if (Input.touchCount == 2)
            {
                Touch t0 = Input.GetTouch(0);
                Touch t1 = Input.GetTouch(1);

                // Pinch zoom
                float currentPinchDistance = Vector2.Distance(t0.position, t1.position);

                if (t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began)
                {
                    lastPinchDistance = currentPinchDistance;
                }
                else if (t0.phase == TouchPhase.Moved || t1.phase == TouchPhase.Moved)
                {
                    float deltaPinch = currentPinchDistance - lastPinchDistance;
                    currentDistance -= deltaPinch * zoomSpeed;
                    currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

                    lastPinchDistance = currentPinchDistance;
                }
            }
        }

        private void UpdateCameraPosition()
        {
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
            Vector3 position = rotation * new Vector3(0, 0, -currentDistance) + cameraPivot.position;

            transform.position = position;
            transform.LookAt(cameraPivot.position + Vector3.up * 0.15f); // Slight upward look for better paper view
        }

        /// <summary>
        /// Call this when loading a new plane model to reset the view nicely.
        /// </summary>
        public void ResetView(float preferredDistance = -1f)
        {
            currentX = 25f;
            currentY = 28f;
            currentDistance = preferredDistance > 0 ? preferredDistance : defaultDistance;
        }
    }
}