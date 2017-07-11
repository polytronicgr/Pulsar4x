using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Pulsar4X.ECSLib;
using Pulsar4X.ECSLib.DataSubscription;

namespace Pulsar4X.ViewModel
{
    /// <summary>
    /// This view model and corrisponding view serves as a container for subcontrols 
    /// which corrispond to the entitys processor/databobs 
    /// </summary>
    public class GenericEntityVM : IHandleMessage
    {
        private Guid EntityGuid { get; set; }
        
        public ObservableCollection<DataBlobVMBase> ChildVMs { get; } = new ObservableCollection<DataBlobVMBase>();
        
        public string Name { get; set; }

        GenericEntityVM(ClientMessageHandler messageHandler, Guid entityGuid)
        {
            EntityGuid = entityGuid;
            

            
            
            
            /*
            
            * get datablob info for entity
            
            foreach (var item in entityDatablobs)
            {
                add a viewmodel for each datablob  
            }
            
            *Create notifications and listners for adding and removal of datablobs to the entity
            *Create notifications and listers for data changes to the datablobs, and pass them to the respective viewmodels
            
            */
        }

        public void Update(BaseToClientMessage message) { throw new NotImplementedException(); }
    }



    public class DataBlobVMBase
    {
    }

    public class CargoDBVM : DataBlobVMBase
    {
        
    }
}