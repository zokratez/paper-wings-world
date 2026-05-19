---
type: roadmap
title: "Phase 0 Checklist — Setup & Skeleton"
status: active
updated: 2026-05
---

# Phase 0 Checklist (Current Work)

**Goal**: Low-intensity setup only. No heavy development. Everything needed so that when full work begins after Huh? Build 44, the foundation is solid and the thinking is captured.

## Completed in This Session

- [x] Project root created at `~/paper-wings-world/`
- [x] `notes/` Obsidian vault structure with 6 folders
- [x] Initial Markdown notes seeded:
  - Vision
  - Tech Decisions (cheap globe path)
  - Locked 8 Planes
  - Scope Lock
  - Linear Integration pattern
- [x] Unity `Assets/` folder skeleton created
- [x] Root README + this checklist

## Remaining Phase 0 Tasks (Low Priority, Can Be Done Slowly)

### Documentation & Research
- [x] Master Paper Airplane Library research complete (36 planes curated from FoldNfly + Red Bull + record holders) → `Research/Paper-Airplane-Library.md`
- [x] **Priority Task #1 complete**: Final 8 MVP planes locked and documented with full profiles in `Game-Features/Paper-Airplanes.md`
- [x] All 8 planes now have high-quality folding steps + educational data in the asset generator (matching Classic Dart standard)
- [x] Real 3D model pipeline complete for all 8 planes: Editor tool generates low-poly rigged prefabs with improved paper-like geometry and correct bone hierarchy
- [x] PaperPlaneAnimator + updated manager fully supports real models (first 4 key planes pre-assigned)
- [x] All 8 planes now have proper rigged low-poly prefabs with improved paper-like geometry (creases, edge detail, thickness simulation)
- [x] All 8 PaperPlaneDefinition assets now point to real 3D models by default
- [x] Added "Refresh All Models in Demo" menu item for quick iteration
- [x] Created `notes/Research/3D-Models.md` documenting current model quality and upgrade path

### Phase 1 – Core Folding System (High Intensity, Quality First)
- [x] Professional data-driven Folding Tutorial System (manager, audio, model controller, UI Toolkit)
- [x] Distinct procedural 3D paper models for all 8 planes with recognizable silhouettes
- [x] Basic sound feedback system (fold whoosh + success tones, placeholder ready)
- [x] Demo scene polish: better camera per plane type, smooth step transitions, clear plane name/difficulty/description display
- [x] Fully playable end-to-end with Classic Dart (and steps for all others)
- [x] Realistic per-plane folding animations across all 8 planes
- [x] Success/reward screen with "Launch to Flight" button (Phase 2 hook)
- [x] FlightCharacteristics data populated on all PaperPlaneDefinition assets for future flight physics
- [x] Checklist updated for Phase 1 completion

### Phase 2 — 3D Flight Physics (In Progress)
- [x] PaperAirplanePhysics with realistic lift, drag, torque, stability, wind + gentle auto-recovery
- [x] Improved FlightController: swipe to steer + optional device tilt for banking
- [x] "Launch to Flight" fully functional (transitions with correct plane)
- [x] "Return to Folding" button in flight scene
- [x] Beautiful Grand Canyon-style test environment (walls, mesas, fog, good lighting)
- [x] FlightDemo.scene with proper bootstrap and environment
- [x] All 8 planes carry flight tuning data ready for physics
- [x] Light thermals in the canyon for rewarding gliding
- [x] Enhanced camera with smooth follow + two-finger free look
- [x] Subtle paper flutter visual feedback during flight
- [x] Scene transitions with fade effect
- [x] Basic flight stats display (altitude, distance, time + best time)
- [x] Light thermals + natural wind currents placed for rewarding long glides
- [x] Improved camera with smooth third-person follow + right-side free look
- [x] Polished "Launch to Flight" and "Return to Folding" with fade transitions

### Technical Skeleton
- [ ] Add a proper Unity `.gitignore` (standard + Cesium/Mapbox specifics)
- [ ] Create `ProjectSettings/` placeholder (or actually open in Unity and save once)
- [ ] Decide on primary 3D globe approach for early prototyping (Mapbox vs pre-baked regional) and record the choice
- [ ] Create a minimal "Paper Plane" 3D prototype scene (one simple folded dart model + basic flight camera) — optional, low priority

### Content Production Prep
- [ ] Set up a consistent photography lighting setup and template for all 8 planes
- [ ] Decide on 3D modeling tool (Blender recommended) and export pipeline to Unity
- [ ] Create first plane's step photos + definition data as a test case

### Ops
- [ ] Create GitHub repo `paper-wings-world` and push this structure
- [ ] Decide on final domain (paperwings.world ?)
- [ ] Create Linear workspace (only when we have real tasks)

## Exit Criteria for Phase 0

When Huh? Build 44 is shipping and we are ready to ramp up:

- All planning documents above exist and are linked
- The 8 planes have at least basic production notes (time estimates, photos started)
- One region (Grand Canyon) has sample terrain data imported and running at 60 FPS on an iPad
- The team (Sam + future agents) can open the project and immediately know what to work on next

---

**Current intensity**: Very low. Only work on this when you have spare cycles and mental space. The notes are the main output of Phase 0.