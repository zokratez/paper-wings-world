---
type: roadmap
title: "Phase 5 — Backend, Accounts & Monetization"
status: in-progress
updated: 2026-05
---

# Phase 5 — Backend, Accounts & Monetization

**Goal**: Move from local-only demo to a connected, account-based, freemium product with real monetization while keeping the core experience delightful and accessible.

This phase introduces Supabase for auth and cloud persistence, and a clean monetization layer (initially local + placeholder, ready for RevenueCat).

---

## Architecture Overview

### High-Level Layers
- **Client (Unity)**: Game logic remains unchanged. New layers:
  - `SupabaseAuth` – Handles anonymous + email/password login, session management, JWT tokens.
  - `SupabaseProgressService` – Syncs `FlightProgress` (bests + unlocks) between local JSON and cloud.
  - `ContentUnlockManager` – Central authority for "is this plane/region unlocked?" combining local data, cloud entitlements, and IAP status.
  - `PurchaseManager` (placeholder) – Abstraction over Unity Purchasing / RevenueCat. Fires events on purchase/restore.
- **Supabase (Backend)**:
  - Auth (built-in)
  - Postgres tables with Row Level Security (RLS)
  - Realtime (future for leaderboards/sync)
- **Monetization**:
  - Freemium gating in `ContentUnlockManager`
  - Product catalog tied to `PaperPlaneDefinition.unlockProductId` and future region unlocks
  - Recommended: RevenueCat for reliable cross-platform subscriptions + one-time purchases

### Data Flow (Progress Sync)
1. User launches app (anonymous or logged in)
2. Load local `flight_progress.json`
3. If authenticated → fetch cloud progress → merge (cloud wins on conflict or use last-write-wins)
4. On any best/unlock change → save locally + push to Supabase (upsert on user_id)
5. On login from anonymous → migrate local progress to the new user account

---

## Supabase Project Setup (Required)

1. Create a new Supabase project at https://supabase.com
2. Note your **Project URL** and **anon public key** (Settings → API)
3. Create the following tables (SQL in Supabase SQL Editor):

```sql
-- Enable UUID extension
create extension if not exists "uuid-ossp";

-- Profiles table (linked to auth.users)
create table public.profiles (
  id uuid references auth.users on delete cascade not null primary key,
  created_at timestamptz default now()
);

-- Flight progress (one row per user)
create table public.flight_progress (
  user_id uuid references auth.users on delete cascade primary key,
  best_flights jsonb not null default '{}',
  unlocked_regions text[] not null default '{}',
  updated_at timestamptz default now()
);

-- Enable RLS
alter table public.profiles enable row level security;
alter table public.flight_progress enable row level security;

-- Policies
create policy "Users can view own profile"
  on public.profiles for select
  using (auth.uid() = id);

create policy "Users can insert own profile"
  on public.profiles for insert
  with check (auth.uid() = id);

create policy "Users can view own progress"
  on public.flight_progress for select
  using (auth.uid() = user_id);

create policy "Users can insert or update own progress"
  on public.flight_progress for insert
  with check (auth.uid() = user_id);

create policy "Users can update own progress"
  on public.flight_progress for update
  using (auth.uid() = user_id);
```

4. Enable Email + Anonymous providers in Authentication → Providers.

---

## Current Implementation Status (Foundation)

**Completed in initial foundation pass**:
- `SupabaseConfig` ScriptableObject (holds URL + anon key)
- `SupabaseAuth` – Anonymous login via REST + JWT session restore from PlayerPrefs. Uses proper DTO parsing.
- `SupabaseProgressService` – Automatic push on `FlightProgress` changes + `LoadProgress()` with merge. Uses clean `CloudProgressDto`.
- `FlightProgress` extended with `OnProgressChanged` event, `ExportForCloud()`, `ImportFromCloud()`, and serializable DTOs.
- `ContentUnlockManager` (static) – central freemium gate. Respects `isFree` on planes and regions + local purchase cache + debug grant.
- `FlightRegion` extended with `isFree` + `unlockProductId` for consistent region gating.
- `PaperPlaneDefinition` already had the unlock fields (reused).

**Wired into playable demo (this iteration)**:
- Editor tool: "Paper Wings / Phase 5 - Create Supabase Config Asset" generates the required asset.
- `FoldingDemoBootstrap` now creates persistent `SupabaseAuth` + `SupabaseProgressService` singletons (with config) that survive scene loads.
- Developer debug panel added directly to the Main Hub (clearly labeled, safe to remove later): Sign in Anonymously, Load Cloud Progress, Reset Local, Grant All Debug.
- Plane Selection cards now respect `ContentUnlockManager.IsPlaneUnlocked()` (locked cards are visually disabled with message).
- Region Selection now uses `ContentUnlockManager.IsRegionUnlocked(id)` (consistent with planes + future IAP).

**Not yet production-ready**:
- Full email signup/login UI and flows
- Proper error handling, token refresh, and anonymous → email migration
- RevenueCat / Unity IAP integration + real purchase restoration
- Advanced conflict resolution (last-write-wins vs server authoritative)
- Secure key storage and production Supabase RLS hardening
- Actual Supabase project + SQL schema applied by the developer

The architecture is clean, testable, and ready for the next concrete steps.

**Immediate next actions (post this wiring):**
- Run the new Editor menu item to generate `SupabaseConfig.asset`
- Create real Supabase project + run the SQL schema from this document
- Fill the config keys and (recommended) place a copy in a `Resources/` folder for zero-config play
- Test the full anonymous → cloud sync flow using the dev panel on the Hub
- Then proceed to email auth UI + real RevenueCat IAP wiring

---

## Monetization Model (Freemium)

**Free Tier** (no account required for basic play):
- 3 beginner planes (Classic Dart, The Ring, Loop Plane)
- Grand Canyon region
- Peaceful mode

**Premium Content** (unlock via one-time purchase or subscription):
- Remaining 5 planes
- Fuji Foothills + Norwegian Coast regions
- Challenge mode / stronger thermals / mastery tracking

**Technical Mapping**:
- `PaperPlaneDefinition.isFree` + `unlockProductId`
- Future: Add same fields to `FlightRegion`
- `ContentUnlockManager.IsContentUnlocked(planeId or regionId)` checks:
  1. Local `isFree`
  2. Cloud entitlement (future)
  3. Active purchase (via PurchaseManager)

**Recommended Stack** (matching original project plan):
- Mobile: RevenueCat (handles Unity IAP + StoreKit + Google Billing + subscriptions)
- Web (future): Stripe

---

## Next Steps (Recommended Order)

1. Create Supabase project + run the SQL above
2. Add `SupabaseConfig` asset in `Assets/ScriptableObjects/`
3. Wire `SupabaseAuth` into the Main Hub (add Login button)
4. Implement full `FlightProgress` cloud sync (upload on change, download on login)
5. Add RevenueCat / Unity Purchasing package + `PurchaseManager`
6. Gate plane/region selection based on `ContentUnlockManager`
7. Add restore purchases + receipt validation
8. Analytics events for sign-up, purchase, sync success

---

## Related Documents & Code

- `PaperPlaneDefinition.cs` – unlockProductId field
- `FlightProgress.cs` – local progress (to be extended)
- Original project plan (PAPER_WINGS_WORLD_PLAN.md) – Supabase + RevenueCat mentioned
- `notes/Roadmap/Scope-Lock.md` – Freemium boundaries

---

**Phase 5 is in early foundation stage.** The goal is to have a clean, testable architecture before building full login UI and IAP flows.

*This document is the single source of truth for Phase 5 architecture and decisions.*