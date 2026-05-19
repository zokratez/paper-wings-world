using UnityEngine;
using UnityEditor;
using PaperWings.Backend;
using System.IO;

namespace PaperWings.Editor
{
    /// <summary>
    /// One-click setup for Phase 5 Supabase + Monetization foundation.
    /// Creates the required SupabaseConfig asset with placeholder values.
    /// </summary>
    public static class SetupPhase5Backend
    {
        private const string ConfigPath = "Assets/ScriptableObjects/SupabaseConfig.asset";

        [MenuItem("Paper Wings/Phase 5 - Create Supabase Config Asset")]
        public static void CreateSupabaseConfigAsset()
        {
            // Ensure folder exists
            string folder = Path.GetDirectoryName(ConfigPath);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
                AssetDatabase.Refresh();
            }

            if (AssetDatabase.LoadAssetAtPath<SupabaseConfig>(ConfigPath) != null)
            {
                Debug.LogWarning($"SupabaseConfig already exists at {ConfigPath}. Delete it first if you want to recreate.");
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<SupabaseConfig>(ConfigPath);
                return;
            }

            var config = ScriptableObject.CreateInstance<SupabaseConfig>();
            config.supabaseUrl = "https://YOUR-PROJECT.supabase.co";
            config.anonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."; // Paste your anon key here after creation

            AssetDatabase.CreateAsset(config, ConfigPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("✅ Phase 5 SupabaseConfig asset created!\n" +
                      "1. Open it in the Inspector.\n" +
                      "2. Fill in your real Supabase Project URL and anon public key (from Supabase Dashboard → Settings → API).\n" +
                      "3. (Optional) Drag it into the FoldingDemoBootstrap in your scene for wiring.");

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<SupabaseConfig>(ConfigPath);
        }
    }
}