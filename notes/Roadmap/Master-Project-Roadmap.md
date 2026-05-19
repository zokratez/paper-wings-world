---
type: roadmap
title: "Master Project Roadmap — Paper Wings World"
status: active
updated: 2026-05
github: "https://github.com/zokratez/paper-wings-world"
---

# Master Project Roadmap — Paper Wings World

**Project:** Paper Wings World  
**Owner:** Sam (ooabisabi, LLC)  
**GitHub:** https://github.com/zokratez/paper-wings-world  
**Last Updated:** May 2026

---

## Project Overview

**Paper Wings World** is a family-friendly mobile app that teaches children and adults how to fold real paper airplanes from around the world, then launch them into a beautiful 3D flight experience over stylized real-world-inspired environments.

The app combines:

- Step-by-step interactive 3D folding tutorials (8 core planes in v1.0)
- Realistic but kid-friendly flight physics
- Multiple distinct flying regions with unique personalities
- Progression, personal bests, and mastery badges

**Core Philosophy:** High-quality, data-driven, delightful, and educational — with a strong emphasis on tablet-first experience and clean architecture.

---

## Current Overall Progress

**Estimated Overall Completion:** **~88%**

- **Phase 0–5:** Complete (Foundation through Backend, Accounts & Monetization)
- **Phase 6:** Just Started (~10%) — Testing, Optimization & Final Polish

---

## Detailed Phase Table

| Phase | Name                              | Status          | % Complete | Key Deliverables |
|-------|-----------------------------------|-----------------|------------|------------------|
| **0** | Setup & Skeleton                  | Complete        | 100%       | Repo, Unity project, Obsidian vault, locked 8 planes + 3 regions, basic architecture |
| **1** | Core Folding Tutorial System      | Complete        | 100%       | Data-driven folding (PaperPlaneDefinition + steps), 8 planes with real 3D rigged models, smooth animations, UI Toolkit screens |
| **2** | 3D Flight Physics + Real Models   | Complete        | 100%       | Realistic aerodynamics (lift/drag/thermals/wind), kid-friendly recovery, FlightDemo scene, full fold→fly→return loop, improved low-poly models |
| **3** | World Expansion & Progression     | Complete        | 100%       | FlightRegion system, 3 distinct regions, Region Selection, Main Hub, Post-Flight Summary, Mastery badges, progression & persistence |
| **4** | Audio, Effects & Visual Polish    | Complete        | 100%       | Region-specific particles, dynamic audio, camera shake, launch effects, unified palette, fade transitions |
| **5** | Backend, Accounts & Monetization  | Complete        | 100%       | Supabase auth (anonymous + email + upgrade), cloud progress sync, PurchaseManager + 2 IAP products, unlock buttons, Premium badges, Settings screen, Restore, toasts |
| **6** | Testing, Optimization & Final Polish | In Progress  | ~10%       | Performance (60 FPS target), loading indicators, device QA matrix, Known Issues & Testing Checklist |

---

## Current Phase Status (Phase 6 — Testing, Optimization & Final Polish)

**Status:** Just started. Solid foundation from Phases 0–5 is in place and highly playable.

### What Is Done (Phases 0–5 Summary)
- Complete folding + real 3D rigged models + smooth animations
- Full flight physics with region-specific wind/thermals/particles/audio
- Main Hub, Region Selection, Post-Flight Summary, Mastery badges, progression
- Supabase auth (anonymous + email upgrade) + cloud progress sync
- Monetization foundation: 2 products, purchase buttons on locked content, Premium badges, Settings screen with Restore & Sign Out, success toasts

### What Is Next (Phase 6)
- Performance fully centralized via new PerformanceManager.EnsurePerformanceSettings() (removes duplication across SceneTransition + bootstraps)
- Basic distance LOD added for 3D paper models (wing tips disabled beyond threshold)
- Final Polish Items section added to testing checklist (UI responsiveness, touch targets, sound balance, etc.)
- Known Issues updated with evaluation (LOD was evaluated and implemented simply where valuable)
- Maintain checklist and prepare device QA / store submission

See `notes/Roadmap/Phase-6-Testing.md` for the detailed checklist and known issues.

---

## High-Level Timeline Estimate

| Milestone                  | Estimated Timeline     | Notes |
|---------------------------|------------------------|-------|
| Phase 0–5 Complete        | Done (May 2026)        | Full foundation + monetization simulation |
| Phase 6 (Testing & Polish)| Late May – June 2026   | Performance, loading, device QA, final checklist |
| Beta + Store Submission   | July 2026              | After Phase 6 sign-off |

**Note:** Timeline is intentionally relaxed while Huh? and other ventures are active.

---

## Key Decisions

### Tech Stack
- **Unity 6 (URP)** as the core engine (tablet-first mobile)
- Heavy use of **ScriptableObjects** for data-driven design (`PaperPlaneDefinition`, `FlightRegion`, etc.)
- **UI Toolkit** for main folding experience
- Custom aerodynamics on top of Unity Rigidbody
- Local JSON persistence (easy to upgrade to Supabase later)

### Scope (Locked for v1.0)
- Exactly **8 paper airplanes**
- Exactly **3 flight regions**
- Tablet-first experience
- No paid 3D data services until revenue

### Quality Standards
- Data-driven architecture everywhere possible
- Excellent editor tooling for rapid content iteration
- Kid-friendly but not childish (delightful for all ages)
- Clean, readable, well-commented code
- Strong documentation in Obsidian + README

---

## Links & Resources

- **GitHub Repository:** https://github.com/zokratez/paper-wings-world
- **Vision:** `notes/Roadmap/Vision.md`
- **Scope Lock:** `notes/Roadmap/Scope-Lock.md`
- **Phase 0 Checklist:** `notes/Roadmap/Phase-0-Checklist.md`
- **Phase 3 Details:** `notes/Roadmap/Phase-3-World-Expansion.md`
- **Phase 4 Plan:** `notes/Roadmap/Phase-4-Polish.md`
- **Paper Airplane Library:** `notes/Research/Paper-Airplane-Library.md`

---

*This is the single source of truth for the overall project roadmap. Individual phase documents contain the detailed task lists.*

**Last major update:** May 2026 (Post-Hub + Badges + Phase 4 kickoff)