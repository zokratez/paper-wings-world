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

**Wired into playable demo (latest polish)**:
- Editor tool: "Paper Wings / Phase 5 - Create Supabase Config Asset" generates the required asset.
- `FoldingDemoBootstrap` now creates persistent `SupabaseAuth` + `SupabaseProgressService` singletons (with config) that survive scene loads.
- Main Hub features:
  - Prominent **Account Status** bar near the top (shows "Signed in as: email" or "Anonymous User").
  - Improved **Permanent Account (Email)** section with labeled fields, red error messages for validation/server errors, loading states (buttons disabled + feedback), and nicer spacing/palette.
  - Existing dev tools panel kept unchanged for quick anonymous testing.
- SupabaseAuth now exposes `OnAuthError` with friendly messages (wrong password, weak password, invalid email, etc.).
- Plane Selection cards now respect `ContentUnlockManager.IsPlaneUnlocked()` (locked cards are visually disabled with message).
- Region Selection now uses `ContentUnlockManager.IsRegionUnlocked(id)` (consistent with planes + future IAP).

**Not yet production-ready**:
- Token refresh / session expiration handling
- Advanced anonymous → email identity linking (basic upgrade flow works)
- RevenueCat / Unity IAP integration + real purchase restoration
- Advanced conflict resolution for progress
- Secure key storage and production Supabase RLS hardening
- Actual Supabase project + SQL schema applied by the developer
- Full production-grade login screen (current Hub integration is solid foundation)

The architecture is clean, testable, and ready for the next concrete steps.

---

## Exact Supabase Dashboard Setup & Test Instructions (User Has Existing Account)

Since you already have a Supabase account, follow these precise steps:

### 1. Create or Open a Project in Supabase Dashboard
1. Go to https://supabase.com/dashboard and sign in.
2. Click **"New Project"** (green button) or select an existing project you want to use for Paper Wings World.
3. Give it a name (e.g. "paper-wings-world-dev"), choose a region close to you, and set a strong password for the database.
4. Wait ~1-2 minutes for the project to provision.

### 2. Enable Providers
- In left sidebar: **Authentication** → **Providers**
- Make sure **Anonymous** is **Enabled** (toggle on).
- (Optional for later) Enable **Email** provider.

### 3. Get Your URL and Anon Key (Critical)
- In left sidebar: **Settings** (gear icon at bottom of sidebar) → **API**
- Copy these two values exactly:
  - **Project URL** → looks like `https://abcdefghijklmnop.supabase.co`
  - **Project API keys** → the **anon** key (long string starting with `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...` — the one labeled "anon" or "public").  
    **Do not** copy the `service_role` key.

### 4. Run the Database Schema (One-Time)
- In left sidebar: **SQL Editor** (new query)
- Copy the entire SQL block from the section **"Supabase Project Setup (Required)"** earlier in this document (the `CREATE TABLE`, `alter table`, and all `create policy` statements).
- Paste into the SQL Editor and click **Run**.
- You should see success messages. Verify the two tables appear under **Table Editor**.

### 5. Create & Fill the Config in Unity
1. In Unity: Menu → **Paper Wings → Phase 5 - Create Supabase Config Asset**
2. The asset appears in `Assets/ScriptableObjects/SupabaseConfig.asset`
3. Select it in the Project window and fill:
   - **Supabase Url** = the Project URL you copied
   - **Anon Key**   = the anon/public key you copied
4. **Recommended for demo play:**
   - Create folder `Assets/Resources/` if missing
   - Copy the filled `SupabaseConfig.asset` into `Assets/Resources/`
   - Ensure the file in Resources is named exactly `SupabaseConfig.asset`

### 6. Test Using the Dev Tools Panel
- Press Play in Unity.
- On the **Main Hub** screen, scroll to the bottom — you will see the beige **🛠️ Phase 5 Dev Tools** bar with exactly three buttons:
  - **Sign in Anonymously**
  - **Load Cloud Progress**
  - **Save Cloud Progress**
- Use the flow described in the "Testing" section below.

### 7. Verify Data in Supabase Dashboard
- **Authentication → Users**: You should see your anonymous user appear (with a UUID).
- **Table Editor → flight_progress**: After doing flights + using Save/Load, you will see a row containing your `user_id`, `best_flights` (json), and `unlocked_regions` (array).

**Testing Flow – Anonymous (Dev Tools panel at bottom of Hub):**
1. Click "Sign in Anonymously" in the beige Dev Tools strip → status shows user ID.
2. Fly planes, achieve best scores or unlock regions (auto-sync + manual Save button).
3. Use "Load Cloud Progress" and "Save Cloud Progress" to test push/pull.
4. Verify data appears in Supabase dashboard (Authentication → Users and Table Editor → flight_progress).

**Testing Flow – Email Login + Anonymous Upgrade (main "Permanent Account (Email)" section):**
1. Fill Email and Password fields in the blue-ish Email section on the Hub.
2. Click **"Sign Up (or Upgrade)"**:
   - If you were anonymous, this upgrades your temporary account to a permanent email account.
   - If new, creates a real account.
3. After success, the status should show "Signed in as: your@email.com".
4. Click **"Sign In"** with the same credentials to test returning.
5. Fly some planes — progress continues to sync to the cloud under the email user.
6. Click **"Sign Out"** (in the email section) and sign back in to verify persistence.
7. Check Supabase dashboard: the user should now have an email associated (instead of purely anonymous).

**Tip**: You can mix flows for testing (sign in anonymously in dev panel, then upgrade via the email Sign Up button). The session token is shared.

---

**Important**: Both flows now work. The dev tools panel is intentionally kept for quick anonymous testing. The email section is the path toward real user accounts.

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