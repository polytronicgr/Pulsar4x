using System;
using System.CodeDom;
using Pulsar4X.ECSLib;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Pulsar4X.ViewModel
{
    public class JobVM : IViewModel
    {
        private readonly IndustryJob _job;
        private IndustrialEntity _industrialEntity;

        public string Item => _job.ItemName;

        internal Entity projectManager => _job.ProjectManager;
        public IndustryType IndustryType => _job.IndustryType;
        internal Guid itemGuid => _job.ItemGuid;

        public int NumberOrdered
        {
            get { return _job.NumberOrdered; }
            set
            {
                _job.NumberOrdered = value;
                OnPropertyChanged();
            }
        }

        public int NumberCompleted => _job.NumberCompleted;

        public bool AutoRepeat
        {
            get { return _job.AutoRepeat; }
            set
            {
                _job.AutoRepeat = value;
                OnPropertyChanged();
            }
        }

        public float BPToNextItem => _job.BPToNextItem;

        public float PartialBPApplied => _job.PartialBPApplied;

        public float PercentToUtilized
        {
            get { return _job.PercentToUtilize; }
            set
            {
                _job.PercentToUtilize = value;
                OnPropertyChanged();
            }
        }

        public float TotalBPApplied => _job.TotalBPApplied;
        public float TotalBPRequired => _job.TotalBPRequired;
        public float PercentCompleted => _job.PercentCompleted;

        public DateTime ProjectCompletion => _job.ProjectedCompletion;

        public JobVM(Entity entity, IndustryJob job)
        {
            _industrialEntity = new IndustrialEntity(entity);
            _job = job;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Refresh(bool partialRefresh = false)
        {
            OnPropertyChanged();
        }
    }
}
