using UnityEngine;
using PaperWings.Folding;

namespace PaperWings.Flight
{
    /// <summary>
    /// Holds the current flight session data (plane + region) between scenes.
    /// This is a temporary solution during development. In a full build this
    /// would be replaced by a proper persistent game state manager.
    /// </summary>
    public static class FlightSessionData
    {
        public static PaperPlaneDefinition SelectedPlane { get; private set; }
        public static FlightRegion SelectedRegion { get; private set; }

        public static void SetSession(PaperPlaneDefinition plane, FlightRegion region)
        {
            SelectedPlane = plane;
            SelectedRegion = region ?? GetDefaultRegion();
        }

        public static void Clear()
        {
            SelectedPlane = null;
            SelectedRegion = null;
        }

        private static FlightRegion GetDefaultRegion()
        {
            // Fallback if no library is set up yet
            return null;
        }
    }
}