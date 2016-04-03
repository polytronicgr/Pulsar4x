using Pulsar4X.ECSLib;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Pulsar4X.ViewModel
{
    public class JobVM<TDataBlob, TJob> : IViewModel
        where TDataBlob : BaseDataBlob
    {
        private StaticDataStore _staticData;
        private IndustryJob _job;
        private Entity _colonyEntity;
        JobAbilityBaseVM<TDataBlob, TJob> _parentJobAbility { get; set; }

        public JobPriorityCommand<TDataBlob, TJob> JobPriorityCommand { get; set; }

        private int _jobTotalPoints;
        public string Item
        {
            get
            {
                if (_job.IndustryType == IndustryType.Refining)
                    return _staticData.RefinedMaterials[_job.ItemGuid].Name;
                else if (_job.IndustryType == IndustryType.InstallationConstruction)
                    return _colonyEntity.GetDataBlob<OwnedDB>().ObjectOwner.GetDataBlob<FactionInfoDB>().ComponentDesigns[_job.ItemGuid].GetDataBlob<NameDB>().DefaultName;
                else
                    return "Unknown Jobtype";

            }
        }

        public int Completed { get { return _job.NumberCompleted; } set { OnPropertyChanged(); } }
        public int BatchQuantity { get { return _job.NumberOrdered; } set { _job.NumberOrdered = value; OnPropertyChanged(); } } //note that we're directly changing the data here.
        public bool Repeat { get { return _job.AutoRepeat; } set { _job.AutoRepeat = value; OnPropertyChanged(); } } //note that we're directly changing the data here.

        public float ItemBuildPointsRemaining { get { return _job.BPPerItem - _job.PartialBPApplied; } set { OnPropertyChanged(); } }
        public double ItemPercentRemaining { get { return ItemBuildPointsRemaining / _job.BPPerItem; } set { OnPropertyChanged(); } }

        
        public JobVM()
        {
        }


        public JobVM(StaticDataStore staticData, Entity colonyEntity, IndustryJob job, JobAbilityBaseVM<TDataBlob, TJob> parentJobAbilityVM)
        {
            _staticData = staticData;
            _colonyEntity = colonyEntity;
            _job = job;
            _parentJobAbility = parentJobAbilityVM;
            
            if (_job.IndustryType == IndustryType.Refining)
                    _jobTotalPoints = _staticData.RefinedMaterials[_job.ItemGuid].RefinaryPointCost;
            else if (_job.IndustryType == IndustryType.InstallationConstruction)
                _jobTotalPoints = _colonyEntity.GetDataBlob<OwnedDB>().ObjectOwner.GetDataBlob<FactionInfoDB>().ComponentDesigns[_job.ItemGuid].GetDataBlob<ComponentInfoDB>().BuildPointCost;

            JobPriorityCommand = new JobPriorityCommand<TDataBlob, TJob>(this);
        }

        public void ChangePriority(int delta)
        {
            _parentJobAbility.ChangeJobPriority(_job, delta);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public void Refresh(bool partialRefresh = false)
        {
            if (PropertyChanged != null)
            {
                Completed = Completed;
                BatchQuantity = BatchQuantity;
                Repeat = Repeat;
                ItemBuildPointsRemaining = ItemBuildPointsRemaining;
                ItemPercentRemaining = ItemPercentRemaining;
            }
        }
    }
}
