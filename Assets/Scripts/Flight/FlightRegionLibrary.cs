using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace PaperWings.Flight
{
    /// <summary>
    /// Central database for all available flight regions.
    /// </summary>
    [CreateAssetMenu(fileName = "FlightRegionLibrary", menuName = "Paper Wings / Flight Region Library")]
    public class FlightRegionLibrary : ScriptableObject
    {
        public List<FlightRegion> regions = new List<FlightRegion>();

        public FlightRegion GetRegionById(string id)
        {
            return regions.FirstOrDefault(r => r.regionId == id);
        }

        public FlightRegion GetDefaultRegion()
        {
            return regions.Count > 0 ? regions[0] : null;
        }
    }
}