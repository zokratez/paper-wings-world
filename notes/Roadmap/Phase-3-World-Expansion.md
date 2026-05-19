---
type: roadmap
title: "Phase 3 — World Expansion"
status: in-progress
updated: 2026-05
---

# Phase 3 — World Expansion & Region System

**Goal**: Turn the single test canyon into a living world with three distinctly different places to fly. Each region must feel meaningfully different in visuals, wind, thermals, and challenge targets while remaining fully data-driven.

Phase 3 builds directly on the folding → flight foundation from Phase 2. The player now chooses *where* they fly after successfully folding a plane.

---

## Clear Goals for Phase 3

1. **Exactly 3 regions for v1.0** (locked in Scope-Lock.md)
   - Grand Canyon (default, balanced)
   - Fuji Foothills (Japan)
   - Norwegian Coast (dramatic fjords)

2. **Distinct flight personalities** — different wind strength, thermal density, spawn heights, and target goals so the same plane flies differently in each place.

3. **Data-driven from day one** — adding a 4th or 10th region later must be a content task, not an engineering task.

4. **Seamless player choice** — after completing any folding tutorial, the player selects a region before launching. A dedicated Region Selection step appears in the success flow.

5. **Foundation for progression** — simple goals, best-time tracking, and future unlock gates per region.

6. **Visual identity** — each region immediately communicates its real-world location through sky, fog, lighting, and environment props.

## System Architecture (Clean & Data-Driven)

The Phase 3 region system follows the same proven pattern as the paper plane system:

- **FlightRegion** (ScriptableObject) — Single source of truth for one region. Contains:
  - Identity (id, displayName)
  - Visuals (skybox material, ambient, fog)
  - Flight tuning (wind direction/strength, thermal multiplier, spawn height)
  - Challenge targets (distanceGoal, glideTimeGoal)

- **FlightRegionLibrary** (ScriptableObject) — Central database asset that holds the list of all regions. Provides lookup helpers (`GetRegionById`, `GetDefaultRegion`).

- **RegionManager** (MonoBehaviour, new in this pass) — The runtime coordinator.
  - Singleton pattern for easy scene access.
  - References the Library.
  - Exposes `ApplyRegion(...)` that handles visuals, physics, and environment in one clean call.
  - Makes the system easy to extend (future: region-specific audio, particles, music).

- **FlightSessionData** — Carries the chosen plane + region across the fold → flight scene transition.

- **FlightDemoBootstrap** + **FlightEnvironment** — Consume the region data and RegionManager to configure the world.

- **FoldingTutorialManager** — Hosts the Region Selection UI after success. Uses RegionManager / Library for loading when available.

This architecture means:
- Adding a new region = create one FlightRegion asset + one line in the generator + (optional) environment theme support.
- No hard-coded logic scattered around the flight or folding code.

The Region Selection experience is implemented as a clean, tablet-friendly panel that appears in the post-fold success flow (title + hint + three descriptive choice buttons). It feels like a deliberate "screen" step before the player can launch.

---

## Region System Architecture

The architecture mirrors the proven `PaperPlaneDefinition` + `PaperPlaneLibrary` pattern for maximum consistency and future extensibility.

### Core Components

| Component                  | Type                  | Responsibility |
|---------------------------|-----------------------|---------------|
| `FlightRegion`            | ScriptableObject      | Single source of truth for one region: visuals + wind/thermal tuning + challenge targets |
| `FlightRegionLibrary`     | ScriptableObject      | Central database that holds references to all regions. Provides lookup by ID and default selection |
| `FlightSessionData`       | Static holder         | Carries the chosen `PaperPlaneDefinition` + `FlightRegion` from folding screen into the flight scene |
| `FlightDemoBootstrap`     | MonoBehaviour         | On scene load, reads the session data and applies region settings (skybox, ambient, fog, wind, spawn height) |
| `FoldingTutorialManager`  | MonoBehaviour (UI)    | Shows region selector on success screen; passes both plane and region into `FlightSessionData` |

### Data Flow

