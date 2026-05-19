using UnityEngine;

namespace PaperWings.Flight
{
    /// <summary>
    /// High-quality third-person follow camera for Paper Wings flight.
    /// 
    /// Features:
    /// - Smooth damped follow behind the plane
    /// - Optional free-look by dragging on the right half of the screen (very intuitive on tablets)
    /// - Automatically returns to chase view when input stops
    /// - Tuned for a fun, cinematic feel while staying readable for kids
    /// </summary>
    public class FlightCameraFollower : MonoBehaviour
    {
        public Transform target;

        [Header("Follow Settings")]
        public float followDistance = 5.2f;
        public float heightOffset = 1.8f;
        public float smoothSpeed = 4.2f;

        [Header("Free Look")]
        [Tooltip("Allow dragging on the right side of the screen for free look")]
        public bool allowFreeLook = true;
        public float freeLookSensitivity = 0.45f;

        private Vector3 velocity;
        private float currentYaw;
        private float currentPitch;
        private bool isFreeLooking = false;
        private Vector2 lastLookPosition;

        private void LateUpdate()
        {
            if (target == null) return;

            HandleFreeLookInput();

            // Calculate desired position relative to plane's orientation + free look offset
            Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
            Vector3 offset = rotation * Vector3.back * followDistance + Vector3.up * heightOffset;

            Vector3 desiredPosition = target.position + offset;

            // Smooth movement
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 1f / smoothSpeed);

            // Look at a point slightly ahead of the plane for better sense of motion
            Vector3 lookTarget = target.position + target.forward * 3.2f + Vector3.up * 0.7f;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookTarget - transform.position), Time.deltaTime * 7f);
        }

        private void HandleFreeLookInput()
        {
            if (!allowFreeLook) return;

            bool rightSideActive = false;
            Vector2 currentTouch = Vector2.zero;

            // Single finger on right half of screen
            if (Input.touchCount == 1)
            {
                Touch t = Input.GetTouch(0);
                currentTouch = t.position;

                if (currentTouch.x > Screen.width * 0.5f &&
                    (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary))
                {
                    rightSideActive = true;
                }
            }
            // Two fingers anywhere = free look (more generous)
            else if (Input.touchCount == 2)
            {
                rightSideActive = true;
                currentTouch = (Input.GetTouch(0).position + Input.GetTouch(1).position) / 2f;
            }

            if (rightSideActive)
            {
                isFreeLooking = true;

                if (Input.GetTouch(0).phase == TouchPhase.Began ||
                    (Input.touchCount == 2 && (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(1).phase == TouchPhase.Began)))
                {
                    lastLookPosition = currentTouch;
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.touchCount == 2)
                {
                    Vector2 delta = currentTouch - lastLookPosition;

                    currentYaw += delta.x * freeLookSensitivity;
                    currentPitch -= delta.y * freeLookSensitivity * 0.6f;
                    currentPitch = Mathf.Clamp(currentPitch, -42f, 55f);

                    lastLookPosition = currentTouch;
                }
            }
            else
            {
                isFreeLooking = false;

                // Smoothly recenter behind the plane
                currentYaw = Mathf.LerpAngle(currentYaw, 0f, Time.deltaTime * 3.8f);
                currentPitch = Mathf.Lerp(currentPitch, 7f, Time.deltaTime * 3.8f);
            }
        }

        /// <summary>
        /// Triggers a subtle screen shake (used for strong launches or thermals).
        /// Kid-friendly — short and gentle.
        /// </summary>
        public void Shake(float intensity = 0.6f, float duration = 0.35f)
        {
            StartCoroutine(DoShake(intensity, duration));
        }

        private System.Collections.IEnumerator DoShake(float intensity, float duration)
        {
            float elapsed = 0f;
            Vector3 originalPosition = transform.localPosition;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float strength = intensity * (1 - (elapsed / duration));

                float x = Random.Range(-1f, 1f) * strength * 0.08f;
                float y = Random.Range(-1f, 1f) * strength * 0.08f;

                transform.localPosition = originalPosition + new Vector3(x, y, 0);

                yield return null;
            }

            transform.localPosition = originalPosition;
        }
    }
}