using UnityEngine;

namespace PaperWings.Flight
{
    /// <summary>
    /// Defines a volume of rising air (thermal).
    /// When the plane enters, it receives a gentle upward force.
    /// Perfect for fun, rewarding gliding in the canyon.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ThermalZone : MonoBehaviour
    {
        [Header("Thermal Properties")]
        [Tooltip("Upward force applied to the plane")]
        public float upwardForce = 6.5f;

        [Tooltip("How strongly the thermal affects planes with different windSensitivity")]
        public float sensitivityMultiplier = 1.0f;

        [Tooltip("Radius of influence (if using trigger)")]
        public float radius = 15f;

        private void OnTriggerStay(Collider other)
        {
            var physics = other.GetComponent<PaperAirplanePhysics>();
            if (physics == null) return;

            // Apply upward lift scaled by the plane's windSensitivity (or stability)
            float force = upwardForce * physics.windSensitivity * sensitivityMultiplier;

            // Make it slightly stronger in the center of the thermal
            float distanceFactor = 1f;
            if (other.attachedRigidbody != null)
            {
                float dist = Vector3.Distance(transform.position, other.attachedRigidbody.worldCenterOfMass);
                distanceFactor = Mathf.InverseLerp(radius, 0f, dist);
            }

            other.attachedRigidbody.AddForce(Vector3.up * force * distanceFactor, ForceMode.Acceleration);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.3f);
            Gizmos.DrawSphere(transform.position, radius);
        }
    }
}