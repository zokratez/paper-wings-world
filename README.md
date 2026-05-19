# Paper Wings World

Family-friendly mobile app that teaches kids and all ages how to fold real paper airplanes from around the world — then launch them into a realistic 3D flight over the actual Earth.

**Current Phase**: Phase 3 — World Expansion & Progression (highly playable)  
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

## Current Playable Experience (Phase 4 Complete - Fully Polished)

You can now experience a complete, motivating loop:

1. **Main Hub** — Friendly welcome screen with title, icon, and two big buttons.
2. **Start New Flight** — Choose from all 8 unique paper airplanes.
3. **Folding Tutorial** — Step-by-step 3D folding with real models and smooth animations.
4. **Region Selection** — After folding, pick from 3 beautiful regions (with personality hints and current best scores).
5. **3D Flight** — Fly your plane with realistic (but forgiving) physics, wind, and thermals that feel different per region.
6. **Post-Flight Summary** — See your distance, time, max altitude + celebration for new personal bests and badges.
7. **My Progress** — View best scores and mastery badges (🥉 Bronze / 🥈 Silver / 🥇 Gold) across all planes and regions.
8. **Progression** — Unlock new regions by achieving distance milestones.

The experience feels cohesive, kid-friendly, and rewarding. Best scores and unlocks persist between sessions.

**Phase 4 Polish Applied**: Region-specific particles, dynamic wind audio, camera shake on thermals/launches, bigger launch effects, unified kid-friendly color palette, and subtle fade transitions between screens. The app now delivers a premium sensory experience.

## Playable Demo Features

You can currently experience:

- **Main Hub** with friendly navigation to flight or progress
- **8 Unique Paper Airplanes** with real 3D rigged models and step-by-step folding tutorials
- **3 Distinct Flight Regions** (Grand Canyon, Fuji Foothills, Norwegian Coast) with unique wind, thermals, and visuals
- **Region-specific particles** (sand/dust, falling leaves/petals, sea spray)
- **Full Progression System** including personal bests, mastery badges (🥉/🥈/🥇), and region unlocking
- **Post-Flight Summary** with stats, celebrations, and replay options
- **Immersive Audio** including varied folding sounds, launch effects, wind/whoosh, and thermal lift sounds
- **Satisfying Feedback** like camera shake on strong thermals and launches, paper flutter particles, and speed contrails
- **Cohesive Kid-Friendly UI** with unified color palette and subtle fade transitions across all screens

All features are fully playable end-to-end in the demo scenes.

### Monetization Flow (Phase 5 - Complete)

- **Freemium model**: First 3 planes + Grand Canyon region are free. Everything else is behind purchase.
- **Two example products**:
  - "Full Content Pack" ($4.99) — unlocks all 8 planes + premium regions.
  - "All Regions Pack" ($2.99) — unlocks the two premium flight regions.
- Purchase buttons ("Unlock for $X.XX") appear on locked cards in Plane and Region Selection.
- **Settings screen** (from Hub) includes Account info, Restore Purchases, and Sign Out.
- Purchase success toasts and "⭐ Premium" badges appear on unlocked content.
- Full simulation mode for immediate testing; clean abstraction ready for real RevenueCat SDK.

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