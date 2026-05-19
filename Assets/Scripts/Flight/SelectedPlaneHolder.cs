using UnityEngine;
using PaperWings.Folding;

namespace PaperWings.Flight
{
    /// <summary>
    /// Simple static holder used during development to pass the chosen plane
    /// from the Folding scene to the Flight scene.
    /// In a full game this would be replaced by a proper state manager or scene transition system.
    /// </summary>
    public static class SelectedPlaneHolder
    {
        public static PaperPlaneDefinition SelectedPlane { get; private set; }

        public static void SetPlane(PaperPlaneDefinition plane)
        {
            SelectedPlane = plane;
        }

        public static void Clear()
        {
            SelectedPlane = null;
        }
    }
}