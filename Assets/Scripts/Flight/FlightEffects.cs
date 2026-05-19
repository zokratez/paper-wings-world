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

        [Header("Region Effects")]
        public ParticleSystem regionParticles;   // Dust, leaves, spray, etc.
        public ParticleSystem speedTrail;        // Contrail / wind streaks

        private ParticleSystem.EmissionModule emissionModule;
        private ParticleSystem.EmissionModule regionEmission;
        private ParticleSystem.EmissionModule trailEmission;

        private FlightRegion currentRegion;

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

            if (speedTrail == null)
            {
                speedTrail = CreateSpeedTrailSystem();
            }
            if (speedTrail != null)
            {
                trailEmission = speedTrail.emission;
                trailEmission.rateOverTime = 0;
            }
        }

        public void SetRegion(FlightRegion region)
        {
            currentRegion = region;

            if (regionParticles != null)
            {
                Destroy(regionParticles.gameObject);
            }

            regionParticles = CreateRegionParticleSystem(region);
            if (regionParticles != null)
            {
                regionEmission = regionParticles.emission;
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
            if (planeRigidbody == null) return;

            float speed = planeRigidbody.velocity.magnitude;
            bool fast = speed > minSpeedForFlutter;

            // Paper flutter (always paper-like)
            if (flutterParticles != null && emissionModule.enabled)
            {
                emissionModule.rateOverTime = fast ? Mathf.Lerp(12, 45, (speed - minSpeedForFlutter) / 25f) : 0;
            }

            // Speed trail / contrail when very fast
            if (speedTrail != null && trailEmission.enabled)
            {
                float trailRate = Mathf.Lerp(0, 60, (speed - 18f) / 20f);
                trailEmission.rateOverTime = Mathf.Max(0, trailRate);
            }

            // Region-specific ambient particles (always on, low rate)
            if (regionParticles != null && regionEmission.enabled)
            {
                regionEmission.rateOverTime = 4f; // gentle constant emission
            }
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

        private ParticleSystem CreateSpeedTrailSystem()
        {
            GameObject psGO = new GameObject("SpeedTrail");
            psGO.transform.SetParent(transform);
            psGO.transform.localPosition = new Vector3(0, 0, -0.6f);

            var ps = psGO.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startLifetime = 0.6f;
            main.startSpeed = 0.1f;
            main.startSize = new ParticleSystem.MinMaxCurve(0.06f, 0.14f);
            main.startColor = new Color(1f, 1f, 1f, 0.35f);
            main.maxParticles = 80;

            var emission = ps.emission;
            emission.rateOverTime = 0;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 8f;
            shape.radius = 0.15f;

            var velocity = ps.velocityOverLifetime;
            velocity.enabled = true;
            velocity.z = new ParticleSystem.MinMaxCurve(-4f, -1f);

            return ps;
        }

        private ParticleSystem CreateRegionParticleSystem(FlightRegion region)
        {
            if (region == null) return null;

            GameObject psGO = new GameObject($"RegionParticles_{region.regionId}");
            psGO.transform.SetParent(transform);
            psGO.transform.localPosition = Vector3.zero;

            var ps = psGO.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startLifetime = 2.2f;
            main.startSpeed = 1.8f;
            main.maxParticles = 30;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 3;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 1.2f;

            var velocity = ps.velocityOverLifetime;
            velocity.enabled = true;
            velocity.x = new ParticleSystem.MinMaxCurve(-0.8f, 0.8f);
            velocity.y = new ParticleSystem.MinMaxCurve(0.3f, 1.8f);

            var color = ps.colorOverLifetime;
            color.enabled = true;

            switch (region.regionId)
            {
                case "grand_canyon":
                    main.startColor = new Color(0.85f, 0.7f, 0.5f, 0.6f); // warm sand/dust
                    main.startSize = 0.18f;
                    break;

                case "fuji_foothills":
                    main.startColor = new Color(0.95f, 0.75f, 0.85f, 0.65f); // soft pink/green leaves
                    main.startSize = new ParticleSystem.MinMaxCurve(0.12f, 0.22f);
                    break;

                case "norwegian_coast":
                    main.startColor = new Color(0.9f, 0.95f, 1f, 0.55f); // sea spray / light mist
                    main.startSize = 0.15f;
                    break;

                default:
                    main.startColor = new Color(0.8f, 0.8f, 0.75f, 0.4f);
                    break;
            }

            return ps;
        }
    }
}