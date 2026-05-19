# Regions

## Overview

Phase 3 introduces multiple distinct flying regions. Each region is defined as a `FlightRegion` ScriptableObject and can have its own:

- Visual identity (skybox, lighting, fog)
- Wind and thermal behavior
- Challenge targets (distance, glide time)
- Future: terrain, music, special mechanics

## Current Regions (Foundation)

| Region            | ID                | Character                  | Wind Strength | Thermal Strength | Distance Goal | Glide Time Goal |
|-------------------|-------------------|----------------------------|---------------|------------------|---------------|-----------------|
| Grand Canyon      | grand_canyon      | Dramatic relief, balanced  | Medium        | Medium           | 650m          | 52s             |
| Fuji Foothills    | fuji_foothills    | Volcanic, strong lift      | Light         | High             | 480m          | 68s             |
| Norwegian Coast   | norwegian_coast   | Windy fjords, fast travel  | Strong        | Low              | 720m          | 41s             |

## How Regions Work

1. Player completes a paper airplane in the folding screen.
2. On success, a region selection appears (currently 3 buttons for foundation).
3. Player chooses a region.
4. "Launch to Flight" passes both the plane and region via `FlightSessionData`.
5. `FlightDemoBootstrap` reads the region and configures:
   - Sky / lighting / fog
   - Wind direction & strength
   - Spawn height
   - (Future) specific environment objects / thermals

## Adding a New Region

1. Create a new `FlightRegion` asset (`Paper Wings → Generate Default Flight Regions` can be extended).
2. Fill in visual and flight parameters.
3. Add it to the `FlightRegionLibrary` (when one is created).
4. Add a button/option in the region selector UI in `FoldingTutorialManager`.
5. (Optional) Create a custom `FlightEnvironment` variant for that region.

## Future Expansion Ideas

- Region-specific particle effects (cherry blossoms in Fuji, sea spray on coast)
- Different background music / ambient sounds
- Unique challenges or collectibles per region
- Progressive unlock (complete challenges in one region to unlock the next)
- Larger world with seamless transitions between regions (long-term)

## Related Systems

- `FlightRegion.cs`
- `FlightRegionLibrary.cs`
- `FlightSessionData.cs`
- `FlightDemoBootstrap.cs` (region application logic)
- `FoldingTutorialManager.cs` (region selection UI after success)