```
Folding complete
    ↓
Success screen shows region buttons (or future carousel)
    ↓
Player picks region → Load FlightRegion asset
    ↓
FlightSessionData.SetSession(plane, region)
    ↓
Scene transition
    ↓
FlightDemoBootstrap reads data and configures:
  - RenderSettings.skybox + ambient + fog
  - PaperAirplanePhysics wind/thermal multipliers
  - Spawn height and initial velocity bias
  - (Future) region-specific environment prefab
```

This design means:
- No hard-coded region logic anywhere except the initial three assets.
- Region tuning lives in Inspector-editable assets.
- UI and flight systems remain generic.

---

## The Three v1.0 Regions

### 1. Grand Canyon (Arizona, USA)

**Region ID**: `grand_canyon`

**Description**  
The iconic layered sandstone canyons of northern Arizona. Vast vertical relief, multiple altitude bands, and powerful thermal lift along the sheer walls. The default "hero" region — dramatic, instantly recognizable, and excellent for teaching altitude management.

**Visual Style**  
- Warm, sun-baked color palette (reds, oranges, pale sandstone)
- Clear, high-contrast blue skies with distant haze
- Strong shadows and rim lighting on canyon walls
- Moderate atmospheric fog that increases with depth
- Bright, dry daylight feel

**Unique Flight Characteristics**  
- **Wind**: Balanced, moderate (0.85× strength). Predictable flow through the canyon.
- **Thermals**: Strong and reliable along walls and over sun-facing mesas (1.0× multiplier).
- **Spawn Height**: ~22 m — gives good initial altitude without being overwhelming.
- **Challenge Targets**: 650 m distance / 52 s glide time.
- **Personality**: The best all-rounder. Rewards both aggressive line choice and patient thermal riding. Perfect first region for every plane.

---

### 2. Fuji Foothills (Honshu, Japan)

**Region ID**: `fuji_foothills`

**Description**  
The lush, forested lower slopes of sacred Mt. Fuji. Rice terraces, small shrines, cedar forests, and gentle volcanic terrain. A calmer, more contemplative place that emphasizes graceful, long-duration flight.

**Visual Style**  
- Rich greens and soft earth tones
- Frequent low mist and gentle morning light
- Volcanic grays and soft lavenders in the distance
- Softer shadows, higher humidity feel
- Beautiful golden-hour and overcast variety

**Unique Flight Characteristics**  
- **Wind**: Light and variable (0.65×). Less push, more drift.
- **Thermals**: Exceptionally strong due to volcanic heating (1.35× multiplier). The standout feature.
- **Spawn Height**: Slightly higher (~28 m) to take advantage of lift.
- **Challenge Targets**: Shorter distance (480 m) but longest glide time goal (68 s).
- **Personality**: The "soaring" region. Planes that are stable and efficient (Nakamichi Glider, The Bird, Stealth Glider) shine here. Players learn to hunt lift rather than power through.

---

### 3. Norwegian Coast (Norway)

**Region ID**: `norwegian_coast`

**Description**  
Dramatic fjords, steep granite cliffs, scattered islands, and lighthouses along the North Atlantic. Strong, consistent winds and a sense of speed and exposure. The most exhilarating and fastest-feeling of the three starter regions.

**Visual Style**  
- Cool, crisp palette: deep blues, grays, whitecaps, mossy greens
- Dramatic cloud cover and changing weather
- Strong horizontal light at golden hour
- Sea spray and low mist over water
- High contrast between dark rock and bright water

**Unique Flight Characteristics**  
- **Wind**: Strong and consistent (1.25×). Powerful tailwinds and crosswinds are common.
- **Thermals**: Weaker (0.75×). Fewer rising columns; more reliance on wind and ridge lift.
- **Spawn Height**: Lower (~15 m) — you start closer to the water and must use the wind immediately.
- **Challenge Targets**: Longest distance goal (720 m) with shortest glide time (41 s).
- **Personality**: The "speed run" region. Darts and fast gliders feel alive. Players must manage energy, use wind lanes, and avoid being pushed into terrain. Most exciting for repeated attempts.

---

## Progression & Challenge Ideas

### Core Loop (v1.0)

1. Fold any plane successfully.
2. Choose a region (initially free choice).
3. Fly with that plane + region combination.
4. See simple post-flight results: distance, air time, whether goals were met.
5. Return to folding or try the same plane in a different region.

