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
        private bool isMobile;

        private void Start()
        {
            isMobile = SystemInfo.deviceType == DeviceType.Handheld;

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
            main.maxParticles = isMobile ? 25 : 50;

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
            float mobileMult = isMobile ? 0.55f : 1f;

            // Paper flutter (always paper-like)
            if (flutterParticles != null && emissionModule.enabled)
            {
                float r = fast ? Mathf.Lerp(12, 45, (speed - minSpeedForFlutter) / 25f) : 0;
                emissionModule.rateOverTime = r * mobileMult;
            }

            // Speed trail / contrail when very fast
            if (speedTrail != null && trailEmission.enabled)
            {
                float trailRate = Mathf.Lerp(0, 60, (speed - 18f) / 20f);
                trailEmission.rateOverTime = Mathf.Max(0, trailRate) * mobileMult;
            }

            // Region-specific ambient particles (always on, low rate)
            if (regionParticles != null && regionEmission.enabled)
            {
                float rate = 4f;

                // Grand Canyon: more dust when flying low
                if (currentRegion != null && currentRegion.regionId == "grand_canyon" && planeTransform != null)
                {
                    float height = planeTransform.position.y;
                    if (height < 15f)
                    {
                        rate = Mathf.Lerp(4f, 18f, (15f - height) / 15f);
                    }
                }

                regionEmission.rateOverTime = rate * mobileMult;
            }
        }

        /// <summary>
        /// Call this on launch for a nice burst of paper particles.
        /// </summary>
        public void PlayLaunchBurst()
        {
            if (flutterParticles != null)
            {
                // Bigger, more dramatic launch burst (reduced on mobile for perf)
                var main = flutterParticles.main;
                main.startSize = 0.25f;
                int burstCount = isMobile ? 20 : 40;
                var burst = new ParticleSystem.Burst(0f, burstCount, 1, 0.1f);
                flutterParticles.emission.SetBurst(0, burst);
                flutterParticles.Play();

                // Also boost the speed trail briefly if available
                if (speedTrail != null)
                {
                    var trailMain = speedTrail.main;
                    trailMain.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.25f);
                }
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
            main.maxParticles = isMobile ? 40 : 80;

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
            main.maxParticles = isMobile ? 15 : 30;
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
                    // Light dust when low - rate modulated in Update
                    break;

                case "fuji_foothills":
                    main.startColor = new Color(0.95f, 0.75f, 0.85f, 0.65f); // soft pink/green leaves/petals
                    main.startSize = new ParticleSystem.MinMaxCurve(0.12f, 0.22f);
                    // Gentle falling effect
                    velocity.y = new ParticleSystem.MinMaxCurve(-1.2f, 0.3f);
                    break;

                case "norwegian_coast":
                    main.startColor = new Color(0.85f, 0.92f, 1f, 0.5f); // sea spray + light snow/fog
                    main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.2f);
                    // More horizontal spread for spray
                    velocity.x = new ParticleSystem.MinMaxCurve(-2.5f, 2.5f);
                    velocity.y = new ParticleSystem.MinMaxCurve(0.1f, 1.2f);
                    break;

                default:
                    main.startColor = new Color(0.8f, 0.8f, 0.75f, 0.4f);
                    break;
            }

            return ps;
        }
    }
}