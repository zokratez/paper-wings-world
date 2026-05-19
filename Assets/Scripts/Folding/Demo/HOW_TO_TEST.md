# How to Test the Folding System Right Now (Early Phase 1)

## Step-by-step (takes ~3 minutes)

1. Open the project in Unity 6.
2. Run the menu item:
   **Paper Wings → Create MVP Plane Assets (v1.0 Locked 8)**
3. This creates 8 `PaperPlaneDefinition` assets + `PaperPlaneLibrary.asset`.
4. Create a new empty scene called `FoldingTutorialDemo`.
5. Create an empty GameObject called "Folding System".
6. Add these components:
   - `FoldingTutorialManager`
   - `FoldingDemoSetup`
7. Create two UIDocuments:
   - One using `PlaneSelectionScreen.uxml`
   - One using `FoldingScreen.uxml`
8. Assign them to the `FoldingTutorialManager`.
9. Create a simple 3D plane using the `SimplePaperPlaneGenerator` (or just a Cube for now).
10. Add the `PaperModelOrbitController` to your main camera and point it at the paper model.

You should now be able to see the selection grid (populated from the 8 assets) and click a plane to enter the folding view with working touch orbit.

## Touch Controls (already implemented)

- 1 finger drag → Orbit the paper model
- 2 finger pinch → Zoom in/out

This is the foundation. Real folding steps + animations come next once you approve the current state.