### Planned Progression Systems (Phase 3+)

- **Region Goals** — Each region has its own distance + glide-time targets. Beating both on a single flight marks the region "mastered" for that plane.
- **Best Flight Tracking** — Local save of personal best distance and duration per plane/region pair.
- **Unlock Order** (recommended)
  - Grand Canyon always unlocked.
  - Fuji Foothills unlocks after completing 2 planes or beating one Grand Canyon goal.
  - Norwegian Coast unlocks after beating at least one Fuji goal or completing 4 planes.
- **Peaceful vs Challenge Mode** (per region)
  - Peaceful: forgiving wind, strong thermals, auto-recovery.
  - Challenge: gusts, reduced thermal help, tighter goals, no hand-holding recovery.
- **Future Content Hooks**
  - Region-specific collectibles (cherry blossom petals in Fuji, puffins or lighthouses on the coast).
  - Special "record" attempts using the best planes for that region's personality.
  - Seasonal or time-of-day variants (light mist in Fuji mornings, golden hour on the Norwegian coast).

---

## Technical Implementation Plan (Data-Driven)

### Already Complete (Foundation)

- `FlightRegion.cs` ScriptableObject with full visual + environmental + challenge fields.
- `FlightRegionLibrary.cs` for centralized lookup.
- `FlightSessionData.cs` (and legacy `SelectedPlaneHolder` bridge) carrying both plane and region.
- `FlightDemoBootstrap.cs` fully region-aware (applies skybox, ambient, fog, wind, spawn height).
- Editor generator: `Paper Wings → Generate Default Flight Regions` creates the three starter assets with distinct tuning.
- Basic region selector buttons appear after folding success (hard-coded for the three IDs, loads via Resources).
- `FoldingTutorialManager` stores `selectedRegionForLaunch` and passes it on launch.

### Remaining Work (Quality & Polish)

1. **Region Selection UI**
   - Replace crude buttons with proper cards or horizontal carousel.
   - Show region name + one-line personality description + small preview image or icon.
   - Large touch targets (tablet-first).

2. **Region-Specific Environments**
   - Create lightweight `FlightEnvironment` variants or configuration per region (different mesa/floor/rock placement, water planes, tree clusters, lighthouse props).
   - Or keep one procedural system and drive it from region data (preferred for v1.0).

3. **Visual & Audio Polish**
   - Actual skybox materials or gradient skies per region.
   - Region-specific ambient audio layers (canyon wind, forest birds, ocean waves).
   - Particle effects (dust devils in canyon, sea spray on coast, mist in Fuji).

4. **Challenge & Stats System**
   - Post-flight summary that compares against region goals.
   - Local persistence of best scores per plane/region.
   - Simple "Region Mastery" badge when both goals are beaten.

5. **Unlock & Gating**
   - Region availability flags in save data.
   - UI to show locked regions with unlock hint.

6. **Library Integration**
   - Create a real `FlightRegionLibrary` asset and populate it.
   - Update loading path in `FoldingTutorialManager` and `FlightDemoBootstrap` to use the library instead of direct Resources loads.

7. **Documentation & Tooling**
   - Expand `Generate Default Flight Regions` menu to support easy addition of new regions with sensible defaults.
   - Finalize this note and link it from `Regions.md`.

---

## Task Checklist

### Foundation (Complete)

- [x] `FlightRegion` ScriptableObject defined with all necessary fields
- [x] `FlightRegionLibrary` created
- [x] `FlightSessionData` updated to carry region alongside plane
- [x] `FlightDemoBootstrap` applies region settings on load
- [x] Editor generator creates the three locked regions with distinct tuning
- [x] Basic region selector appears after folding success (3 buttons)
- [x] `notes/Game-Features/Regions.md` created with overview table

### Polish & Systems (In Progress / Next)

- [ ] Professional region selection UI (cards/carousel with descriptions)
- [ ] Region-specific environment objects or procedural configuration
- [ ] Post-flight results screen showing distance/time vs region goals
- [ ] Local best-score persistence per plane + region
- [ ] Region unlock progression (Grand Canyon free, others gated)
- [ ] Skybox materials + basic ambient audio per region
- [ ] Particle / VFX hooks (dust, mist, spray)
- [ ] Full integration with `FlightRegionLibrary` asset (remove ad-hoc Resources loads)

