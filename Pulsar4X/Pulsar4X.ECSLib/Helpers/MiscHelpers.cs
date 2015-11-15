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
            return systems.Single(i => i.Id == id);
        }

        public static bool HasReqiredItems(JDictionary<Guid, int> stockpile, Dictionary<Guid, int> costs)
        {
            if (costs == null)
                return true;
            return costs.All(kvp => stockpile.ContainsKey(kvp.Key) && (stockpile[kvp.Key] >= kvp.Value));
        }

        public static void UseFromStockpile(JDictionary<Guid, int> stockpile, Dictionary<Guid, int> costs)
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
