using UnityEngine;
using UnityEditor;
using PaperWings.Folding;
using PaperWings.Core;
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
            {
                AssetDatabase.CreateFolder("Assets/ScriptableObjects", "PaperPlanes");
            }

            var library = ScriptableObject.CreateInstance<PaperPlaneLibrary>();
            var planes = new List<PaperPlaneDefinition>();

            // === 1. Classic Dart (Beginner - Fully populated for playable demo) ===
            var dart = CreateBasePlane(
                id: "classic_dart",
                name: "Classic Dart",
                difficulty: Core.DifficultyLevel.Beginner,
                category: Core.PlaneCategory.Dart,
                isFree: true,
                shortDesc: "The timeless 5-fold schoolyard dart. The plane almost everyone folds first.",
                educationalNote: "Instant success on the first or second try. Perfectly teaches the basics of symmetry, sharp leading edges, and how speed affects flight. Ideal first plane in the app.",
                culturalNote: "The universal starter paper airplane taught to children worldwide for generations."
            );
            dart.steps = new List<FoldingStep>
            {
                new FoldingStep { stepNumber = 1, title = "Create Center Line", instructionText = "Fold the paper in half vertically from top to bottom, crease firmly, then unfold completely. This creates your center guideline.", animationClipName = "Fold_CenterLine", animationDuration = 1.8f },
                new FoldingStep { stepNumber = 2, title = "Fold Top Corners", instructionText = "Fold the top left and top right corners down so they meet exactly on the center line. Crease sharply.", animationClipName = "Fold_TopCorners", animationDuration = 2.2f },
                new FoldingStep { stepNumber = 3, title = "Lock the Nose", instructionText = "Fold the top point (the small triangle) down so its tip touches the bottom edge of the previous folds.", animationClipName = "Fold_TopPoint", animationDuration = 2.0f },
                new FoldingStep { stepNumber = 4, title = "Fold in Half", instructionText = "Fold the entire plane in half along the center line so the wings come together.", animationClipName = "Fold_Half", animationDuration = 1.6f },
                new FoldingStep { stepNumber = 5, title = "Form the Wings", instructionText = "Fold both wings down along the body. Make the creases parallel to the body and even on both sides.", animationClipName = "Fold_Wings", animationDuration = 2.4f },
                new FoldingStep { stepNumber = 6, title = "Stabilize the Tips", instructionText = "Fold the outer edges of the wings up slightly (about 1 cm). This adds stability.", animationClipName = "Fold_WingTips", animationDuration = 1.5f }
            };
            planes.Add(dart);

            // === 2. The Ring (Beginner) ===
            var ring = CreateBasePlane(
                id: "the_ring", name: "The Ring", difficulty: Core.DifficultyLevel.Beginner, category: Core.PlaneCategory.Ring, isFree: true,
                shortDesc: "Two connected rings forming a tube. One of Red Bull Paper Wings' three official folding school designs.",
                educationalNote: "Feels almost magical. Kids are amazed it flies at all. Excellent for teaching how non-traditional shapes can be more stable than 'normal' airplanes. One of the most forgiving designs for young builders.",
                culturalNote: "Featured in Red Bull's global Paper Wings competition as the official hangtime/stability starter design."
            );
            ring.steps = new List<FoldingStep>
            {
                new FoldingStep { stepNumber = 1, title = "Fold the First Ring", instructionText = "Fold the paper into a large ring shape by bringing one end to meet the other with overlap.", animationClipName = "Ring_First", animationDuration = 2.0f },
                new FoldingStep { stepNumber = 2, title = "Create the Second Ring", instructionText = "Fold the overlapping section to form the second connected ring.", animationClipName = "Ring_Second", animationDuration = 2.0f },
                new FoldingStep { stepNumber = 3, title = "Lock the Tube", instructionText = "Tuck and crease the ends to form a stable cylindrical tube with two rings.", animationClipName = "Ring_Lock", animationDuration = 1.8f }
            };
            planes.Add(ring);

            // === 3. Loop Plane (Beginner) ===
            var loop = CreateBasePlane(
                id: "loop_plane", name: "Loop Plane", difficulty: Core.DifficultyLevel.Beginner, category: Core.PlaneCategory.Stunt, isFree: true,
                shortDesc: "A simple design specifically engineered to perform clean vertical loops.",
                educationalNote: "Provides immediate, repeatable 'wow' moments. Teaches cause-and-effect (throw angle, speed, and elevator trim). Turns folding into play instantly.",
                culturalNote: "Modern design created to demonstrate that paper airplanes can do real aerobatics."
            );
            loop.steps = new List<FoldingStep>
            {
                new FoldingStep { stepNumber = 1, title = "Basic Dart Base", instructionText = "Start with a strong dart shape for speed and stability.", animationClipName = "Loop_Base", animationDuration = 1.5f },
                new FoldingStep { stepNumber = 2, title = "Elevator Folds", instructionText = "Fold the rear edges upward to create elevators that will cause looping.", animationClipName = "Loop_Elevators", animationDuration = 1.8f },
                new FoldingStep { stepNumber = 3, title = "Wing Adjustments", instructionText = "Slightly angle the wings for the perfect loop trajectory.", animationClipName = "Loop_Wings", animationDuration = 1.6f }
            };
            planes.Add(loop);

            // === 4. Nakamichi Glider (Intermediate - Cultural) ===
            var naka = CreateBasePlane(
                id: "nakamichi_glider", name: "Nakamichi Glider", difficulty: Core.DifficultyLevel.Intermediate, category: Core.PlaneCategory.Glider, isFree: false,
                shortDesc: "A refined Japanese glider design emphasizing wide wings, slight dihedral, and a locked rear structure.",
                educationalNote: "Feels like a real sailplane. Players learn the value of wing area and dihedral for lift. Strong cultural tie-in for the 'world' theme of the app.",
                culturalNote: "Part of the respected Nakamichi family of Japanese paper airplane designs, known worldwide in the paper aviation community."
            );
            naka.steps = new List<FoldingStep>
            {
                new FoldingStep { stepNumber = 1, title = "Wide Fuselage", instructionText = "Create a wide, stable body for the glider profile.", animationClipName = "Glider_Fuselage", animationDuration = 2.0f },
                new FoldingStep { stepNumber = 2, title = "Large Wings", instructionText = "Fold very wide wings with gentle dihedral angle.", animationClipName = "Glider_Wings", animationDuration = 2.5f },
                new FoldingStep { stepNumber = 3, title = "Lock the Tail", instructionText = "Secure the rear with the signature Nakamichi lock for stability.", animationClipName = "Glider_Tail", animationDuration = 1.8f },
                new FoldingStep { stepNumber = 4, title = "Fine Trim", instructionText = "Make small adjustments to the wing tips for maximum glide.", animationClipName = "Glider_Trim", animationDuration = 1.5f }
            };
            planes.Add(naka);

            // === 5. Stealth Glider (Intermediate) ===
            var stealth = CreateBasePlane(
                id: "stealth_glider", name: "Stealth Glider", difficulty: Core.DifficultyLevel.Intermediate, category: Core.PlaneCategory.Glider, isFree: false,
                shortDesc: "FoldNfly's top-rated design for maximum time in the air. Wide wing area optimized for slow, floating flight.",
                educationalNote: "Rewards patience and fine-tuning. Perfect plane for the 3D globe — players will want to keep it aloft while exploring canyons and coastlines. Teaches trimming and launch technique.",
                culturalNote: "One of the most tested and recommended gliders in the modern paper airplane community (FoldNfly)."
            );
            stealth.steps = new List<FoldingStep>
            {
                new FoldingStep { stepNumber = 1, title = "Broad Wing Base", instructionText = "Fold a very wide, flat wing platform for lift.", animationClipName = "Stealth_Base", animationDuration = 2.2f },
                new FoldingStep { stepNumber = 2, title = "Stabilizing Folds", instructionText = "Add subtle folds along the wings for slow, stable flight.", animationClipName = "Stealth_Stabilize", animationDuration = 2.0f },
                new FoldingStep { stepNumber = 3, title = "Trim for Duration", instructionText = "Fine-tune the center and tips for maximum hang time.", animationClipName = "Stealth_Trim", animationDuration = 1.7f }
            };
            planes.Add(stealth);

            // === 6. Light Spinner (Intermediate - Stunt) ===
            var spinner = CreateBasePlane(
                id: "light_spinner", name: "Light Spinner", difficulty: Core.DifficultyLevel.Intermediate, category: Core.PlaneCategory.Stunt, isFree: false,
                shortDesc: "Creates beautiful, hypnotic corkscrew spirals as it descends. Wing angle controls the tightness of the spin.",
                educationalNote: "One of the most delightful planes to watch in flight. Kids (and adults) laugh and experiment with the wing angles. Great for the 'flight personality' system in the simulator.",
                culturalNote: "Popularized by FoldNfly as their top acrobatic recommendation."
            );
            spinner.steps = new List<FoldingStep>
            {
                new FoldingStep { stepNumber = 1, title = "Asymmetric Base", instructionText = "Build a light, balanced body with offset folds.", animationClipName = "Spinner_Base", animationDuration = 1.8f },
                new FoldingStep { stepNumber = 2, title = "Opposite Wing Folds", instructionText = "Fold one wing up and the other down to induce spin.", animationClipName = "Spinner_Wings", animationDuration = 2.1f },
                new FoldingStep { stepNumber = 3, title = "Spin Calibration", instructionText = "Adjust the angle between wings to control spiral speed.", animationClipName = "Spinner_Calibrate", animationDuration = 1.6f }
            };
            planes.Add(spinner);

            // === 7. Canard (Advanced - Educational) ===
            var canard = CreateBasePlane(
                id: "canard", name: "Canard", difficulty: Core.DifficultyLevel.Advanced, category: Core.PlaneCategory.AdvancedWing, isFree: false,
                shortDesc: "Features small forward wings (canards) ahead of the main wing — a real aerodynamic concept used on many modern aircraft.",
                educationalNote: "The strongest teaching plane in the set. Players learn that real airplanes use canards for pitch control. Looks dramatically different in the air and on the folding table.",
                culturalNote: "Based on French paper aviation experiments and the canard configuration used in real aviation (Burt Rutan designs, some fighters, and birds)."
            );
            canard.steps = new List<FoldingStep>
            {
                new FoldingStep { stepNumber = 1, title = "Main Wing", instructionText = "Fold the large main wing platform first.", animationClipName = "Canard_Main", animationDuration = 2.0f },
                new FoldingStep { stepNumber = 2, title = "Forward Canards", instructionText = "Carefully fold the small forward control surfaces.", animationClipName = "Canard_Canards", animationDuration = 2.3f },
                new FoldingStep { stepNumber = 3, title = "Balance & Trim", instructionText = "Adjust the relationship between canards and main wing for stable glide.", animationClipName = "Canard_Trim", animationDuration = 1.9f }
            };
            planes.Add(canard);

            // === 8. The Bird (Advanced - Distance) ===
            var bird = CreateBasePlane(
                id: "the_bird", name: "The Bird", difficulty: Core.DifficultyLevel.Advanced, category: Core.PlaneCategory.Glider, isFree: false,
                shortDesc: "FoldNfly's top-rated distance champion. Features distinctive 'bird-like' accordion wings for stability and lift.",
                educationalNote: "The 'hero' long-distance plane. Players feel real accomplishment when they master the folds and throw. Perfect capstone for the paid tier.",
                culturalNote: "Frequently cited in paper airplane communities as one of the best-performing distance designs ever documented."
            );
            bird.steps = new List<FoldingStep>
            {
                new FoldingStep { stepNumber = 1, title = "Accordion Wing Base", instructionText = "Create the signature layered, bird-like wing structure.", animationClipName = "Bird_Accordion", animationDuration = 2.5f },
                new FoldingStep { stepNumber = 2, title = "Fuselage Lock", instructionText = "Lock the body for rigidity during long glides.", animationClipName = "Bird_Fuselage", animationDuration = 1.8f },
                new FoldingStep { stepNumber = 3, title = "Wing Shaping", instructionText = "Shape the wings with precise creases for maximum lift and stability.", animationClipName = "Bird_Shaping", animationDuration = 2.2f },
                new FoldingStep { stepNumber = 4, title = "Final Trim", instructionText = "Make micro-adjustments to the wing tips for record distance.", animationClipName = "Bird_Trim", animationDuration = 1.6f }
            };
            planes.Add(bird);

            library.allPlanes = planes;

            AssetDatabase.CreateAsset(library, Path + "PaperPlaneLibrary.asset");

            foreach (var plane in planes)
            {
                AssetDatabase.CreateAsset(plane, Path + plane.planeId + ".asset");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[Paper Wings] HIGH INTENSITY: Generated {planes.Count} high-quality PaperPlaneDefinition assets with accurate data from the master library.");
        }

        private static PaperPlaneDefinition CreateBasePlane(string id, string name, Core.DifficultyLevel difficulty, Core.PlaneCategory category, bool isFree, string shortDesc, string educationalNote, string culturalNote)
        {
            var def = ScriptableObject.CreateInstance<PaperPlaneDefinition>();
            def.planeId = id;
            def.displayName = name;
            def.difficulty = difficulty;
            def.primaryCategory = category;
            def.isFree = isFree;
            def.shortDescription = shortDesc;
            def.educationalNote = educationalNote;
            def.culturalNote = culturalNote;
            def.steps = new List<FoldingStep>();

            // Populate sensible default flight tuning data for future Phase 2
            def.flightData = new FlightCharacteristics
            {
                baseSpeed = category == Core.PlaneCategory.Dart || category == Core.PlaneCategory.Stunt ? 1.3f : 0.85f,
                glideEfficiency = category == Core.PlaneCategory.Glider ? 1.4f : (category == Core.PlaneCategory.Ring ? 1.15f : 1.0f),
                stability = category == Core.PlaneCategory.Ring || category == Core.PlaneCategory.Glider ? 1.25f : 0.95f,
                windSensitivity = category == Core.PlaneCategory.Glider ? 0.7f : (category == Core.PlaneCategory.Stunt ? 1.35f : 1.0f)
            };

            return def;
        }
    }
}