### Documentation & Future-Proofing

- [x] This Phase 3 note created and linked from roadmap
- [ ] Update `Phase-0-Checklist.md` to mark Phase 3 foundation complete
- [ ] Extend region generator to support one-click addition of future regions
- [ ] Add region affinity hints to `PaperPlaneDefinition` (optional future field)
- [ ] Write region-specific educational notes for the in-app "About this place" panel

---

## Related Documents

- `Game-Features/Regions.md` — short technical reference
- `Roadmap/Scope-Lock.md` — confirms exactly these three regions for v1.0
- `Roadmap/Vision.md` — high-level world experience goals
- `FlightRegion.cs`, `FlightRegionLibrary.cs`, `FlightDemoBootstrap.cs` — source of truth implementations

---

**Status**: Foundation complete and working. Next priority is a beautiful, tablet-friendly region selection experience and region-specific visual variety so the three places genuinely feel like different worlds.

*This document is the single source of truth for Phase 3 scope and architecture.*

---

## Implementation Status — Completed (High-Quality Pass)

All four requested tasks finished with attention to code quality, data-driven design, and kid-friendly feel.

### 1. 3D Model Improvements (Creases, Thickness, Ring & Bird)
- `GeneratePaperPlaneModels.cs` significantly enhanced.
- Added realistic leading/trailing edge rolls, multi-layer creases, and proper paper thickness simulation on bodies, wings, and tips.
- **The Ring**: Now has convincing tubular structure with inner walls + top/bottom lips (dramatically more annular than before).
- **The Bird**: Multiple accordion-style pleat layers + extra tip "feathers" for the signature layered look.
- All extra detail geometry is cheap (extra quads), properly parented, and does not interfere with the 6-bone `PaperPlaneAnimator` contract.
- Body volume pass + tip exaggeration gives every plane a satisfying "real paper" card-stock presence.
- Run `Paper Wings/Refresh All Models in Demo` (or the full generate + assign) to see the improved models.

### 2. Region System (ScriptableObject + Manager)
- `FlightRegion` is the complete data container (identity, visuals, flight tuning, challenge goals).
- `FlightRegionLibrary` created as the central manager asset (populated automatically by generator).
- `FlightSessionData` (static session carrier) fully supports plane + region handoff.
- `FlightDemoBootstrap` cleaned and completed:
  - Properly applies region visuals (fog, ambient, camera background).
  - Passes thermal multiplier and wind to physics.
  - Calls region-aware environment builder.
- Duplicate `FlightCameraFollower` code removed from bootstrap (single source of truth in its own file).
- Loading and selection hooks already present in `FoldingTutorialManager` (buttons for all 3; Resources or library fallback documented).

### 3. Fuji Foothills + Norwegian Coast (with Unique Personality)
- Both regions fully defined in the generator with production-ready distinct values:
  - **Fuji**: Light wind, very strong thermals (1.35×), high spawn, long glide-time goal (68 s). Soft misty visuals + forest props.
  - **Norwegian Coast**: Strong wind (1.25×), reduced thermals, low spawn over water, long-distance goal (720 m). Cool granite + sea visuals with island/water props.
- `FlightEnvironment.BuildEnvironment(region)` now switches on `regionId` and produces:
  - Different floor/wall/sky color palettes
  - Themed props (mesas vs forest patches vs sea + islands)
  - Thermals scaled by the region's multiplier
- Grand Canyon remains the warm, balanced default.
- Running the generator creates all three `.asset` files + a ready-to-use `FlightRegionLibrary.asset`.

### 4. Documentation
- This note updated with full implementation details.
- All changes respect data-driven principles and tablet-first quality bar.

### How to Activate the Full Phase 3 Experience (Test Instructions)

