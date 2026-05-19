# Paper Wings World

Family-friendly mobile app that teaches kids and all ages how to fold real paper airplanes from around the world — then launch them into a realistic 3D flight over the actual Earth.

**Current Phase**: Phase 2 — 3D Flight Physics (in active development)  
**Target Platforms**: Tablet-first (iPad + high-end Android tablets)  
**MVP Scope (locked)**: 8 paper airplanes + 3 starting flight regions

## How to Work on This Project

### Opening in Unity
1. Open the folder `~/paper-wings-world/` directly in **Unity 6+**.
2. Let Unity resolve packages on first open.
3. Useful Editor menus are under **Paper Wings → ...** in the menu bar (especially the model generation, assignment, and refresh tools).

### Opening the Obsidian Vault (Documentation & Notes)
1. Open **Obsidian**.
2. Choose "Open folder as vault".
3. Select the `notes/` folder inside this project.
4. All planning, research, design decisions, and checklists live here.

## Current Status (Phase 2 - Complete)

**Phase 2 is complete.** All major systems are functional and at a high standard:

- All 8 MVP planes have **low-poly rigged 3D models** with proper bone hierarchy and improved paper-like geometry (creases, edge detail, subtle thickness).
- All 8 planes are assigned real models by default.
- Full folding tutorial system works with real 3D models + natural folding animations.
- Flight physics (lift, drag, stability, thermals, wind) + kid-friendly auto-recovery.
- Smooth third-person camera with right-side free-look.
- Scene transitions with fade.
- Live flight stats (altitude, distance, time + best time).
- Complete fold → launch → fly → return flow works end-to-end.

**Phase 3 Foundation (World Expansion) is in progress**:
- Region system (`FlightRegion` + `FlightRegionLibrary`) created.
- 3 distinct regions defined: Grand Canyon, Fuji Foothills, Norwegian Coast.
- Region selection appears after successfully folding a plane.
- Each region can have unique visuals, wind, thermals, and challenges.

See `notes/Roadmap/Phase-0-Checklist.md` for detailed task tracking.

### How to Test the Current Build (Recommended Order)

1. Open the project in **Unity 6+**.
2. Run these menu items **in this exact order**:
   - **Paper Wings → HIGH INTENSITY - Generate All 8 MVP Planes + Demo Data**
   - **Paper Wings → Generate Low-Poly Rigged Paper Planes (All 8)**
   - **Paper Wings → Assign Real Models to All PaperPlaneDefinitions**
   - **Paper Wings → Refresh All Models in Demo**
3. Ensure both scenes are added to **Build Settings**:
   - `FoldingTutorialDemo`
   - `FlightDemo`
4. (Optional) Rebuild the scenes using the menu items if they are missing or outdated.
5. Open `FoldingTutorialDemo.unity` and press **Play**.

**Recommended test flow:**
- Select and fully fold one of the planes with real 3D models (Classic Dart, The Ring, Nakamichi Glider, or The Bird).
- Press **"Launch to Flight →"**.
- Fly in the canyon environment (right side of screen = free-look camera, fly through thermals for lift).
- Use the **"Return to Folding"** button when finished.

See `Assets/Scripts/Folding/Demo/HOW_TO_TEST.md` for detailed controls and tips.

## Documentation & Notes

All planning, design decisions, research, and checklists live in the **Obsidian vault** located at `notes/`.

See the following key notes:
- [Project Vision](notes/Roadmap/Vision.md)
- [Locked Scope & Constraints](notes/Roadmap/Scope-Lock.md)
- [Paper Airplane List & Steps](notes/Game-Features/Paper-Airplanes.md)
- [Current Phase Checklist](notes/Roadmap/Phase-0-Checklist.md)

## Folder Structure

```
paper-wings-world/
├── notes/                 # Obsidian vault (all documentation & planning)
├── Assets/                # Unity project
│   ├── Scripts/
│   ├── Prefabs/
│   ├── Scenes/
│   └── ...
├── Editor/                # Custom Unity Editor tools
└── README.md
```

## Principles

This project follows the high-quality, organized approach used across ooabisabi projects:
- Clean, maintainable, and well-documented code
- Data-driven architecture where possible
- Obsidian as the single source of truth for all non-code documentation

---

*Last updated during Phase 2 (3D Flight Physics + Real 3D Paper Models)*
- Ship the complete thing

---

**Owner**: Sam (ooabisabi, LLC)  
**Started**: May 2026 (Phase 0 approval)  
**Repo**: This directory (will be pushed to GitHub as `paper-wings-world` once initial structure stabilizes)