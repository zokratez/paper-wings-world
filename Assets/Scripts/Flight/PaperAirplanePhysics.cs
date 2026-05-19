using UnityEngine;

namespace PaperWings.Flight
{
    /// <summary>
    /// High-quality, kid-friendly paper airplane physics.
    /// Based on real aerodynamics (lift, drag, torque, stability) but heavily tuned for fun and forgiveness.
    /// 
    /// Designed to work with any PaperPlaneDefinition.FlightCharacteristics.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PaperAirplanePhysics : MonoBehaviour
    {
        [Header("Flight Tuning (from PaperPlaneDefinition)")]
        public float baseSpeed = 1.0f;
        public float glideEfficiency = 1.0f;
        public float stability = 1.0f;
        public float windSensitivity = 1.0f;

        [Header("Physics Parameters")]
        [Tooltip("Base lift coefficient. Higher = glides better.")]
        public float liftCoefficient = 0.8f;

        [Tooltip("Drag coefficient. Higher = slows down faster.")]
        public float dragCoefficient = 0.35f;

        [Tooltip("How strongly the plane tries to level itself (stability).")]
        public float autoLevelStrength = 2.5f;

        [Header("Wind")]
        public Vector3 windDirection = new Vector3(0.3f, 0, 0.1f);
        public float baseWindStrength = 0.8f;

        [Header("Forgiving Settings (Kid-Friendly)")]
        [Tooltip("Minimum speed before physics become very gentle (prevents frustrating stalls)")]
        public float minimumSpeed = 3.5f;

        [Tooltip("Height below which the plane gets gentle upward assistance (prevents crashing into ground)")]
        public float recoveryHeight = 8f;

        [Tooltip("How strong the automatic recovery lift is")]
        public float recoveryLiftStrength = 4.5f;

        private Rigidbody rb;
        private float currentSpeedMultiplier = 1f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.drag = 0.05f;           // light air resistance
            rb.angularDrag = 0.8f;     // helps with stability
        }

        /// <summary>
        /// Call this right after spawning the plane from the folding screen.
        /// </summary>
        public void InitializeFromDefinition(PaperWings.Folding.PaperPlaneDefinition definition)
        {
            if (definition == null || definition.flightData == null) return;

            var data = definition.flightData;

            baseSpeed = data.baseSpeed;
            glideEfficiency = data.glideEfficiency;
            stability = data.stability;
            windSensitivity = data.windSensitivity;

            // Scale auto-level with stability
            autoLevelStrength = 1.8f + (stability - 1f) * 1.2f;
        }

        private void FixedUpdate()
        {
            if (rb == null) return;

            Vector3 velocity = rb.velocity;
            float speed = velocity.magnitude;

            // --- Lift (main gliding force) ---
            // Lift is strongest when moving forward and slightly nose-up
            float forwardSpeed = Vector3.Dot(velocity, transform.forward);
            float angleOfAttack = Vector3.Angle(transform.forward, velocity.normalized) * 0.02f;

            float lift = forwardSpeed * liftCoefficient * glideEfficiency * (1f + angleOfAttack);
            Vector3 liftForce = transform.up * lift * currentSpeedMultiplier;
            rb.AddForce(liftForce, ForceMode.Force);

            // --- Drag (opposes motion) ---
            float drag = speed * dragCoefficient / Mathf.Max(speed, 1f);
            Vector3 dragForce = -velocity.normalized * drag;
            rb.AddForce(dragForce, ForceMode.Force);

            // --- Gentle Auto-Level / Stability (torque) ---
            // The plane naturally wants to fly level — this is what makes it feel like a real paper plane
            Vector3 targetUp = Vector3.up;
            Vector3 currentUp = transform.up;

            float stabilityFactor = stability * 0.8f;
            Vector3 torque = Vector3.Cross(currentUp, targetUp) * autoLevelStrength * stabilityFactor;
            rb.AddTorque(torque, ForceMode.Acceleration);

            // --- Wind with gentle variation (simulates light currents) ---
            float windVariation = Mathf.Sin(Time.time * 0.4f) * 0.3f + Mathf.Sin(Time.time * 0.9f) * 0.2f;
            Vector3 currentWind = (windDirection.normalized + new Vector3(0, windVariation * 0.15f, 0)) * baseWindStrength * windSensitivity;
            rb.AddForce(currentWind, ForceMode.Force);

            // --- Kid-Friendly Forgiveness ---
            // If speed drops too low, reduce aggressive physics so it doesn't plummet immediately
            currentSpeedMultiplier = Mathf.Lerp(0.6f, 1.2f, Mathf.InverseLerp(minimumSpeed * 0.6f, minimumSpeed * 1.4f, speed));
            currentSpeedMultiplier = Mathf.Clamp(currentSpeedMultiplier, 0.55f, 1.3f);

            // Very light forward bias so it always has a little "glide energy"
            if (speed < minimumSpeed)
            {
                rb.AddForce(transform.forward * 3f, ForceMode.Acceleration);
            }

            // --- Gentle Auto-Recovery (Prevents Frustrating Ground Crashes) ---
            // When the plane gets too low, give it a soft upward boost so kids can recover easily
            if (transform.position.y < recoveryHeight && rb.velocity.y < 0.5f)
            {
                float recoveryFactor = Mathf.InverseLerp(recoveryHeight * 0.3f, recoveryHeight, transform.position.y);
                float recoveryLift = recoveryLiftStrength * (1f - recoveryFactor);

                rb.AddForce(Vector3.up * recoveryLift, ForceMode.Acceleration);

                // Also reduce downward velocity
                if (rb.velocity.y < -1f)
                {
                    Vector3 vel = rb.velocity;
                    vel.y *= 0.7f;
                    rb.velocity = vel;
                }
            }

            // --- Thermals (Rising Air Columns) ---
            // These are applied externally via ThermalZone triggers for now.
            // Future: We can add global thermal sampling here if needed.
        }

        /// <summary>
        /// Called by the flight controller when the player gives input.
        /// Gentle roll + pitch control.
        /// </summary>
        public void ApplyPlayerControl(float rollInput, float pitchInput, float strength = 1f)
        {
            if (rb == null) return;

            // Roll (banking)
            rb.AddTorque(transform.forward * rollInput * strength * 18f, ForceMode.Acceleration);

            // Pitch (nose up/down)
            rb.AddTorque(transform.right * -pitchInput * strength * 14f, ForceMode.Acceleration);
        }
    }
}