using System;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public enum CommandType
    {
        Invalid = 0,
        Crewed,
        Flag,
    }

    public class CommandAbilityDB : BaseDataBlob
    {
        [JsonProperty]
        public CommandType CommandType { get; internal set; }

        [JsonConstructor]
        public CommandAbilityDB(CommandType commandType = CommandType.Invalid)
        {
            CommandType = commandType;
        }
        
        public override object Clone()
        {
            return new CommandAbilityDB(CommandType);
        }
    }
}
