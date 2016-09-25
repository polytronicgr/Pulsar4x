using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsar4X.ECSLib
{
    public interface AttributeDescription
    {
        string Name { get; }
        string Description { get; }
        double Value { get; }
    }
}
