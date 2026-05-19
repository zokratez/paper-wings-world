# Phase 7: Release Preparation

**Goal:** Take the polished, tested build from Phase 6 through all store submission requirements, asset production, legal/compliance, and final pre-flight checks so the app can ship to the App Store and Google Play.

**Current State (end of Phase 6):**  
- Fully playable end-to-end experience with folding, flight, progression, accounts (Supabase), and freemium gating (simulation mode).  
- Performance centralized, LOD, splash, transitions, and device testing checklist complete.  
- Editor tooling for scenes and mobile settings in place.

**Phase 7 will turn a high-quality internal build into a shippable consumer product.**

---

## 1. Store Accounts & App Records (Prerequisites)

### Apple App Store Connect
- [ ] Apple Developer Program membership ($99/year) active
- [ ] Create App ID + Bundle Identifier in developer portal (must exactly match Unity PlayerSettings)
- [ ] Create new app record in App Store Connect
  - Primary language: English (United States)
  - Secondary: Spanish (if localized later)
- [ ] Agree to all contracts, tax forms, and banking setup (critical for IAP payouts)
- [ ] Add test users for TestFlight (internal + external)

### Google Play Console
- [ ] Google Play Console developer account ($25 one-time)
- [ ] Create new app
- [ ] Set up merchant account for IAP (if offering paid content)
- [ ] Complete content rating questionnaire (important for kid-friendly app)
- [ ] Prepare for 64-bit compliance and target API level

**Critical:** Bundle identifier must be identical across Unity, Apple, and Google (e.g. `com.ooabisabi.paperwingsworld`).

---

## 2. Required Visual & Brand Assets — Detailed Checklist

**Use this exact checklist when producing assets.** All dimensions are in pixels. Create a folder `Assets/StoreAssets/` (or external design folder) with final production files.

### App Icons (Mandatory)
- [ ] **Master source**: 2048×2048 or vector (SVG/AI) — "paper_wings_icon_master"
- [ ] **iOS App Store** (required): 1024×1024 — `icon_1024.png`
- [ ] **iOS** (all slots in Unity or Asset Catalog): 180, 120, 87, 80, 58, 40, etc.
- [ ] **Android Adaptive Icon**: 
  - Foreground: 432×432 (safe zone 264×264)
  - Background: 432×432
- [ ] **Android Legacy + Play Store**: 512×512 (`icon_play.png`)
- [ ] **Unity PlayerSettings**: Assign the correct icons for iOS and Android tabs

**Tool recommendation**: Use https://appicon.co or Icon Slayer for batch generation from master.

### Screenshots — iOS (Required for App Store)
- [ ] **6.7" iPhone** (iPhone 14/15/16 Pro Max): 1290×2796 (portrait) or landscape equivalent — at least 3–5 screens
- [ ] **6.5" iPhone** (XS Max / 11 Pro Max): 1242×2688
- [ ] **5.5" iPhone** (8 Plus): 1242×2208
- [ ] **12.9" iPad** (3rd gen+): 2048×2732
- [ ] **11" iPad Pro**: 1668×2388

**Recommended content order**:
1. Main Hub (welcome + big buttons)
2. Plane Selection with Premium badges visible
3. Folding tutorial step with real 3D model
4. Region selection (beautiful Grand Canyon or Fuji)
5. In-flight over one of the regions + stats
6. Post-flight summary with badge celebration

### Screenshots — Android (Google Play)
- [ ] **Phone**: 1080×1920 or 1440×2560 (at least 2–4)
- [ ] **7" Tablet**: 1920×1200 or 1280×800 landscape/portrait
- [ ] **10" Tablet**: 2560×1600 or similar

**Note**: Google requires screenshots for the default language; additional languages optional.

### Feature Graphic (Google Play — Required)
- [ ] 1024×500 px — `feature_graphic.png`
- Must be eye-catching, show the plane in flight over a scenic region

### Promo Video (Strongly Recommended)
- [ ] 15–60 seconds, 1080p or higher
- Show folding → launch → beautiful flight
- Export in formats required by each store

### Store Listing Text Assets (prepare in Phase-7 doc or separate file)
- [ ] App name: "Paper Wings World"
- [ ] Subtitle (30 chars max): "Fold. Fly. Explore the world."
- [ ] Full description (up to 4000 chars) — benefit-focused, mention 8 planes, 3 regions, real physics, progression
- [ ] Keywords (App Store — 100 chars total)
- [ ] Category: Education (primary) or Games > Simulation

### Privacy Policy & Legal Links
- [ ] Hosted live Privacy Policy at permanent URL (e.g. `https://paperwingsworld.com/privacy`)
- [ ] "Privacy Policy" button in Settings screen (already implemented in Phase 7 — currently shows placeholder text; must point to live URL before submission)
- [ ] Terms of Service (recommended)
- [ ] Support email visible in policy and Settings

**Guidelines for all screenshots**:
- Use final release build (no dev UI, no "Development Build" watermark)
- Real device frames optional but nice
- Show actual gameplay, not mocked
- High contrast, kid-friendly but premium feel
- Test on light and dark device themes if supported

**Current Status**: Placeholder Privacy Policy drafted in `notes/PrivacyPolicy.md` and wired into Settings screen. Real hosted version + final copy still required.

---

## 3. Store Listing Copy (English Primary)

### Required Text
- **Title:** Paper Wings World
- **Subtitle:** Fold real paper airplanes. Fly them around the world.
- **Description:** Write compelling, benefit-focused copy highlighting:
  - Educational folding tutorials with real 3D models
  - Realistic physics and beautiful global regions
  - Progression, personal bests, and mastery badges
  - Free core experience + optional premium unlocks
