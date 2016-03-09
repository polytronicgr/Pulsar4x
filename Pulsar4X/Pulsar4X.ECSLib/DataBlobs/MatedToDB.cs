using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsar4X.ECSLib.DataBlobs
{
    /// <summary>
    /// DataBlob to define what this Entity is mated to.
    /// If mated, the entity will automatically follow the Root Entity.
    /// </summary>
    public class MatedToDB : TreeHierarchyDB
    {
        public MatedToDB(Entity parent) : base(parent) { }

        public override object Clone()
        {
            return new MatedToDB(Parent);
        }
    }
}
