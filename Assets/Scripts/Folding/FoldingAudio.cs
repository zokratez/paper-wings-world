using UnityEngine;

namespace PaperWings.Folding
{
    /// <summary>
    /// Lightweight audio player for folding feedback.
    /// Placeholder clips can be replaced with real SFX later.
    /// </summary>
    public class FoldingAudio : MonoBehaviour
    {
        [Header("Audio Sources")]
        public AudioSource whooshSource;   // Paper folding movement
        public AudioSource successSource;  // Step complete / ding

        [Header("Placeholder Clips (assign in inspector or leave null for built-in tones)")]
        public AudioClip foldWhoosh;
        public AudioClip stepComplete;

        public void PlayFoldSound(int intensity = 1)
        {
            if (whooshSource == null) return;

            if (foldWhoosh != null)
            {
                float volume = Mathf.Clamp(0.5f + (intensity * 0.15f), 0.5f, 1.0f);
                whooshSource.PlayOneShot(foldWhoosh, volume);
            }
            else
            {
                // Varied synthesized paper fold sound based on step intensity
                float frequency = 650f + (intensity * 120f);
                float volume = 0.55f + (intensity * 0.12f);
                AudioSource.PlayClipAtPoint(GenerateTone(frequency, 0.18f), transform.position, volume);
            }
        }

        public void PlaySuccessSound()
        {
            if (successSource == null) return;

            if (stepComplete != null)
            {
                successSource.PlayOneShot(stepComplete, 0.85f);
            }
            else
            {
                AudioSource.PlayClipAtPoint(GenerateTone(1200, 0.25f), transform.position, 0.8f);
            }
        }

        // Very basic synthesized tone for placeholder use (no external assets required)
        private AudioClip GenerateTone(float frequency, float duration)
        {
            int sampleRate = 44100;
            int samples = Mathf.FloorToInt(sampleRate * duration);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / sampleRate;
                data[i] = Mathf.Sin(2 * Mathf.PI * frequency * t) * Mathf.Exp(-3f * t); // quick decay
            }

            AudioClip clip = AudioClip.Create("Tone", samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        /// <summary>
        /// Kid-friendly chime for new personal bests (higher, celebratory tone)
        /// </summary>
        public void PlayNewBestSound()
        {
            AudioSource.PlayClipAtPoint(GenerateTone(1400, 0.35f), transform.position, 0.9f);
            // Play a quick follow-up note for "sparkle" feel
            AudioSource.PlayClipAtPoint(GenerateTone(1750, 0.2f), transform.position, 0.7f);
        }

        /// <summary>
        /// Pleasant unlock sound for new badges / mastery tiers
        /// </summary>
        public void PlayBadgeUnlockSound()
        {
            AudioSource.PlayClipAtPoint(GenerateTone(950, 0.4f), transform.position, 0.85f);
        }

        /// <summary>
        /// Satisfying launch sound when transitioning from folding success to flight.
        /// </summary>
        public void PlayLaunchSound()
        {
            if (successSource != null && stepComplete != null)
            {
                successSource.PlayOneShot(stepComplete, 1.0f);
            }
            else
            {
                // Big celebratory whoosh + rising tone for launch feel
                AudioSource.PlayClipAtPoint(GenerateTone(650, 0.5f), transform.position, 0.9f);
                AudioSource.PlayClipAtPoint(GenerateTone(1100, 0.35f), transform.position, 0.75f);
            }
        }
    }
}