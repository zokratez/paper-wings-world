using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace PaperWings.Folding
{
    /// <summary>
    /// Central database for all paper planes.
    /// In production we will load this via Addressables for scalability.
    /// For Phase 1 we can use a single asset that references the 8 MVP planes.
    /// </summary>
    [CreateAssetMenu(fileName = "PaperPlaneLibrary", menuName = "Paper Wings / Paper Plane Library")]
    public class PaperPlaneLibrary : ScriptableObject
    {
        public List<PaperPlaneDefinition> allPlanes = new List<PaperPlaneDefinition>();

        public PaperPlaneDefinition GetPlaneById(string id)
        {
            return allPlanes.FirstOrDefault(p => p.planeId == id);
        }

        public List<PaperPlaneDefinition> GetFreePlanes()
        {
            return allPlanes.Where(p => p.isFree).ToList();
        }

        public List<PaperPlaneDefinition> GetPlanesByDifficulty(Core.DifficultyLevel difficulty)
        {
            return allPlanes.Where(p => p.difficulty == difficulty).ToList();
        }

        public List<PaperPlaneDefinition> GetPlanesByCategory(Core.PlaneCategory category)
        {
            return allPlanes.Where(p => p.primaryCategory == category).ToList();
        }
    }
}