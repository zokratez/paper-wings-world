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

        public void PlayFoldSound()
        {
            if (whooshSource == null) return;

            if (foldWhoosh != null)
            {
                whooshSource.PlayOneShot(foldWhoosh, 0.7f);
            }
            else
            {
                // Simple placeholder tone
                AudioSource.PlayClipAtPoint(GenerateTone(800, 0.15f), transform.position, 0.6f);
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
    }
}