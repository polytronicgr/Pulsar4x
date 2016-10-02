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
        public ObservableDictionary<Type, ComponentAtbsListVM> ComponentsDict { get; } = new ObservableDictionary<Type, ComponentAtbsListVM>();
        public ObservableCollection<ComponentAtbsListVM> ComponentsList { get; } = new ObservableCollection<ComponentAtbsListVM>();// { return ComponentsDict.Values.ToList(); } }
        public FactionComponentListVM(Entity factionEntity)
        {
            _factionEntity = factionEntity;
            FactionInfoDB factionInfo = factionEntity.GetDataBlob<FactionInfoDB>();
            foreach (var design in factionInfo.ComponentDesigns.Values)
            {
                foreach (var item in design.DataBlobs.Where(db => db is IAttributeDescription))
                {

                    Type type = item.GetType();
                    if (!ComponentsDict.ContainsKey(type))
                    {
                        ComponentsDict.Add(type, new ComponentAtbsListVM(item));
                        var newitem = new ComponentAtbsListVM(item);
                        ComponentsList.Add(newitem);
                        newitem.AddDesign(design);
                    }
                    ComponentsDict[type].AddDesign(design);
                   
                }
            }
        }
    }


    public class ComponentAtbsListVM 
    {
        internal Type AtbType { get; }
        public string AtbName { get; private set; }
        public string AtbDescription { get; private set; }
        public ObservableCollection<ComponentDesignDetailVM> DesignsList { get; } = new ObservableCollection<ComponentDesignDetailVM>();

        public ComponentAtbsListVM(BaseDataBlob atbType) 
        {
            AtbType = atbType.GetType();
            IAttributeDescription atbDesc = (IAttributeDescription)atbType;
            AtbName = atbDesc.Name;
            AtbDescription = atbDesc.Description;

        }

        public void AddDesign(Entity design)
        {
            DesignsList.Add(new ComponentDesignDetailVM(design));
        }
    }

    public class ComponentDesignDetailVM
    {
        public string DesignName { get; private set; }
        public string DesignDescription { get; private set; }
        public List<IAttributeDescription> AttributeList { get; } = new List<IAttributeDescription>();

        private Entity _designEntity;        
        public Guid EntityID { get { return _designEntity.Guid; } }
        
        public ComponentDesignDetailVM(Entity design)
        {
            _designEntity = design;
            DesignName = _designEntity.GetDataBlob<NameDB>().DefaultName;

            foreach (var item in design.DataBlobs.Where(db => db is IAttributeDescription))
            {
                AttributeList.Add((IAttributeDescription)item);
            }
        }

    }

}
