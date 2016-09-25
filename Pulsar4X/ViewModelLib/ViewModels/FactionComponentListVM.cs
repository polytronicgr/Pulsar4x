using Pulsar4X.ECSLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsar4X.ViewModel
{
    public class FactionComponentListVM
    {
        private Entity _factionEntity;
        public ObservableDictionary<Type, ComponentsByAtbListVM> ComponentsList { get; } = new ObservableDictionary<Type, ComponentsByAtbListVM>();
        public FactionComponentListVM(Entity factionEntity)
        {
            FactionInfoDB factionInfo = factionEntity.GetDataBlob<FactionInfoDB>();
            foreach (var design in factionInfo.ComponentDesigns.Values)
            {
                foreach (var item in design.DataBlobs.Where(db => db is AttributeDescription))
                {

                    Type type = item.GetType();
                    if (!ComponentsList.ContainsKey(type))
                    {
                        ComponentsList.Add(type, new ComponentsByAtbListVM(design));
                    }
                    else      
                        ComponentsList[type].AddDesign(design);
                }
            }
        }
    }


    public class ComponentsByAtbListVM 
    {
        public string AtributeString { get; private set; }
        public ObservableCollection<ComponentSpecificDesignVM> DesignsList { get; } = new ObservableCollection<ComponentSpecificDesignVM>();

        public ComponentsByAtbListVM(Entity design)
        {


        }

        public void AddDesign(Entity design)
        {
            
        }
    }

    public class ComponentDesignDetailVM
    {
        public string DesignName { get; private set; }
        public string DesignDescription { get; private set; }
        public List<AttributeDescription> AttributeList { get; } = new List<AttributeDescription>();

        private Entity _designEntity;        
        public Guid EntityID { get { return _designEntity.Guid; } }
        
        public ComponentDesignDetailVM(Entity design)
        {
            DesignName = design.GetDataBlob<NameDB>().DefaultName;

            foreach (var item in design.DataBlobs.Where(db => db is AttributeDescription))
            {
                AttributeList.Add((AttributeDescription)item);
            }
        }

    }

}
