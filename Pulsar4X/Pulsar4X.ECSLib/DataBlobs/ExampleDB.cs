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
using System;
using System.Runtime.Serialization;

namespace Pulsar4X.ECSLib
{
    // All datablobs must be public.
    public class BasicExampleDB
        : BaseDataBlob
    // All datablobs MUST derive from BaseDataBlob
    // If they don't they wont work with the EntityManager.
    // EntityManager does quite a bit of work to shift things around
    // and it's not easy to understand everything the EntityManager does. So just derive from BaseDataBlob.
    {
        // Any value you want to save must be marked with [JsonProperty] attribute.
        [JsonProperty]
        private int _importantNumber;
        [JsonProperty]
        private ICloneable _cloneableObj;

        private BasicExampleDB()
        {
            // This is called the "default parameterless constructor" most datablobs will need one of these
            // so that JSON, the code that restores a game from the disk, can instinate an object of this type.
            // It doesn't need to actually do anything, it just has to exist. See: ComponentDB
            // This is AKA the "JSON Constructor"
        }

        // Datablobs need to implement a deep-copy constructor.
        public BasicExampleDB(BasicExampleDB clone)
        {
            _importantNumber = clone._importantNumber;
            _cloneableObj = (ICloneable)_cloneableObj.Clone();
        }

        // Datablobs must implement the IClonable interface.
        // Most datablobs simply call their own constructor like so:
        public override object Clone()
        {
            return new BasicExampleDB(this);
        }
    }

    public class IntermediateExampleDB : BaseDataBlob
    {
        // Any non-private variables need to be encapsulated with a Property.
        private int _viewableInt;

        [JsonProperty]
        public int ViewableInt
        {
            get { return _viewableInt; }
            // In order to invoke INotifyPropertyChanged events, use SetField to set the value.
            set { SetField(ref _viewableInt, value); }
        }

        // The above is actually equivilent to this, use this format for reabibility.
        private int _viewableInt2;
        [PublicAPI]
        [JsonProperty]
        public int ViewableInt2 { get { return _viewableInt2; } set { SetField(ref _viewableInt2, value); } }

    [PublicAPI]
        public void BadFunction()
        {
            // DataBlobs are DATA, not LOGIC. You should have MINIMAL logic in datablobs.
            // About the only acceptable LOGIC in a datablob is for unit conversion, derived property calculation,
            // or automatic object resolution. See MassVolumeDB, OrbitDB, and TreeHierarchyDB for respective examples.

            // So where do you put the logic? In the Processors! See OrbitProcessor for a great example!
        }

        #region Stuff we already talked about.

        private IntermediateExampleDB()
        {
        }

        public IntermediateExampleDB(IntermediateExampleDB clone)
        {
            _viewableInt = clone.ViewableInt;
            ViewableInt2 = clone.ViewableInt2;
        }

        public override object Clone()
        {
            return new IntermediateExampleDB(this);
        }

        #endregion
    }

    public class AdvancedExampleDB : BaseDataBlob
    {
        // References to other entities are ok. The EntityManager will handle serializing and deserializing the references properly.
        [PublicAPI]
        public Entity FriendEntity
        {
            get { return _friendEntity; }
            internal set { SetField(ref _friendEntity, value); }
        }

        [JsonProperty]
        private Entity _friendEntity;

        // References to StarSystems that are SERIALIZED are NOT ok.
        [JsonProperty]
        public StarSystem MySystem; // BAD

        // Instead, either store the guid and look up the system when needed (from the Game.Systems dictionary)
        [JsonProperty]
        public Guid MySystemGuid;

        // Or if you want to get really fancy, use a deserialization callback to resolve the star system after load-time.
        public StarSystem MyStarSystem;

        // JSON deserialization callback.
        [OnDeserialized]
        private void Deserialized(StreamingContext context)
        {
            // Star system resolver loads myStarSystem from mySystemGuid after the game is done loading.
            var game = (Game)context.Context;
            game.PostLoad += (sender, args) => { if (!game.Systems.TryGetValue(MySystemGuid, out MyStarSystem)) throw new GuidNotFoundException(MySystemGuid); };
        }

        #region Stuff we already talked about.

        private AdvancedExampleDB()
        {
        }

        public AdvancedExampleDB(AdvancedExampleDB clone)
        {
            MySystemGuid = clone.MySystemGuid;
            _friendEntity = clone.FriendEntity;
        }

        public override object Clone()
        {
            return new AdvancedExampleDB(this);
        }

        #endregion
    }
}
