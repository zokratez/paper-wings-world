---
type: research
title: "Tech Stack & 3D Globe Decisions (Phase 0)"
status: approved
updated: 2026-05
cost-constraint: "Free or <$50/month until revenue"
---

# Tech Stack & 3D Globe Decisions — Phase 0

**Approved**: 2026-05 (with explicit cost adjustment from original plan)

## Primary Engine: Unity 6

**Decision**: Unity 6 (Personal — free under $200K revenue) + C#.

**Rationale**:
- Mature, excellent iPad/iOS export and performance tooling.
- Strong ecosystem for 3D paper physics (soft-body or custom mesh deformation for folding).
- Best-in-class mobile profiling and device-adaptive quality settings.
- Mapbox has first-class Unity SDK support.

**Alternative considered**: Godot 4.3+ — rejected for v1.0 because Unity's 3D tooling, terrain systems, and existing flight-sim community examples are stronger for our specific needs. Revisit post-v1.0 if desired.

## Mobile UI / App Shell

**Decision**: Pure Unity app (no React Native / Flutter wrapper).

**Rationale**: Single codebase, single build pipeline, best 3D + 2D integration for the folding tutorials. The entire experience (folding + flight) is deeply 3D and benefits from being in one engine.

A minimal marketing/landing site (Next.js + Vercel, English-first) can be added later under a separate `marketing/` folder if needed.

## 3D Globe + Real-World Terrain (Critical Cost-Constrained Decision)

**Constraint (user-approved)**: No paid Cesium ion commitment. Maximum $0–50/month until the app generates real revenue. Prefer completely free or near-free solutions for v1.0.

### Recommended Approach for v1.0 (Tablet-First, 3 Regions Only)

**Primary recommendation: Mapbox Maps SDK for Unity (Free tier)**

- **Cost**: 25,000 Monthly Active Users (MAU) free. Includes unlimited vector + raster + terrain tiles within the allowance. Extremely generous for early-stage and even early commercial use.
- **3D Terrain**: Excellent Terrain-RGB elevation data + satellite imagery.
- **iPad Performance**: Strong mobile focus. Works well on modern iPads with proper LOD and caching.
- **Region Control**: Easy to restrict playable area to the three locked MVP regions (Grand Canyon, Fuji foothills, Scottish coast) via bounds, style filters, or custom tilesets.
- **Risk**: Usage-based, but the 25k MAU free tier means we stay at $0 for a very long time.

**Strong zero-cost / fully offline alternative (preferred if we want to eliminate all risk)**

Pre-bake high-resolution terrain and imagery **only for the three specific regions** using 100% free public datasets:

- Elevation: Copernicus DEM (Europe), SRTM 1-arc-second (US), ALOS World 3D (Japan/global)
- Imagery: Sentinel-2 (AWS open data), NASA Blue Marble, USGS National Map
- Process into Unity Terrain tiles or a custom subdivided icosphere + vertex displacement mesh
- Low-poly procedural globe for the rest of the planet (with simple NASA Blue Marble texturing)
- All assets downloaded once at first launch or via Wi-Fi pre-cache

**Advantages**:
- $0 forever
- Predictable performance on tablets (no streaming hitches during low-altitude flight)
- Full offline support after initial download
- No terms-of-service or quota surprises

**Disadvantages**:
- Higher upfront engineering work (terrain tiling, LOD system, globe projection math)
- Less "live" global exploration outside the three regions

**Hybrid (recommended for Phase 1 prototyping)**: Start with Mapbox for rapid iteration and beautiful visuals during development. Evaluate switching the production build to the pre-baked offline regional terrain before public launch if we want zero ongoing cost.

### Why We Are Not Using Full Cesium + Google Photorealistic for v1.0

- Cesium ion Community tier has very low root-tile quotas (1,000/month) and commercial restrictions.
- Google Photorealistic 3D Tiles + high-quality terrain quickly burns through paid tiers ($149–$499+/mo).
- Our MVP only needs three bounded, high-beauty regions — not planet-wide photogrammetry.
- The cost adjustment explicitly forbids paid commitment until revenue exists.

Cesium for Unity remains the long-term aspirational choice for "fly anywhere on Earth" once the app is earning. We will keep the architecture flexible enough to swap terrain providers later.

## Folding Tutorial System

- Custom 3D paper mesh with believable bending (Unity soft-body, mesh deformation via bones + blend shapes, or a lightweight physics approach).
- Step engine driven by ScriptableObject or JSON definitions (plane-agnostic).
- Touch raycasting to detect fold zones.
- Validation + success feedback per step.
- Ghosted "next fold" hints and slow-motion replay.
- Export to PDF (using Unity's built-in or a lightweight PDF library).

## Backend & Monetization (Deferred to Phase 4)

- New Supabase project (never reuse PACO or Huh?).
- RevenueCat for IAP (one-time unlock + optional subscription).
- Stripe for any web purchases (marketing site).
- PostHog for analytics (same events pattern as Huh?).

All backend work is explicitly out of scope for Phase 0–3.

## Audio

Unity built-in audio + addressables for region-specific ambience banks (wind layers, birds, ocean, distant aircraft). FMOD only if budget allows later.

## Localization

English-first for v1.0. All strings in Unity Localization tables or Addressables. Spanish added in a later phase using the same pattern as Huh?.

## Summary of Phase 0 Tech Choices

- Engine: Unity 6 (free)
- Globe for v1.0: Mapbox (free tier) **or** pre-baked regional offline terrain (free public data) — decision to be validated in early prototyping
- No paid 3D streaming services until revenue
- Tablet-first performance targets and UI
- Single Unity codebase

---

*This document is the source of truth for all tech decisions until revenue allows re-evaluation.*