using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Pulsar4X.ECSLib;

namespace Pulsar4X.ViewModel
{
    public class IndustrialEntityVM : IViewModel
    {
        private IndustrialEntity _industrialEntity;

        public Dictionary<IndustryType, ObservableCollection<JobVM>> Jobs;  

        public event PropertyChangedEventHandler PropertyChanged;

        public IndustrialEntityVM(Entity entity)
        {
            _industrialEntity = new IndustrialEntity(entity);
        }

        public void Refresh(bool partialRefresh = false)
        {
            _industrialEntity = new IndustrialEntity(_industrialEntity.Entity);
        }
    }
}
