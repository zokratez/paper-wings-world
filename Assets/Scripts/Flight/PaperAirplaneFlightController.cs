using UnityEngine;
using PaperWings.Demo;

namespace PaperWings.Flight
{
    /// <summary>
    /// High-quality, kid-friendly flight controller for Paper Wings World.
    /// 
    /// Controls:
    /// - Swipe/Drag: Horizontal = banking/roll, Vertical = pitch (nose up/down)
    /// - Optional device tilt (accelerometer) for banking on tablets/phones
    /// - Very forgiving and intuitive for children
    /// </summary>
    [RequireComponent(typeof(PaperAirplanePhysics))]
    public class PaperAirplaneFlightController : MonoBehaviour
    {
        [Header("Launch Settings")]
        [Tooltip("Initial forward speed when launched from folding screen")]
        public float launchForce = 18f;
        public float launchUpAngle = 12f;

        [Header("Touch Control")]
        [Tooltip("Overall sensitivity of touch input")]
        public float controlSensitivity = 1.0f;

        [Tooltip("How quickly control input responds (higher = snappier)")]
        public float inputResponsiveness = 8f;

        [Header("Tilt Control (Optional)")]
        [Tooltip("Enable device accelerometer for banking (great on iPad)")]
        public bool useDeviceTilt = true;
        public float tiltSensitivity = 1.2f;

        [Header("Limits (Kid-Friendly)")]
        public float maxPitchInput = 1.0f;
        public float maxRollInput = 1.3f;

        private PaperAirplanePhysics physics;
        private Rigidbody rb;

        private Vector2 currentInput;       // x = roll, y = pitch
        private Vector2 targetInput;
        private Vector2 lastTouchPos;

        private bool isTouching = false;

        private void Awake()
        {
            physics = GetComponent<PaperAirplanePhysics>();
            rb = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Called when transitioning from the folding screen.
        /// Gives the plane a nice, stable launch into the air.
        /// </summary>
        public void LaunchFromFoldingScreen()
        {
            if (rb == null) return;

            Vector3 launchDirection = transform.forward + Vector3.up * Mathf.Sin(launchUpAngle * Mathf.Deg2Rad);
            launchDirection.Normalize();

            rb.velocity = launchDirection * launchForce * physics.baseSpeed;
            rb.angularVelocity = Vector3.zero;

            // Give it a tiny bit of initial stability
            transform.rotation = Quaternion.Euler(-5f, transform.eulerAngles.y, 0);
        }

        private void Update()
        {
            HandleTouchInput();
            HandleDeviceTilt();
            SmoothInput();
        }

        private void FixedUpdate()
        {
            if (physics != null && currentInput.sqrMagnitude > 0.001f)
            {
                physics.ApplyPlayerControl(
                    Mathf.Clamp(currentInput.x, -maxRollInput, maxRollInput),
                    Mathf.Clamp(currentInput.y, -maxPitchInput, maxPitchInput),
                    controlSensitivity
                );
            }

            // Subtle visual flutter feedback (if using procedural model)
            var visualModel = GetComponent<ProceduralPaperPlane>();
            if (visualModel != null && physics != null)
            {
                float speed = rb ? rb.velocity.magnitude : 10f;
                float turnRate = currentInput.x; // use roll input as proxy for turning
                visualModel.ApplyFlightVisuals(speed, turnRate);
            }
        }

        private void HandleTouchInput()
        {
            if (Input.touchCount == 0)
            {
                isTouching = false;
                targetInput = Vector2.zero;
                return;
            }

            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPos = touch.position;
                isTouching = true;
            }
            else if (touch.phase == TouchPhase.Moved && isTouching)
            {
                Vector2 delta = touch.position - lastTouchPos;

                // Horizontal drag = banking (roll)
                // Vertical drag = pitch (nose up/down)
                float roll = delta.x * 0.015f;
                float pitch = delta.y * 0.012f;

                targetInput = new Vector2(roll, pitch);
                lastTouchPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isTouching = false;
                targetInput = Vector2.zero;
            }
        }

        private void HandleDeviceTilt()
        {
            if (!useDeviceTilt) return;

            // Use device tilt for gentle banking (very natural on tablets)
            Vector3 accel = Input.acceleration;

            // Left/right tilt controls roll
            float tiltRoll = -accel.x * tiltSensitivity;

            // Blend with touch input (touch takes priority when active)
            if (!isTouching)
            {
                targetInput.x = Mathf.Lerp(targetInput.x, tiltRoll, Time.deltaTime * 3f);
            }
        }

        private void SmoothInput()
        {
            // Smooth the input so controls feel nice and not twitchy
            currentInput = Vector2.Lerp(currentInput, targetInput, Time.deltaTime * inputResponsiveness);
        }

        /// <summary>
        /// Optional external boost (can be called from UI button)
        /// </summary>
        public void GentleBoost(float amount = 1f)
        {
            if (rb != null)
            {
                rb.AddForce(transform.forward * amount * 5f * physics.baseSpeed, ForceMode.VelocityChange);
            }
        }
    }
}