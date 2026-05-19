# How to Test the Full Experience (Folding + Real 3D Models + Flight) — Updated for Tomorrow

## Prerequisites
- Unity 6+
- Both scenes added to Build Settings:
  - `FoldingTutorialDemo`
  - `FlightDemo`

## Step-by-step (5–7 minutes)

1. Open the project in Unity 6.

2. Generate all plane data + real 3D models:
   - **Paper Wings → HIGH INTENSITY - Generate All 8 MVP Planes + Demo Data**
   - **Paper Wings → Generate Low-Poly Rigged Paper Planes (All 8)**
   - **Paper Wings → Assign Real Models to Key PaperPlaneDefinitions**  
     (This assigns the new 3D prefabs to Classic Dart, The Ring, Nakamichi Glider, and The Bird)

3. (Re)create the demo scenes if needed:
   - **Paper Wings → HIGH INTENSITY - Create Playable Demo Scene**
   - **Paper Wings → HIGH INTENSITY - Create FlightDemo Scene**

4. Open `FoldingTutorialDemo.unity` and press **Play**.

5. In the plane selection grid, choose one of the planes with real models (recommended order):
   - Classic Dart
   - The Ring
   - Nakamichi Glider
   - The Bird

6. Fold the plane completely. You should now see the proper low-poly 3D model with natural folding animation.

7. On the success screen, press **"Launch to Flight →"**.

You will load into the FlightDemo scene flying the real 3D model with correct physics, thermals, flutter, stats, and the improved camera (including right-side free-look).

## Controls

**Folding:**
- Tap to advance steps (animations play automatically)
- Touch orbit on the 3D model

**Flight:**
- Swipe anywhere → Steer (pitch + roll)
- Drag on the **right half of the screen** → Free-look camera
- Fly through green thermal zones for extra lift

**General:**
- "Return to Folding" button in flight scene

## Notes
- The other 4 planes still use the procedural fallback (you can assign their prefabs manually if desired).
- All models use the same bone names, so animations and future real art swaps will work seamlessly.

Test the full loop (fold → launch → fly → return) and let me know how the new 3D models feel!