using UnityEngine;
using UnityEditor;
using PaperWings.Folding;
using PaperWings.Core;
using System.Collections.Generic;

namespace PaperWings.EditorTools
{
    /// <summary>
    /// One-click generator for the 8 locked v1.0 PaperPlaneDefinition assets.
    /// Run from menu: Paper Wings > Create MVP Plane Assets
    /// </summary>
    public static class CreateMvpPlaneAssets
    {
        private const string AssetPath = "Assets/ScriptableObjects/PaperPlanes/";

        [MenuItem("Paper Wings/Create MVP Plane Assets (v1.0 Locked 8)")]
        public static void CreateAllMvpPlanes()
        {
            // Ensure folder exists
            if (!AssetDatabase.IsValidFolder(AssetPath))
            {
                System.IO.Directory.CreateDirectory(AssetPath);
                AssetDatabase.Refresh();
            }

            var library = ScriptableObject.CreateInstance<PaperPlaneLibrary>();
            var planes = new List<PaperPlaneDefinition>();

            // 1. Classic Dart - Beginner
            planes.Add(CreatePlane(
                id: "classic_dart",
                name: "Classic Dart",
                difficulty: Core.DifficultyLevel.Beginner,
                category: Core.PlaneCategory.Dart,
                isFree: true,
                shortDesc: "The timeless 5-fold schoolyard dart. The plane almost everyone folds first.",
                educational: "Instant success on the first or second try. Perfectly teaches the basics of symmetry, sharp leading edges, and how speed affects flight.",
                cultural: "The universal starter paper airplane taught to children worldwide for generations."
            ));

            // 2. The Ring - Beginner
            planes.Add(CreatePlane(
                id: "the_ring",
                name: "The Ring",
                difficulty: Core.DifficultyLevel.Beginner,
                category: Core.PlaneCategory.Ring,
                isFree: true,
                shortDesc: "Two connected rings forming a tube. One of Red Bull Paper Wings' three official folding school designs.",
                educational: "Feels almost magical. Excellent for teaching how non-traditional shapes can be more stable than 'normal' airplanes.",
                cultural: "Featured in Red Bull's global Paper Wings competition as the official hangtime/stability starter design."
            ));

            // 3. Loop Plane - Beginner
            planes.Add(CreatePlane(
                id: "loop_plane",
                name: "Loop Plane",
                difficulty: Core.DifficultyLevel.Beginner,
                category: Core.PlaneCategory.Stunt,
                isFree: true,
                shortDesc: "A simple design specifically engineered to perform clean vertical loops.",
                educational: "Provides immediate, repeatable 'wow' moments. Teaches cause-and-effect (throw angle, speed, and elevator trim).",
                cultural: "Modern design created to demonstrate that paper airplanes can do real aerobatics."
            ));

            // 4. Nakamichi Glider - Intermediate (Cultural)
            planes.Add(CreatePlane(
                id: "nakamichi_glider",
                name: "Nakamichi Glider",
                difficulty: Core.DifficultyLevel.Intermediate,
                category: Core.PlaneCategory.Glider,
                isFree: false,
                shortDesc: "A refined Japanese glider design emphasizing wide wings, slight dihedral, and a locked rear structure.",
                educational: "Feels like a real sailplane. Players learn the value of wing area and dihedral for lift.",
                cultural: "Part of the respected Nakamichi family of Japanese paper airplane designs, known worldwide."
            ));

            // 5. Stealth Glider - Intermediate
            planes.Add(CreatePlane(
                id: "stealth_glider",
                name: "Stealth Glider",
                difficulty: Core.DifficultyLevel.Intermediate,
                category: Core.PlaneCategory.Glider,
                isFree: false,
                shortDesc: "FoldNfly's top-rated design for maximum time in the air. Wide wing area optimized for slow, floating flight.",
                educational: "Rewards patience and fine-tuning. Perfect plane for the 3D globe — players will want to keep it aloft while exploring.",
                cultural: "One of the most tested and recommended gliders in the modern paper airplane community."
            ));

            // 6. Light Spinner - Intermediate (Stunt)
            planes.Add(CreatePlane(
                id: "light_spinner",
                name: "Light Spinner",
                difficulty: Core.DifficultyLevel.Intermediate,
                category: Core.PlaneCategory.Stunt,
                isFree: false,
                shortDesc: "Creates beautiful, hypnotic corkscrew spirals as it descends. Wing angle controls the tightness of the spin.",
                educational: "One of the most delightful planes to watch in flight. Kids laugh and experiment with the wing angles.",
                cultural: "Popularized by FoldNfly as their top acrobatic recommendation."
            ));

            // 7. Canard - Advanced (Educational)
            planes.Add(CreatePlane(
                id: "canard",
                name: "Canard",
                difficulty: Core.DifficultyLevel.Advanced,
                category: Core.PlaneCategory.AdvancedWing,
                isFree: false,
                shortDesc: "Features small forward wings (canards) ahead of the main wing — a real aerodynamic concept used on modern aircraft.",
                educational: "The strongest teaching plane. Players learn that real airplanes use canards for pitch control.",
                cultural: "Based on French paper aviation experiments and the canard configuration used in real aviation."
            ));

            // 8. The Bird - Advanced (Distance)
            planes.Add(CreatePlane(
                id: "the_bird",
                name: "The Bird",
                difficulty: Core.DifficultyLevel.Advanced,
                category: Core.PlaneCategory.Glider,
                isFree: false,
                shortDesc: "FoldNfly's top-rated distance champion. Features distinctive 'bird-like' accordion wings for stability and lift.",
                educational: "The 'hero' long-distance plane. Players feel real accomplishment when they master the folds and throw.",
                cultural: "Frequently cited as one of the best-performing distance designs in the paper airplane community."
            ));

            // Add to library
            library.allPlanes = planes;

            // Save everything
            AssetDatabase.CreateAsset(library, AssetPath + "PaperPlaneLibrary.asset");

            foreach (var plane in planes)
            {
                string safeName = plane.planeId.Replace(" ", "_");
                AssetDatabase.CreateAsset(plane, AssetPath + safeName + ".asset");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[Paper Wings] Successfully created {planes.Count} MVP plane assets + PaperPlaneLibrary in {AssetPath}");
            EditorUtility.DisplayDialog("Paper Wings", 
                $"Created 8 MVP PaperPlaneDefinition assets and the central library.\n\nLocation: {AssetPath}", 
                "Great!");
        }

        private static PaperPlaneDefinition CreatePlane(
            string id, string name, Core.DifficultyLevel difficulty, 
            Core.PlaneCategory category, bool isFree,
            string shortDesc, string educational, string cultural)
        {
            var def = ScriptableObject.CreateInstance<PaperPlaneDefinition>();
            def.planeId = id;
            def.displayName = name;
            def.difficulty = difficulty;
            def.primaryCategory = category;
            def.isFree = isFree;
            def.shortDescription = shortDesc;
            def.educationalNote = educational;
            def.culturalNote = cultural;

            // Placeholder values - user will fill in real 3D model + steps later
            def.steps = new List<FoldingStep>();

            return def;
        }
    }
}