1. Open the Unity project.
2. Run the menu item: **Paper Wings → Generate Low-Poly Rigged Paper Planes (All 8)** (refreshes improved models).
3. Run **Paper Wings → Generate Default Flight Regions** (creates the 3 regions + library in `Assets/ScriptableObjects/FlightRegions/` and the library asset).
4. (Optional but recommended for demo) Copy the three region `.asset` files into `Assets/Resources/FlightRegions/` so the button loader finds them.
5. Open `FoldingTutorialDemo` scene, play, fold any plane → success screen now shows the three region buttons.
6. Pick a region (different each time) → Launch to Flight.
7. Observe:
   - Dramatically different sky/fog/ambient feel.
   - Different prop layout (warm mesas vs green forest vs blue sea + island).
   - Flight feel changes (Fuji = floaty lift; Norway = fast wind-driven; Canyon = balanced).
   - The same plane (e.g. The Bird or Classic Dart) behaves differently in each place.

The foundation is now solid, clean, and extensible. Adding a 4th region is now a 10-minute content task (extend the generator switch + one line in the UI).

**Status**: Phase 3 foundation + polish complete at high quality. Ready for UI polish (nice cards instead of plain buttons) and persistence of best flights per region.

## Latest Progress (This Session)

### 1. Polished Region Selection Screen
- Replaced plain buttons with proper card-style VisualElements using dedicated USS classes (`.region-card`, icon, title, desc).
- Added distinct emoji icons + color backgrounds for instant visual recognition (🏜️ warm orange for Canyon, 🗻 green for Fuji, 🌊 blue for Coast).
- Clear hierarchy: icon + bold title + personality description line.
- Larger, comfortable tablet touch targets with good spacing and subtle card shadow/border.
- Selection is now a true "screen" step inside the success panel.

### 2. Basic Progression System
- New `FlightProgress.cs` (in `Progression/` folder) — clean, self-contained, JSON-persisted to device persistent path.
- Tracks personal best distance + glide time per (planeId + regionId) pair.
- Simple milestone-based unlocking:
  - Grand Canyon always unlocked (starter).
  - 500 m+ in Grand Canyon → unlocks Fuji Foothills.
  - 600 m+ in Fuji → unlocks Norwegian Coast.
- Integrated into flight end (Return to Folding captures stats from `FlightStatsDisplay` and records via `FlightProgress.RecordFlight`).
- Region Selection now only shows currently unlocked regions (progression-aware).
- Save happens automatically on improvement.

### 3. Improved Per-Region Variety
- Extended `FlightRegion` ScriptableObject with `environmentPrimaryColor` / `environmentSecondaryColor`.
- Generator now sets rich, distinct theming data for all three regions (warm sandstone, lush forest green, cool ocean granite).
- `FlightEnvironment` now primarily pulls floor/wall colors from the region data (data-driven).
- Sky/fog/ambient/lighting already varied per region via generator + RegionManager + Bootstrap.
- Wind, thermal multiplier, spawn height, and challenge targets remain strongly differentiated in the region assets.

All changes keep the system clean, data-driven, and easy to extend (new region = one asset + generator entry + optional prop theme).

## Latest Additions (Current Session)

### 1. Best Scores in Region Selection Cards
- Cards now display personal best distance and time pulled live from `FlightProgress.GetBest(planeId, regionId)`.
- Shows "Best: XXX m / XX s" or "Best: --" when no flight recorded yet.
- Only appears on unlocked cards for the currently folded plane.
- Gives immediate feedback and motivation ("I can beat my best!").

### 2. Nice Locked State for Regions
- All three regions are always shown in the selection (better UX than hiding them).
- Locked cards are greyed out (`.region-card-locked` style), with 🔒 icon, "Locked" label, and clear unlock requirement text:
  - Fuji: "Reach 500m in Grand Canyon to unlock"
  - Norwegian: "Reach 600m in Fuji Foothills to unlock"
- Locked cards are non-interactive (no click handler, lower opacity).

### 3. Simple "My Progress" Screen
- Added "📊 My Progress" button in the success panel (after region selection).
- Clicking it opens a clean full-screen overlay panel with:
  - All 8 planes (from `planeLibrary`).
  - For each plane: best scores across the three regions (or "No flights yet" / "🔒 Locked").
- Professional, readable list layout with good contrast and spacing.
- Close button returns to the success/reward view.
- Fully dynamic, uses the same `FlightProgress` data source.
- Accessible right after any successful fold (no need for main menu yet).

This completes a very satisfying vertical slice of Phase 3 progression and feedback loops while staying lightweight and demo-friendly.