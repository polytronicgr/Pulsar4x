using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Pulsar4X.ECSLib
{
    public static class Misc
    {
        /// <summary>
        /// Small utility function to lookup a star system based on its ID.
        /// This is needed by datablobs who only stor a systems ID when they reference it.
        /// </summary>
        /// <param name="Id">Guid of the star system.</param>
        /// <returns>The star system.</returns>
        [PublicAPI]
        public static StarSystem LookupStarSystem(ReadOnlyCollection<StarSystem> systems,  Guid id)
        {
            return systems.Single(i => i.Guid == id);
        }

        
        /// <summary>
        /// returns true if systems contains a system with guid, and out that starSystem
        /// </summary>
        /// <param name="systems">list of systems</param>
        /// <param name="guid">guid of starsystem</param>
        /// <param name="starSystem">the star system</param>
        /// <returns></returns>
        public static bool FindStarSystem(ReadOnlyCollection<StarSystem> systems, Guid guid, out StarSystem starSystem)
        {
            if (systems.Any(i => i.Guid == guid))
            {
                starSystem = systems.Single(i => i.Guid == guid);
                return true;
            }
            starSystem = null;
            return false;
        }

        /// <summary>
        /// checks stockpile againsed costs. 
        /// </summary>
        /// <param name="stockpile"></param>
        /// <param name="costs"></param>
        /// <returns>True if stockpile has all the items and amounts</returns>
        public static bool HasReqiredItems(JDictionary<Guid, int> stockpile, Dictionary<Guid, int> costs)
        {
            if (costs == null)
                return true;
            return costs.All(kvp => stockpile.ContainsKey(kvp.Key) && (stockpile[kvp.Key] >= kvp.Value));
        }

        /// <summary>
        /// subtracts costs from stockpile. 
        /// will throw a key not found if stockpile does not contain at item,
        /// will go into negitives if the key is found but there are less items in the stockpile than cost.  
        /// </summary>
        /// <param name="stockpile"></param>
        /// <param name="costs"></param>
        internal static void UseFromStockpile(JDictionary<Guid, int> stockpile, Dictionary<Guid, int> costs)
        {
            if (costs != null)
            {
                foreach (var kvp in costs)
                {
                    stockpile[kvp.Key] -= kvp.Value;
                }
            }
        }
    }
}