- **What's New** (for v1.0): "First public release"

**Localization note:** English is the initial language. Spanish support can be added later (aligns with other ooabisabi projects).

### Privacy & Data
- Privacy Policy URL (must be publicly hosted and linked in both stores)
- Data types collected (account, progress, purchase history)
- Children's privacy considerations (COPPA — app is kid-friendly)

---

## 4. Privacy Policy & Legal Requirements

**Mandatory for both stores** (especially because of accounts + IAP):

- Host a clear Privacy Policy (recommended location: `https://paperwingsworld.com/privacy` or via GitHub pages / Notion / dedicated page).
- The policy must cover:
  - What data is collected (Supabase user IDs, email if provided, flight progress, purchase receipts)
  - How data is used and shared (no selling of personal data)
  - Third parties (Supabase, Apple/Google payment processors, future analytics)
  - User rights (delete account, data export)
  - Children's data handling

**Recommended additional pages:**
- Terms of Service
- Support / Contact email

**App Store specific:** Include a link in the app (Settings screen already has account section — add a "Privacy Policy" button that opens the URL).

**Google Play:** Provide the policy URL during app creation.

---

## 5. Technical & Build Requirements Before Submission

### Unity PlayerSettings (use the new Editor helper)
- Correct Bundle Identifier for iOS and Android
- Version and Build numbers (semantic: 1.0.0 / 1)
- Company Name: ooabisabi LLC (or your legal entity)
- Product Name: Paper Wings World
- IL2CPP scripting backend (release performance)
- Target iOS version: 15.0+
- Android minSdk: 24, targetSdk: latest (34+)
- Architectures: ARM64 for both platforms
- Icons and splash configured (custom splash already implemented)

### Backend & Monetization (Critical Blockers for Real Launch)
- **Replace simulation mode** in `PurchaseManager` with real RevenueCat SDK + App Store / Play billing.
- Switch Supabase to production project (new keys, RLS verified, backups enabled).
- Remove or gate all dev-only UI (dev tools panel) for release builds (use conditional compilation or build flags).
- Add crash reporting (Sentry or Unity Cloud Diagnostics) and analytics (PostHog / Plausible or Firebase).
- Verify all webhooks (Stripe/RevenueCat if used) and deep link handling.

### Testing Gates
- Pass the entire **Final Testing Checklist** (see Phase-6-Testing.md) on hero devices + at least one low/mid device.
- Internal TestFlight + Google Play Internal test track for 1+ week.
- At least one full end-to-end purchase flow with real sandbox payments.
- Account deletion / data export flow tested.
- Offline behavior + cloud sync edge cases.

---

## 6. Editor Tooling for Release

New in Phase 7:
- `Paper Wings / HIGH INTENSITY - Prepare for Device Build (iOS + Android)` — one-click configuration of production PlayerSettings, bundle IDs, version, IL2CPP, SDK targets, etc.
- `Paper Wings / HIGH INTENSITY - Prepare Release Build` — turns Development Build OFF, disables profiler/debugging, enforces release optimizations.

Run the Device Build helper first, then the Release Build helper immediately before every candidate build for TestFlight or Play track.

Also continue using:
- Scene generation and model refresh tools
- Previous mobile testing helper

---

## 7. Submission & Review Process

### Apple
- Upload via Xcode or Transporter
- Complete App Review information (age rating, content descriptions, export compliance)
- Submit for review (expect 1–7 days; longer for first app)

### Google
- Upload AAB via Play Console
- Content rating, ads declaration (none), target audience (include "Designed for Families" if qualifying)
- Submit for review (usually faster)

**Common rejection reasons to pre-empt:**
- Missing privacy policy link
- Inaccurate content rating
- IAP not properly integrated or described
- Placeholder text or dev UI visible
- Performance or crash issues on review devices

---

## 8. Post-Launch & Iteration Plan

- Monitor crash reports and support emails in first 48 hours
- Prepare v1.0.1 hotfix pipeline
- Plan first content update (new plane or region) using existing data-driven system
- Collect user feedback for future localization and feature prioritization
- Consider "Designed for Families" or teacher resources later

---

## 9. Current Blockers & Open Items (Update as Resolved)

- [ ] Real RevenueCat integration (currently simulation)
- [ ] Production Supabase project + verified RLS + backup strategy
- [x] Privacy policy placeholder drafted (`notes/PrivacyPolicy.md`) + in-app viewer added to Settings screen (Phase 7)
- [ ] Privacy policy hosted at permanent public URL and wired to live link
- [ ] Full set of store screenshots and icons (production quality)
- [ ] Final device testing pass on the Top 10 checklist
- [ ] Remove / conditional-compile dev tools panel for release builds
- [ ] Final store descriptions and keywords copy

---

## 10. Next Steps After This Document

1. Host the Privacy Policy at a permanent public URL and update the in-app button to open the live page.
2. Produce all visual assets using the detailed checklist above.
3. Integrate real IAP backend (RevenueCat) and remove simulation mode.
4. Run "Prepare for Device Build" + "Prepare Release Build" helpers and produce signed release candidates.
5. Execute full device test matrix (Top 10 checklist).
6. Create App Store / Play Console records with final bundle ID.
7. Submit for review.

---

*This document is the living single source of truth for all release preparation work.*

**Phase 7 will get Paper Wings World from "excellent internal demo" to "shipped on the App Store and Google Play."**