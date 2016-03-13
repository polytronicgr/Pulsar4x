using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public enum CommanderType
    {
        Invalid = 0,
        Navy,
        Ground,
        Administrator,
        Scientist,
    }

    public class LeaderDB : BaseDataBlob
    {
        [JsonProperty]
        public Entity AssignedTo { get; internal set; } = Entity.InvalidEntity;
        [JsonProperty]
        public bool IsFemale { get; internal set; }
        [JsonProperty]
        public int Rank { get; internal set; }
        [JsonProperty]
        public CommanderType Type { get; internal set; }

        public LeaderDB(bool isFemale = false, int rank = 0, CommanderType type = CommanderType.Invalid)
        {
            IsFemale = isFemale;
            Rank = rank;
            Type = type;
        }
        
        public override object Clone()
        {
            return new LeaderDB {AssignedTo = AssignedTo, IsFemale = IsFemale, Rank = Rank, Type = Type};
        }
    }
}