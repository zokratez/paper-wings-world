using UnityEngine;

namespace PaperWings.Flight
{
    /// <summary>
    /// Visual effects for flight — paper flutter particles, simple environment polish.
    /// Keeps things lightweight for mobile.
    /// </summary>
    public class FlightEffects : MonoBehaviour
    {
        [Header("References")]
        public Transform planeTransform;
        public Rigidbody planeRigidbody;

        [Header("Flutter Particles")]
        public ParticleSystem flutterParticles;
        public float minSpeedForFlutter = 12f;

        private ParticleSystem.EmissionModule emissionModule;

        private void Start()
        {
            if (flutterParticles == null)
            {
                flutterParticles = CreateFlutterParticleSystem();
            }

            if (flutterParticles != null)
            {
                emissionModule = flutterParticles.emission;
                emissionModule.rateOverTime = 0;
            }
        }

        /// <summary>
        /// Creates a simple lightweight paper flutter particle system at runtime.
        /// </summary>
        private ParticleSystem CreateFlutterParticleSystem()
        {
            GameObject psGO = new GameObject("PaperFlutterParticles");
            psGO.transform.SetParent(transform);
            psGO.transform.localPosition = Vector3.zero;

            var ps = psGO.AddComponent<ParticleSystem>();

            var main = ps.main;
            main.startLifetime = 0.8f;
            main.startSpeed = 2.5f;
            main.startSize = 0.12f;
            main.startColor = new Color(0.95f, 0.93f, 0.88f, 0.7f);
            main.maxParticles = 50;

            var emission = ps.emission;
            emission.rateOverTime = 0;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.4f;

            var velocity = ps.velocityOverLifetime;
            velocity.enabled = true;
            velocity.x = new ParticleSystem.MinMaxCurve(-1.5f, 1.5f);
            velocity.y = new ParticleSystem.MinMaxCurve(-0.8f, 0.8f);

            return ps;
        }

        private void Update()
        {
            if (planeRigidbody == null || flutterParticles == null) return;

            float speed = planeRigidbody.velocity.magnitude;
            bool shouldEmit = speed > minSpeedForFlutter;

            emissionModule.rateOverTime = shouldEmit ? Mathf.Lerp(8, 35, (speed - minSpeedForFlutter) / 20f) : 0;
        }

        /// <summary>
        /// Call this on launch for a nice burst of paper particles.
        /// </summary>
        public void PlayLaunchBurst()
        {
            if (flutterParticles != null)
            {
                var burst = new ParticleSystem.Burst(0f, 25);
                flutterParticles.emission.SetBurst(0, burst);
                flutterParticles.Play();
            }
        }
    }
}