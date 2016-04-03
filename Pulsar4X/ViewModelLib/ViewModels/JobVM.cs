using System.CodeDom;
using Pulsar4X.ECSLib;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Pulsar4X.ViewModel
{
    public class JobVM<TDataBlob, TJob> : IViewModel
        where TDataBlob : BaseDataBlob
    {
        private Game _game;
        private StaticDataStore _staticData;
        private IndustryJob _job;
        private IndustrialEntity _industrialEntity;
        JobAbilityBaseVM<TDataBlob, TJob> _parentJobAbility { get; set; }

        public JobPriorityCommand<TDataBlob, TJob> JobPriorityCommand { get; set; }
        
        public string Item
        {
            get
            {
                switch (_job.IndustryType)
                {
                    case IndustryType.Refining:
                        return _staticData.RefinedMaterials[_job.ItemGuid].Name;
                    case IndustryType.InstallationConstruction:
                        Entity installationDesign = _game.GlobalManager.GetGlobalEntityByGuid(_job.ItemGuid);
                        return installationDesign.GetDataBlob<NameDB>()?.GetName(_industrialEntity.OwnedDB.EntityOwner) ?? "ERROR: NO NAMEDB FOUND FOR ENTITY";
                    default:
                        return "Unknown Jobtype";
                }
            }
        }

        public int Completed { get { return _job.NumberCompleted; } set { OnPropertyChanged(); } }
        public int BatchQuantity { get { return _job.NumberOrdered; } set { _job.NumberOrdered = value; OnPropertyChanged(); } } //note that we're directly changing the data here.
        public bool Repeat { get { return _job.AutoRepeat; } set { _job.AutoRepeat = value; OnPropertyChanged(); } } //note that we're directly changing the data here.

        public float ItemBuildPointsRemaining { get { return _job.BPPerItem - _job.PartialBPApplied; } set { OnPropertyChanged(); } }
        public double ItemPercentRemaining { get { return ItemBuildPointsRemaining / _job.BPPerItem; } set { OnPropertyChanged(); } }

        public JobVM(Game game, StaticDataStore staticData, Entity entity, IndustryJob job, JobAbilityBaseVM<TDataBlob, TJob> parentJobAbilityVM)
        {
            _game = game;
            _staticData = staticData;
            _industrialEntity = new IndustrialEntity(entity);
            _job = job;
            _parentJobAbility = parentJobAbilityVM;
            
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
