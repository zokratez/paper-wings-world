# Paper Wings World

Family-friendly mobile app that teaches kids and all ages how to fold real paper airplanes from around the world — then launch them into a realistic 3D flight over the actual Earth.

**Current Phase**: Phase 7 Complete — Ready for Real-Device Validation & Submission  
**Target Platforms**: Tablet-first (iPad + high-end Android tablets)  
**MVP Scope (locked)**: 8 paper airplanes + 3 starting flight regions

## How to Work on This Project

### Opening in Unity
1. Open the folder `~/paper-wings-world/` directly in **Unity 6+**.
2. Let Unity resolve packages on first open.
3. Useful Editor menus are under **Paper Wings → ...** in the menu bar (especially the model generation tools and the **HIGH INTENSITY** helpers for mobile testing and device builds).

### Opening the Obsidian Vault (Documentation & Notes)
1. Open **Obsidian**.
2. Choose "Open folder as vault".
3. Select the `notes/` folder inside this project.
4. All planning, research, design decisions, and checklists live here.

## Current Status (Phase 7 Complete — Ready for Submission)

**Phase 7 is complete.** All release preparation tooling, documentation, and infrastructure have been delivered. The project is at ~95% and ready for final real-device validation and store submission.

**Phase 7 Deliverables Complete:**
- Full suite of HIGH INTENSITY Editor helpers including the one-click **Full Release Preparation** (chains every prior release helper).
- Comprehensive `notes/Roadmap/Phase-7-Release.md` with asset checklists, Build & Export Workflow, Release Build Verification Checklist, and the new Pre-Submission Validation Checklist (final 10 critical checks).
- Privacy Policy integration (placeholder + in-app viewer).
- Updated Master Roadmap and this README with clear submission guidance.

**Phase 6 & Earlier:** All foundation systems (folding, flight, progression, auth, monetization simulation, performance, LOD, dev tools removal) are production-ready.

**Remaining Work (Blockers):**
- Real RevenueCat integration (remove simulation)
- Production Supabase + hosted live Privacy Policy
- Final visual assets (icons + screenshots from release builds)
- Execute the Pre-Submission Validation Checklist on physical devices

## Ready for Submission

The release pipeline is now fully operational:

**One-Click Preparation:**
- **Paper Wings → HIGH INTENSITY - Full Release Preparation** (recommended — runs the entire chain)
- Individual helpers also available for granular control

**Final Validation (Before Upload):**
- Use the **Pre-Submission Validation Checklist** (last 10 critical checks) in `notes/Roadmap/Phase-7-Release.md`.
- All testing must be performed on real devices with builds produced after the Full Release Preparation helper.
- The Verification Checklist and detailed workflow in the same document provide the complete pre-upload gate.

Once the remaining blockers above are resolved and the final 10 checks pass on physical hardware, the project is ready to upload to App Store Connect and Google Play Console.

See `notes/Roadmap/Phase-7-Release.md` for the living checklists and exact process.

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
3. For internal device testing, run:
   - **Paper Wings → HIGH INTENSITY - Prepare Build Settings for Mobile Testing**
4. For production / store submission builds, run the new release helper:
   - **Paper Wings → HIGH INTENSITY - Prepare for Device Build (iOS + Android)**
     (sets bundle IDs, IL2CPP, version, SDK targets, ARM64 architectures, etc.)
5. (Optional) Rebuild the scenes using the other setup menu items if they are missing or outdated.
5. Open `FoldingTutorialDemo.unity` and press **Play**.

**Recommended test flow:**
- Select and fully fold one of the planes with real 3D models (Classic Dart, The Ring, Nakamichi Glider, or The Bird).
- Press **"Launch to Flight →"**.
- Fly in the canyon environment (right side of screen = free-look camera, fly through thermals for lift).
- Use the **"Return to Folding"** button when finished.

See `Assets/Scripts/Folding/Demo/HOW_TO_TEST.md` for detailed controls and tips.

## How to Build for Release (Final v1.0 Process)

Use this when you are ready to produce builds for TestFlight, Play Internal test, or store submission.

**Always run the preparation helpers in this exact order** (they are cumulative):

1. Generate/refresh content if needed:
   - **Paper Wings → HIGH INTENSITY - Generate All 8 MVP Planes + Demo Data**
   - **Paper Wings → Generate Low-Poly Rigged Paper Planes (All 8)**
   - **Paper Wings → Assign Real Models to All PaperPlaneDefinitions**
   - **Paper Wings → Refresh All Models in Demo**

2. Prepare scenes and settings:
   - **Paper Wings → HIGH INTENSITY - Prepare Build Settings for Mobile Testing**

3. Production configuration (run these three in order):
   - **Paper Wings → HIGH INTENSITY - Prepare for Device Build (iOS + Android)**
   - **Paper Wings → HIGH INTENSITY - Prepare Release Build**
   - **Paper Wings → HIGH INTENSITY - Remove Dev Tools for Release**
     (this step guarantees the Phase 5 dev panel and debug buttons are stripped from the final player)

4. Open **File > Build Settings**, choose your target platform (iOS or Android), and **Build**.

### Platform-Specific Notes
- **iOS**: After Unity exports the Xcode project, open it in Xcode, archive, and distribute via Organizer to TestFlight or App Store Connect.
- **Android**: Prefer building an App Bundle (AAB). Upload directly to the Google Play Console release track of your choice.

**Verification after every release build**:
- Install on a real device.
- Confirm the dev tools panel is gone from the Main Hub.
- Run the full Final Testing Checklist from `Phase-6-Testing.md`.
- Test auth, purchases, cloud sync, and the Privacy Policy button.

For the complete detailed workflow (including post-build verification steps for both platforms), see the "Build & Export Workflow" section in `notes/Roadmap/Phase-7-Release.md`.

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