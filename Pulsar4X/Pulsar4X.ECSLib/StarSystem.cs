using System;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    [JsonObject(MemberSerialization.OptIn)]
    public class StarSystem
    {   
        /// <summary>
        /// An id used to prevent serilization of the star system multpule times.
        /// This will be diffreent every time a system is generated, even it it used the same seed.
        /// </summary>
        [PublicAPI]
        [JsonProperty]
        public Guid Guid { get; private set; }      
        
        [PublicAPI]
        public int Seed
        {
            get { return _seed; }
        }

        [JsonProperty]
        private readonly int _seed;




        [PublicAPI]
        public NameDB NameDB
        {
            get { return _nameDB; }
        }

        [JsonProperty]
        private readonly NameDB _nameDB;

        internal Random RNG { get; private set; }

        internal int EconLastTickRun
        {
            get { return _econLastTickRun; }
            set { _econLastTickRun = value; }
        }

        [JsonProperty]
        private int _econLastTickRun;

        public EntityManager SystemManager
        {
            get { return _systemManager; }
        }

        [JsonProperty("SystemManager")]
        private readonly EntityManager _systemManager;

        public StarSystem()
        {
        }

        public StarSystem(Guid guid)
        {
            Guid = guid;
        }

        public StarSystem(Game game, string name, int seed)
        {
            _systemManager = new EntityManager(game);
            _nameDB = new NameDB(name);
            _seed = seed;
            RNG = new Random(seed);
            EconLastTickRun = 0;
            Guid = Guid.NewGuid();  // give the system a unique id... this should be the only thing that is different about a system generated with the same id.
        }
    }
}
