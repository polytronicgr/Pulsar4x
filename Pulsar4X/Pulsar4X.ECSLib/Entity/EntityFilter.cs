using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsar4X.ECSLib
{
    public class EntityFilter
    {
        public ComparableBitArray ContainsDataBlobs { get; }
        public ComparableBitArray ExcludesDataBlobs { get; }

        private static ComparableBitArray BlankDataBlobMask { get; } = EntityManager.BlankDataBlobMask();

        public bool MatchesEntity(Entity entity)
        {
            ComparableBitArray entityDataBlobMask = entity.DataBlobMask;
            if ((entityDataBlobMask & ContainsDataBlobs) != ContainsDataBlobs)
            {
                return false;
            }

            if ((entityDataBlobMask & ExcludesDataBlobs) != BlankDataBlobMask)
            {
                return false;
            }
            return true;
        }

        public EntityFilter(ComparableBitArray containsMask, ComparableBitArray excludesMask)
        {
            ContainsDataBlobs = new ComparableBitArray(containsMask);
            ExcludesDataBlobs = new ComparableBitArray(excludesMask);
        }
    }
}
