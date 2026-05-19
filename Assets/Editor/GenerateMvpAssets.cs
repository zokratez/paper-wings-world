using UnityEngine;
using UnityEditor;
using PaperWings.Folding;
using System.Collections.Generic;

namespace PaperWings.Editor
{
    /// <summary>
    /// HIGH INTENSITY: One-click generator for all 8 locked v1.0 planes + sample data.
    /// Creates real assets with populated steps for Classic Dart (playable demo).
    /// </summary>
    public static class GenerateMvpAssets
    {
        private const string Path = "Assets/ScriptableObjects/PaperPlanes/";

        [MenuItem("Paper Wings/HIGH INTENSITY - Generate All 8 MVP Planes + Demo Data")]
        public static void Generate()
        {
            if (!AssetDatabase.IsValidFolder(Path))
                AssetDatabase.CreateFolder("Assets/ScriptableObjects", "PaperPlanes");

            var library = ScriptableObject.CreateInstance<PaperPlaneLibrary>();
            var planes = new List<PaperPlaneDefinition>();

            // 1. Classic Dart - FULLY POPULATED FOR DEMO
            var dart = CreateBasePlane("classic_dart", "Classic Dart", Core.DifficultyLevel.Beginner, Core.PlaneCategory.Dart, true);
            dart.steps = new List<FoldingStep>
            {
                new FoldingStep { stepNumber = 1, title = "Fold in half", instructionText = "Fold the paper in half vertically, then unfold. This creates the center line.", animationClipName = "Fold_CenterLine", animationDuration = 1.8f },
                new FoldingStep { stepNumber = 2, title = "Fold top corners", instructionText = "Fold the top two corners down to the center line.", animationClipName = "Fold_TopCorners", animationDuration = 2.2f },
                new FoldingStep { stepNumber = 3, title = "Fold the top point down", instructionText = "Fold the top point down to the bottom edge of the previous folds.", animationClipName = "Fold_TopPoint", animationDuration = 2.0f },
                new FoldingStep { stepNumber = 4, title = "Fold in half again", instructionText = "Fold the plane in half along the center line.", animationClipName = "Fold_Half", animationDuration = 1.6f },
                new FoldingStep { stepNumber = 5, title = "Fold the wings", instructionText = "Fold both wings down along the body. Make them even.", animationClipName = "Fold_Wings", animationDuration = 2.4f },
                new FoldingStep { stepNumber = 6, title = "Add wing folds", instructionText = "Fold the wing tips up slightly for stability.", animationClipName = "Fold_WingTips", animationDuration = 1.5f }
            };
            planes.Add(dart);

            // 2-8: Basic definitions (full steps can be added later)
            planes.Add(CreateBasePlane("the_ring", "The Ring", Core.DifficultyLevel.Beginner, Core.PlaneCategory.Ring, true));
            planes.Add(CreateBasePlane("loop_plane", "Loop Plane", Core.DifficultyLevel.Beginner, Core.PlaneCategory.Stunt, true));
            planes.Add(CreateBasePlane("nakamichi_glider", "Nakamichi Glider", Core.DifficultyLevel.Intermediate, Core.PlaneCategory.Glider, false));
            planes.Add(CreateBasePlane("stealth_glider", "Stealth Glider", Core.DifficultyLevel.Intermediate, Core.PlaneCategory.Glider, false));
            planes.Add(CreateBasePlane("light_spinner", "Light Spinner", Core.DifficultyLevel.Intermediate, Core.PlaneCategory.Stunt, false));
            planes.Add(CreateBasePlane("canard", "Canard", Core.DifficultyLevel.Advanced, Core.PlaneCategory.AdvancedWing, false));
            planes.Add(CreateBasePlane("the_bird", "The Bird", Core.DifficultyLevel.Advanced, Core.PlaneCategory.Glider, false));

            library.allPlanes = planes;

            AssetDatabase.CreateAsset(library, Path + "PaperPlaneLibrary.asset");

            foreach (var p in planes)
            {
                AssetDatabase.CreateAsset(p, Path + p.planeId + ".asset");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("HIGH INTENSITY: All 8 MVP PaperPlaneDefinition assets created with sample data for Classic Dart.");
        }

        private static PaperPlaneDefinition CreateBasePlane(string id, string name, Core.DifficultyLevel diff, Core.PlaneCategory cat, bool isFree)
        {
            var def = ScriptableObject.CreateInstance<PaperPlaneDefinition>();
            def.planeId = id;
            def.displayName = name;
            def.difficulty = diff;
            def.primaryCategory = cat;
            def.isFree = isFree;
            def.steps = new List<FoldingStep>(); // Will be filled for Classic Dart
            return def;
        }
    }
}