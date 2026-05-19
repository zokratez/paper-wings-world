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

            Debug.Log(@"✅ Phase 5 SupabaseConfig asset created at Assets/ScriptableObjects/SupabaseConfig.asset

EXACT NEXT STEPS (user has Supabase account):

1. Go to https://supabase.com/dashboard and create a new project (or use existing).
2. In your project:
   - Left sidebar → Settings (gear icon) → API
   - Copy 'Project URL' (e.g. https://xyzabc123.supabase.co)
   - Copy the 'anon public' key (long JWT starting with eyJ...)
3. In Unity, select the new SupabaseConfig.asset in Project window.
4. Paste the URL into 'Supabase Url' field.
5. Paste the anon key into 'Anon Key' field.
6. Recommended: Copy the .asset file into Assets/Resources/ (create the folder) and name it exactly 'SupabaseConfig.asset' so the demo auto-wires on Play.

The asset is now ready for the Dev Tools panel on the Hub.");

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<SupabaseConfig>(ConfigPath);
        }
    }
}