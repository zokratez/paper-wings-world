# Paper Wings World

Family-friendly mobile app that teaches kids and all ages how to fold real paper airplanes from around the world — then launch them into a realistic 3D flight over the actual Earth.

**Status**: Phase 0 — Setup, documentation, and skeleton only.  
Full development begins after Huh? Build 44 ships and generates initial revenue.

**Name**: Paper Wings World (English-first)  
**Target**: Tablet-first (iPad Pro/Air + high-end Android tablets as hero devices)  
**MVP Scope (locked)**: Exactly 8 paper airplanes + exactly 3 flight regions for v1.0

## Quick Links

- [Project Vision](notes/Roadmap/Vision.md)
- [Locked Scope & Constraints](notes/Roadmap/Scope-Lock.md)
- [Tech Decisions (incl. cheapest globe solution)](notes/Research/Tech-Decisions.md)
- [Paper Airplane List (8 planes)](notes/Game-Features/Paper-Airplanes.md)
- [Phase 0 Checklist](notes/Roadmap/Phase-0-Checklist.md)

## Folder Structure

```
/notes/                 ← Primary Obsidian vault (research, design, roadmap)
/Assets/                ← Unity project structure (skeleton ready)
/docs/                  ← Additional technical docs
```

## HIGH INTENSITY MODE - Getting a Playable Demo (5 minutes)

1. Open `~/paper-wings-world/` in **Unity 6+**.
2. Run: **Paper Wings → HIGH INTENSITY - Generate All 8 MVP Planes + Demo Data**
3. Run: **Paper Wings → Generate Low-Poly Rigged Paper Planes (All 8)**
4. Run: **Paper Wings → Assign Real Models to Key PaperPlaneDefinitions**
5. Run: **Paper Wings → HIGH INTENSITY - Create Playable Demo Scene** and **Create FlightDemo Scene** if needed
4. Open the new scene: `Assets/Scenes/FoldingTutorialDemo.unity`
5. Press Play.
6. Click any plane card (Classic Dart has full working steps + animation).

Touch controls: Drag to rotate the paper, pinch to zoom.

See `Assets/Scripts/Folding/Demo/HOW_TO_TEST.md` for details.

**Phase 1 Complete** — All 8 planes have realistic folding + success/reward + Launch to Flight hook. Ready for Phase 2 (3D Flight Physics).

**Phase 2 Started** — Realistic aerodynamics + touch flight controls + FlightDemo scene now exist. You can launch planes from the folding screen.

## Principles (inherited from ooabisabi operating agreement)

- Accuracy over agreement
- Evidence-first (device-verified on real iPads)
- Spanish later (English-first for v1.0)
- Ship the complete thing

---

**Owner**: Sam (ooabisabi, LLC)  
**Started**: May 2026 (Phase 0 approval)  
**Repo**: This directory (will be pushed to GitHub as `paper-wings-world` once initial structure stabilizes)