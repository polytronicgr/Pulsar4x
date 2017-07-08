#region Copyright/License
/* 
 *Copyright© 2017 Daniel Phelps
    This file is part of Pulsar4x.

    Pulsar4x is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Pulsar4x is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Pulsar4x.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public enum CommanderTypes
    {
        Navy,
        Ground,
        Civilian
    }

    public class CommanderDB : BaseDataBlob
    {
        private CommanderNameSD _name;
        private int _rank;
        private CommanderTypes _type;

        [JsonProperty]
        public CommanderNameSD Name { get { return _name; } internal set { SetField(ref _name, value); } }

        [JsonProperty]
        public int Rank { get { return _rank; } internal set { SetField(ref _rank, value); } }

        [JsonProperty]
        public CommanderTypes Type { get { return _type; } internal set { SetField(ref _type, value); } }

        public CommanderDB() { }

        public CommanderDB(CommanderNameSD name, int rank, CommanderTypes type)
        {
            Name = name;
            Rank = rank;
            Type = type;
        }

        public CommanderDB(CommanderDB commanderDB)
        {
            //Should we create new commander? I think no but we have rank in there and same commander with different ranks is not good.
            Name = commanderDB.Name;

            Rank = commanderDB.Rank;
            Type = commanderDB.Type;
        }

        public override object Clone()
        {
            return new CommanderDB(this);
        }
    }
}
