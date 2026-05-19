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

## Getting Started (Phase 1 - Folding System)

1. Open the folder `~/paper-wings-world/` in **Unity 6+**.
2. Let Unity import the packages (UI Toolkit, Addressables, URP, etc.).
3. Go to menu: **Paper Wings → Create MVP Plane Assets (v1.0 Locked 8)**  
   → This will generate all 8 ScriptableObjects + the central library.
4. Create a new scene and add the `FoldingTutorialManager` + UI Documents.
5. Use the `SimplePaperPlaneGenerator` + `PaperModelOrbitController` for early 3D testing.
6. The `notes/` folder is the Obsidian vault (highly recommended to open separately).

See `Assets/Scripts/Folding/README.md` for detailed status and next steps.

## Principles (inherited from ooabisabi operating agreement)

- Accuracy over agreement
- Evidence-first (device-verified on real iPads)
- Spanish later (English-first for v1.0)
- Ship the complete thing

---

**Owner**: Sam (ooabisabi, LLC)  
**Started**: May 2026 (Phase 0 approval)  
**Repo**: This directory (will be pushed to GitHub as `paper-wings-world` once initial structure stabilizes)