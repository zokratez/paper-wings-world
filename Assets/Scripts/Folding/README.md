# Folding Tutorial System — Phase 1 Foundation

This folder contains the core data-driven Folding Tutorial System for Paper Wings World.

## Current Status (as of May 2026)

**Built:**
- Complete data layer (`PaperPlaneDefinition`, `FoldingStep`, `PaperPlaneLibrary`)
- Main runtime controller (`FoldingTutorialManager`)
- UI Toolkit skeleton screens (selection + folding view)
- Large touch-target friendly styling for tablets
- Placeholder architecture for 3D model + animation playback

**Not yet built (intentionally):**
- Actual 3D paper models and fold animations (Sam is producing content)
- Real fold line highlighting shader
- Touch rotation / orbit controls on the paper model
- Persistence of progress
- Printable PDF generation (will use a lightweight library later)

## How to Use (for the 8 Locked Planes)

1. Open `~/paper-wings-world/` in Unity 6+.
2. Let Unity resolve the packages in `Packages/manifest.json`.
3. Create a new Scene and add the `FoldingTutorialManager` prefab (to be created).
4. Create `PaperPlaneDefinition` assets for each of the 8 MVP planes in `Assets/ScriptableObjects/PaperPlanes/`.
5. Assign thumbnails, 3D prefabs (when ready), and populate the `steps` list.
6. Create a `PaperPlaneLibrary` asset and drag the 8 definitions into it.
7. Use UI Builder to flesh out the two UXML files with proper layout.

## Architecture Goals

- 100% data-driven — adding plane #9 should require zero code changes.
- Modular — selection, step logic, 3D view, and printing are loosely coupled.
- Tablet-first — all interactive elements have large touch targets (min 56px recommended).

## Next Work (after content approval)

- Real 3D paper models with proper folding rigs (bones or blend shapes)
- Animation clips for each step
- Touch-based paper rotation + zoom
- Step validation (user actually performs the fold)
- Progress saving

See the main project notes for the locked 8 planes and full library research.