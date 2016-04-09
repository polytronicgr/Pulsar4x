using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pulsar4X.ECSLib
{
    public class ComponentInstancesDB : BaseDataBlob
    {
        [JsonProperty]
        internal Dictionary<Entity, List<ComponentInstance>> specificInstances { get; set; } = new Dictionary<Entity, List<ComponentInstance>>();

        [PublicAPI]
        public IReadOnlyDictionary<Entity, IReadOnlyCollection<ComponentInstance>> SpecificInstances => specificInstances.ToDictionary(componentKVP => componentKVP.Key, componentKVP => (IReadOnlyCollection<ComponentInstance>)componentKVP.Value);

        public ComponentInstancesDB() { }

        public ComponentInstancesDB(IEnumerable<Entity> componentDesigns)
        {
            foreach (var item in componentDesigns)
            {
                ComponentInstance instance = new ComponentInstance(item);
                if (!specificInstances.ContainsKey(item))
                    specificInstances.Add(item, new List<ComponentInstance>() { instance });
                else
                {
                    specificInstances[item].Add(instance);
                }
            }
        }

        public ComponentInstancesDB(ComponentInstancesDB db)
        {
            specificInstances = new Dictionary<Entity, List<ComponentInstance>>(db.specificInstances);
        }

        public override object Clone()
        {
            return new ComponentInstancesDB(this);
        }
    }

    public class ComponentInstance
    {
        public Entity DesignEntity { get; internal set; }
        public bool IsEnabled { get; internal set; }
        public int HTKRemaining { get; internal set; }
        public object StateInfo { get; internal set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="designEntity">The Component Entity, MUST have a ComponentDB</param>
        /// <param name="isEnabled">whether the component is enabled on construction. default=true</param>
        internal ComponentInstance(Entity designEntity, bool isEnabled = true)
        {
            ComponentDB componentInfo = designEntity.GetDataBlob<ComponentDB>();

            if (componentInfo == null)
            {
                throw new Exception("designEntity Must contain a ComponentDB");
            }

            DesignEntity = designEntity;
            IsEnabled = isEnabled;
            HTKRemaining = componentInfo.HTK;
        }
    }
}
