using UnityEngine;

namespace PaperWings.Backend
{
    /// <summary>
    /// Holds Supabase project credentials.
    /// Create one asset via Assets > Create > Paper Wings > Supabase Config
    /// Never commit real keys to git — use build-time injection or remote config in production.
    /// </summary>
    [CreateAssetMenu(fileName = "SupabaseConfig", menuName = "Paper Wings / Supabase Config")]
    public class SupabaseConfig : ScriptableObject
    {
        [Header("Supabase Project")]
        [Tooltip("Your project URL, e.g. https://xyz.supabase.co")]
        public string supabaseUrl;

        [Tooltip("The 'anon' / public key from Settings → API")]
        public string anonKey;

        public string AuthUrl => $"{supabaseUrl}/auth/v1";
        public string RestUrl => $"{supabaseUrl}/rest/v1";
    }
}