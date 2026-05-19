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
- [x] **Priority Task #1 complete**: Final 8 MVP planes locked and documented with full profiles in `Game-Features/Paper-Airplanes.md` (3 Beginner, 3 Gliders/Long Distance, 2 Stunt, 2 Cultural/Educational)
- [ ] Create `Designs/Folding-UI-Patterns.md` (how the step-by-step tutorial screen should feel)
- [ ] Create `Aerodynamics/First-Principles.md` (basic lift, drag, stability notes for the 8 planes)
- [ ] Create `Marketing/Positioning.md` (one-pager for App Store + future site)
- [ ] Create `Research/Free-Terrain-Sources.md` (detailed list of SRTM, Copernicus, Sentinel-2 download links + processing steps)
- [ ] Flesh out each of the 8 planes with real folding step counts, estimated photography time, and 3D model complexity notes (Sam)

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