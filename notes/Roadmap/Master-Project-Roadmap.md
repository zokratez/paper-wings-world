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

**Estimated Overall Completion:** **95%** (Phase 7 tooling & docs complete; final backend integration + assets remain)

- **Phase 0–6:** Complete
- **Phase 7:** Complete (all release preparation infrastructure delivered)

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
| **6** | Testing, Optimization & Final Polish | Complete     | 100%       | PerformanceManager, LOD, splash, Final Testing Checklist (top 10), multiple Editor helpers, dev tools gating |
| **7** | Release Preparation                 | Complete     | 100%       | Full suite of release Editor helpers (Device Build, Release Build, Remove Dev Tools, Full Release Preparation), Phase-7-Release.md with verification checklist & workflow, Privacy Policy integration, build documentation |

---

## Current Phase Status (Phase 6 — Testing, Optimization & Final Polish)

**Status:** ✅ Phase 6 Complete. All wrap-up items delivered. Ready for real-device testing and closed beta.

### What Is Done (Phases 0–5 Summary)
- Complete folding + real 3D rigged models + smooth animations
- Full flight physics with region-specific wind/thermals/particles/audio
- Main Hub, Region Selection, Post-Flight Summary, Mastery badges, progression
- Supabase auth (anonymous + email upgrade) + cloud progress sync
- Monetization foundation: 2 products, purchase buttons on locked content, Premium badges, Settings screen with Restore & Sign Out, success toasts

### What Is Next (Phase 6 — Complete)
- Editor helper "Paper Wings / HIGH INTENSITY - Prepare Build Settings for Mobile Testing" added (one-click scenes + mobile settings)
- Short Final Testing Checklist (top 10 must-test items) added to Phase-6-Testing.md
- PerformanceManager centralized, PaperModelLOD implemented, splash + polish items complete
- Ready for device QA matrix and closed beta / store submission prep

See `notes/Roadmap/Phase-6-Testing.md` for the detailed checklist and known issues.

---

## Phase 7: Release Preparation (Complete)

**Status:** ✅ Phase 7 Complete. All release preparation tooling, documentation, and infrastructure delivered. Project is now ready for final backend integration, asset production, and v1.0 submission.

### Phase 7 Deliverables Delivered
- Comprehensive living document `notes/Roadmap/Phase-7-Release.md` with:
  - Store accounts & asset checklists (exact sizes for icons/screenshots)
  - Build & Export Workflow (step-by-step iOS + Android)
  - Release Build Verification Checklist (pre-submission gate)
  - Privacy policy requirements and in-app integration notes
- Full suite of Editor helpers under **Paper Wings / HIGH INTENSITY**:
  - Prepare Build Settings for Mobile Testing
  - Prepare for Device Build (iOS + Android)
  - Prepare Release Build
  - Remove Dev Tools for Release (with #if gating of Phase 5 dev panel)
  - **Full Release Preparation** (one-click that runs the entire chain in sequence)
- Privacy Policy placeholder drafted (`notes/PrivacyPolicy.md`) + wired into Settings screen with in-app viewer
- Updated root README with "How to Build for Release" section
- Master roadmap and all docs updated with final status

### Remaining Work Before Submission (not Phase 7 scope)
- Real RevenueCat IAP integration (remove simulation)
- Production Supabase setup + hosted privacy policy
- Final visual assets (icons + all required screenshots from release builds)
- End-to-end device verification using the Verification Checklist

See `notes/Roadmap/Phase-7-Release.md` (especially the Verification Checklist and Full Release Preparation helper) for the complete path to submission.

---

## High-Level Timeline Estimate

| Milestone                  | Estimated Timeline     | Notes |
|---------------------------|------------------------|-------|
| Phase 0–5 Complete        | Done (May 2026)        | Full foundation + monetization simulation |
| Phase 6 (Testing & Polish)| Late May – June 2026   | Performance, loading, device QA, final checklist |
| Phase 7 (Release Prep)    | June 2026              | Complete — all Editor helpers, Phase-7 doc, verification checklist, dev tools removal |
| Submission & Launch       | July 2026+             | After final IAP/Supabase integration, assets, and Verification Checklist sign-off |

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

**Last major update:** May 2026 (Phase 7 Release Preparation kickoff — Phase-7 doc + DeviceBuildHelper)