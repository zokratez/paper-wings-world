# Paper Wings World

Family-friendly mobile app that teaches kids and all ages how to fold real paper airplanes from around the world — then launch them into a realistic 3D flight over the actual Earth.

**Current Phase**: Phase 2 — 3D Flight Physics (in active development)  
**Target Platforms**: Tablet-first (iPad + high-end Android tablets)  
**MVP Scope (locked)**: 8 paper airplanes + 3 starting flight regions

## How to Work on This Project

### Opening in Unity
1. Open the folder `~/paper-wings-world/` directly in **Unity 6+**.
2. Let Unity resolve packages on first open.
3. Useful Editor menus are under **Paper Wings → ...** in the menu bar (especially the model generation and assignment tools).

### Opening the Obsidian Vault (Documentation & Notes)
1. Open **Obsidian**.
2. Choose "Open folder as vault".
3. Select the `notes/` folder inside this project.
4. All planning, research, design decisions, and checklists live here.

## Current Status (Phase 2)

- All 8 MVP planes now have **low-poly rigged 3D models** with proper bone hierarchy.
- The first 4 key planes (Classic Dart, The Ring, Nakamichi Glider, The Bird) are assigned real models by default.
- Full folding tutorial system works with real 3D models (with procedural fallback for the others).
- Flight physics, thermals, improved camera, and stats are implemented and playable.
- Complete fold → launch → fly → return flow is functional.

See `notes/Roadmap/Phase-0-Checklist.md` for detailed task tracking.

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