using UnityEngine;

namespace PaperWings.Flight
{
    /// <summary>
    /// Audio layer for the flight experience.
    /// Handles dynamic wind/whoosh that scales with speed + region ambient feel.
    /// Uses synthesized fallbacks for now (easy to replace with real clips later).
    /// </summary>
    public class FlightAudio : MonoBehaviour
    {
        [Header("References")]
        public Transform planeTransform;           // The flying paper plane
        public Rigidbody planeRigidbody;

        [Header("Settings")]
        public float minSpeedForWhoosh = 8f;
        public float maxSpeedForWhoosh = 35f;

        private AudioSource windSource;
        private float targetVolume = 0f;
        private float currentVolume = 0f;

        private void Awake()
        {
            windSource = gameObject.AddComponent<AudioSource>();
            windSource.loop = true;
            windSource.playOnAwake = false;
            windSource.spatialBlend = 0f; // 2D for simplicity in prototype
        }

        private void Start()
        {
            // Start with a soft wind loop using synthesized tone
            windSource.clip = GenerateWindLoopClip();
            windSource.volume = 0f;
            windSource.Play();
        }

        private void Update()
        {
            if (planeRigidbody == null || planeTransform == null) return;

            float speed = planeRigidbody.velocity.magnitude;
            float speed01 = Mathf.InverseLerp(minSpeedForWhoosh, maxSpeedForWhoosh, speed);

            // Target volume based on speed
            targetVolume = Mathf.Lerp(0.15f, 0.85f, speed01);

            // Smooth volume
            currentVolume = Mathf.Lerp(currentVolume, targetVolume, Time.deltaTime * 4f);
            windSource.volume = currentVolume;

            // Slight pitch variation for "gust" feel
            windSource.pitch = Mathf.Lerp(0.92f, 1.08f, speed01 + (Mathf.Sin(Time.time * 1.5f) * 0.03f));
        }

        /// <summary>
        /// Simple synthesized wind loop (no external assets required).
        /// In a real build this would be replaced by a proper wind sample.
        /// </summary>
        private AudioClip GenerateWindLoopClip()
        {
            int sampleRate = 44100;
            float duration = 2.5f;
            int samples = Mathf.FloorToInt(sampleRate * duration);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / sampleRate;
                // Low rumble + higher air noise
                float low = Mathf.Sin(2 * Mathf.PI * 85f * t) * 0.6f;
                float mid = Mathf.PerlinNoise(t * 12f, 0) * 0.4f;
                float high = (Mathf.PerlinNoise(t * 28f, 3.7f) - 0.5f) * 0.35f;

                data[i] = (low + mid + high) * 0.7f;
            }

            AudioClip clip = AudioClip.Create("WindLoop", samples, 1, sampleRate, true);
            clip.SetData(data, 0);
            return clip;
        }

        /// <summary>
        /// Call this when the plane is launched to give an initial strong whoosh.
        /// </summary>
        public void PlayLaunchWhoosh()
        {
            AudioSource.PlayClipAtPoint(GenerateSimpleWhooshClip(), transform.position, 0.95f);
        }

        private AudioClip GenerateSimpleWhooshClip()
        {
            int sampleRate = 44100;
            float duration = 0.7f;
            int samples = Mathf.FloorToInt(sampleRate * duration);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / sampleRate;
                float freq = Mathf.Lerp(1200f, 450f, t);
                data[i] = Mathf.Sin(2 * Mathf.PI * freq * t) * Mathf.Exp(-2.2f * t) * 0.8f;
            }

            AudioClip clip = AudioClip.Create("LaunchWhoosh", samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}