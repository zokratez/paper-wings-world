---
type: roadmap
title: "Phase 6 — Testing, Optimization & Final Polish"
status: in-progress
updated: 2026-05
---

# Phase 6 — Testing, Optimization & Final Polish

**Status:** In Progress

**Goal**: Take the high-quality, fully-featured demo from Phase 5 and harden it for real devices — performance, stability, polish, comprehensive testing, and a clear path to beta / store submission.

This phase focuses on:
- Systematic QA and known issues tracking
- Mobile/tablet performance optimizations (60 FPS target, battery/thermal friendly)
- Smooth loading / transition experience
- Final content and UX polish
- Device testing matrix and release checklist

---

## Current Overall Project Status (End of Phase 5)

From Master Project Roadmap:
- **Phases 0–5 Complete** (Foundation, Folding, Flight, World Expansion, Polish, Backend/Auth/Monetization)
- Core experience is delightful, educational, and monetization-ready in simulation mode.
- Supabase auth + cloud progress sync working (anonymous + email).
- RevenueCat-style IAP foundation with two products, unlock buttons, Premium badges, Settings screen, Restore, and confirmations (all simulated for immediate testing).

**Estimated Overall Completion:** ~85–88%

---

## Known Issues (Living List)

**Critical / Blocking**
- None currently.

**High Priority**
- Scene load hitch on some devices during cross-scene transitions (Folding ↔ Flight) — mitigated by fade but not eliminated.
- No real device testing matrix executed yet (all work in Editor + limited simulator).

**Medium / Polish**
- Performance on lower-end Android tablets may drop below 60 FPS in dense particle + physics scenes.
- No explicit battery/thermal throttling awareness.
- Purchase simulation does not persist across app restarts in a user-visible way beyond the existing prefs (fine for demo).
- Loading indicator during transitions is basic (fade only).

**Low / Nice-to-Have**
- No haptic feedback on purchases or strong thermals.
- No accessibility labels / VoiceOver testing yet.
- Analytics events for monetization funnel not yet wired.

*(This list will be updated continuously during Phase 6 testing.)*

---

## Testing Checklist

### Functional Flows (Must Pass on All Target Devices)
- [ ] Full auth loop: Anonymous sign-in → Email sign-up (upgrade) → Sign-in → Sign-out
- [ ] Cloud progress sync: Best scores and region unlocks survive sign-out / restart / device change (via Supabase)
- [ ] Freemium gating:
  - Free planes/regions always accessible
  - Locked content shows correct "Unlock for $X.XX" button
  - Purchasing Full Content Pack unlocks planes + regions
  - Purchasing All Regions Pack unlocks only premium regions
  - "⭐ Premium" badge appears on IAP-unlocked content
- [ ] Settings screen: Account info correct, Restore Purchases works, Sign Out works and refreshes Hub
- [ ] Purchase flow: Buttons trigger simulation, success toast appears, content updates immediately
- [ ] Restore after simulated purchases shows correct state
- [ ] Full play loop: Hub → Fold → Region Selection → Flight (with region data) → Post-Flight Summary → Return
- [ ] Progression: Milestones unlock regions correctly even with IAP present
- [ ] Persistence: Local + cloud data survives app background/foreground and restart

### Performance & Device Targets
- [ ] Stable 60 FPS on iPad Pro / iPad Air (M-series) in all regions
- [ ] Minimum 45–50 FPS on mid-range Android tablets (e.g. Galaxy Tab S7/S8)
- [ ] Graceful degradation on phones (no hard 30 FPS lock yet, but no major hitches)
- [ ] No excessive heat or battery drain during 10+ minute flight sessions
- [ ] Memory stable (no leaks during repeated scene transitions)

### Polish & UX
- [ ] Loading / splash indicator visible and smooth during all scene transitions
- [ ] No visual glitches on different aspect ratios (tablet and phone)
- [ ] All text readable, touch targets ≥ 56 px
- [ ] Consistent kid-friendly palette and typography across Hub, Settings, Selection, Flight HUD
- [ ] Audio continues to feel great (no cutoffs on load)
- [ ] Edge cases: Buying when already owned, restoring with no purchases, signing out mid-flight (graceful)

### Device Matrix (Priority Order)
| Device Type          | Priority | Status     | Notes |
|----------------------|----------|------------|-------|
| iPad Pro / Air (M1+) | P0       | Pending    | Hero device |
| High-end Android tablet | P0    | Pending    | Galaxy Tab S series |
| Mid-range Android tablet | P1   | Pending    | Common in target market |
| iPhone (recent)      | P2       | Pending    | Secondary |
| Low-end Android      | P2       | Pending    | Graceful degradation |

---

## Performance Optimizations (Planned / In Progress)

- Target 60 FPS with `Application.targetFrameRate = 60`
- `QualitySettings.vSyncCount = 0`
- Mobile-friendly quality preset (balanced between visuals and performance)
- Simple adaptive quality scaler if frame rate drops (future)
- Ensure no expensive operations on main thread during flight (particles, audio, stats)
- Review physics timestep and fixed update for mobile

---

## Loading / Transition Polish (Planned / In Progress)

- Enhance existing `SceneTransition` to display a simple loading indicator (text + subtle animation) while the new scene loads and initializes.
- Keep the existing high-quality fade for polish.
- Ensure the loading state feels intentional and never leaves the user on a frozen black screen.

---

## Next Steps in Phase 6

1. Execute device testing matrix (start with hero devices).
2. Implement and validate the performance and loading changes above.
3. Expand Known Issues list with real findings.
4. Final content pass (any remaining educational copy, model tweaks, etc.).
5. Prepare store assets, privacy policy, and submission checklist.
6. Closed beta / TestFlight internal testing.

---

*This document is the living single source of truth for Phase 6 testing, issues, and optimization work.*

**Phase 6 will turn a great demo into a shippable, high-quality product.**