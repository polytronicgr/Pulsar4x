using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Pulsar4X.ECSLib
{
    [JsonObject(MemberSerialization.OptIn)]
    public class StarSystem
    {
        public Game Game { get; private set; }

        
        [JsonProperty]
        public Guid Guid { get; private set; }

        [JsonProperty]
        internal int SystemIndex { get; set; }

        
        [JsonProperty]
        public NameDB NameDB { get; private set; }

        
        [JsonProperty]
        public EntityManager SystemManager { get; private set; }

        
        [JsonProperty]
        public int Seed { get; private set; }
        internal Random RNG { get; private set; }

        [JsonConstructor]
        internal StarSystem()
        {
        }

        public StarSystem(Game game, string name, int seed)
        {
            Game = game;
            Guid = Guid.NewGuid();
            NameDB = new NameDB(name);
            SystemManager = new EntityManager(game);
            Seed = seed;
            RNG = new Random(seed);

            game.Systems.Add(Guid, this);
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            Game = (Game)context.Context;
            Game.PostLoad += GameOnPostLoad;

        }

        private void GameOnPostLoad(object sender, EventArgs eventArgs)
        {
            if (!Game.Systems.ContainsKey(Guid))
            {
                Game.Systems.Add(Guid, this);
                Game.GameMasterFaction.GetDataBlob<FactionDB>().KnownSystems.Add(Guid);
            }
        }
    }
}
