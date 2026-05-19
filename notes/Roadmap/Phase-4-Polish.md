---
type: roadmap
title: "Phase 4 — Audio, Effects & Visual Polish"
status: in-progress
updated: 2026-05
---

# Phase 4 — Audio, Effects & Visual Polish

**Goal**: Elevate the sensory experience so the game feels premium, immersive, and delightful for kids and families. Move from "functional prototype" to "polished experience".

This phase focuses on audio feedback, particle/visual effects, and consistent visual identity across the Hub, Folding, Region Selection, Flight, and Progress screens.

---

## 1. Audio Improvements

### Goals
- Make folding feel tactile and satisfying with varied sounds.
- Add audio feedback for key moments (launch, flight, celebrations).
- Use both real clips (when available) and high-quality synthesized fallbacks.

### Planned Work
- **FoldingAudio enhancements**:
  - Varied fold sounds per step type (sharp creases vs. large folds).
  - Dedicated launch sound when transitioning to flight.
  - Stronger, more celebratory sounds for new personal bests and badge unlocks.
- **New FlightAudio component**:
  - Dynamic wind/whoosh that scales with plane speed.
  - Ambient region sounds (canyon wind, forest, sea).
  - Subtle paper flutter audio when turning fast.

**Current State**: Basic synthesized tones exist in `FoldingAudio`. No dedicated flight audio layer yet.

---

## 2. Visual Effects (Particles & Polish)

### Goals
- Give the feeling of real paper in motion.
- Make each region feel alive and atmospheric.
- Celebrate key moments visually (especially launch and achievements).

### Planned Work
- **Paper Flutter / Trail Particles**:
  - Subtle paper pieces or edge flutter when the plane exceeds a speed threshold.
  - Light trailing effect on wingtips during turns.
- **Environment Effects**:
  - Gentle wind lines or dust motes in Grand Canyon.
  - Soft mist/fog layers in Fuji.
  - Sea spray or light foam hints on Norwegian Coast.
- **Launch Effect**:
  - Satisfying visual burst or "unfold + fly away" animation/particle effect when pressing Launch from the success screen.
- **Other Polish**:
  - Improved lighting per region (directional light color/angle + better ambient).
  - Subtle screen-space effects or post-process if performance allows (low priority).

**Current State**: No particle systems in flight or folding scenes. Environment is static procedural planes.

---

## 3. Overall Visual Polish & Consistency

### Goals
- Unified kid-friendly visual language across all UI and 3D.
- Each region feels distinct but the whole app feels like one cohesive product.
- Improved readability and delight on tablets.

### Planned Work
- **Color Palette**:
  - Define a consistent "Paper Wings" palette (soft blues, warm earth tones, friendly greens, accent oranges).
  - Apply to Hub, Plane Selection, Folding screens, Success panel, Region cards, Progress screen, and flight UI.
- **Lighting & Sky**:
  - Region-specific sky gradients, better fog, improved directional lighting.
  - Consistent use of `FlightRegion` data for visuals (already partially wired via `RegionManager` and `FlightEnvironment`).
- **UI Refinements**:
  - Ensure all cards, buttons, and text follow the same rounded, friendly, high-contrast style.
  - Better iconography and spacing.

**Current State**: Some region variation exists in `FlightEnvironment`. UI styles are good but not fully unified. Hub and cards were recently improved in Phase 3.

---

## 4. Technical Approach & Constraints

- Keep performance in mind (mobile/tablet).
- Prefer code-driven particle systems and synthesized audio for the MVP (easy to replace with real assets later).
- Extend existing systems:
  - `FoldingAudio`
  - `FlightDemoBootstrap` + `FlightEnvironment`
  - `RegionManager`
  - UI Toolkit styles in `FoldingScreen.uss`
- All new effects should be toggleable or easily disabled for lower-end devices.

---

## 5. Task Checklist

### Audio
- [ ] Enhance `FoldingAudio` with per-step variation and launch sound
- [ ] Create `FlightAudio` component (wind/whoosh + region ambients)
- [ ] Improve celebration sounds (new best + badge)
- [ ] Wire audio calls in `FoldingTutorialManager`, `LaunchToFlight`, `FlightDemoBootstrap`, and progress system

### Visual Effects
- [ ] Add paper flutter/trail particle system (speed-based)
- [ ] Add simple environment particles (dust/wind lines per region)
- [ ] Create launch visual effect (particles + scale/animation on success → flight)
- [ ] Integrate effects into `FlightDemoBootstrap` and region system

### Visual Polish
- [ ] Define and document consistent color palette
- [ ] Update USS and inline styles for Hub, cards, and screens
- [ ] Improve sky/lighting per region in `FlightEnvironment` + `FlightDemoBootstrap`
- [ ] Review all screens for kid-friendly consistency

### Documentation
- [x] Create `Phase-4-Polish.md` with plan
- [ ] Keep this document updated with progress after each major piece

---

## Implementation Notes (Living Section)

**Current Focus**: Starting with audio foundation (most impactful for feel), then particles, then palette unification.

## Progress (This Session)

- **Audio**:
  - `FoldingAudio` now supports varied per-step fold sounds (intensity-based synthesized tones).
  - Added `PlayLaunchSound()` with big celebratory whoosh.
  - New `FlightAudio.cs` component with dynamic speed-based wind/whoosh loop + launch burst.
  - Wired into folding launch and flight bootstrap.

- **Visual Effects**:
  - New `FlightEffects.cs` with runtime-created lightweight paper flutter particles that scale with speed.
  - Launch burst particles + visual pop on the 3D model when launching from success screen.
  - Wired into flight scene.

- **Polish**:
  - Added kid-friendly palette comment block in USS.
  - Enhanced lighting/environment variation already benefits from previous region data work.

More work remains on stronger real audio clips and region-specific particle variety, but the foundation is solid and delightful.

All work will be done with the same high-quality, readable, data-driven standards used in previous phases.

*This document is the single source of truth for Phase 4 scope.*