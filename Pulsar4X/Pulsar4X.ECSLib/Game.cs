using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Pulsar4X.ECSLib
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Game
    {
        #region Properties

        [PublicAPI]
        [JsonProperty]
        public List<Player> Players;
        
        [PublicAPI]
        [JsonProperty]
        public Player SpaceMaster;

        [PublicAPI]
        [JsonProperty]
        public Entity GameMasterFaction;

        [PublicAPI]
        public bool IsLoaded { get; internal set; } = false;

        [PublicAPI]
        public DateTime CurrentDateTime
        {
            get { return _currentDateTime; }
            internal set { _currentDateTime = value; }
        }
        [JsonProperty]
        private DateTime _currentDateTime;

        /// <summary>
        /// List of StarSystems currently in the game.
        /// </summary>
        [JsonProperty]
        internal Dictionary<Guid, StarSystem> Systems { get; private set; } = new Dictionary<Guid, StarSystem>();

        [JsonProperty] public readonly EntityManager GlobalManager;

        [PublicAPI]
        [JsonProperty]
        public StaticDataStore StaticData { get; internal set; } = new StaticDataStore();

        [CanBeNull]
        [PublicAPI]
        public PulseInterrupt CurrentInterrupt { get; private set; }

        [PublicAPI]
        public SubpulseLimit NextSubpulse { get; private set; } = new SubpulseLimit();

        [JsonProperty]
        internal GalaxyFactory GalaxyGen { get; private set; }

        [PublicAPI]
        public EventLog EventLog { get; internal set; }

        internal readonly Dictionary<Guid, EntityManager> GlobalGuidDictionary = new Dictionary<Guid, EntityManager>();
        internal readonly ReaderWriterLockSlim GuidDictionaryLock = new ReaderWriterLockSlim();
        private PathfindingManager _pathfindingManager;

        [PublicAPI]
        [JsonProperty]
        public GameSettings Settings { get; set; }

        public bool IsRunning { get; private set; }

        public bool Quit { get; set; }

        public double TimeMultiplier { get; set; }

        [JsonProperty]
        private readonly OrbitProcessor _orbitProcessor = new OrbitProcessor();

        [JsonProperty]
        private readonly EconProcessor _econProcessor;

        private readonly object _syncRoot = new object();

        #endregion

        #region Events
        /// <summary>
        /// PostLoad event fired when the game is loaded.
        /// Event is cleared each load.
        /// </summary>
        internal event EventHandler PostLoad;

        /// <summary>
        /// FrameUpdate event fired each time the engine initiates a FrameUpdate.
        /// The UI should register a function for this event and send it through the dispatcher
        /// to update ViewModels
        /// </summary>
        internal event EventHandler<long> FrameUpdate;

        /// <summary>
        /// FixedUpdate event fired each time the engine initiates a FixedUpdate.
        /// THis will be handled internally and will deal with firing the processors.
        /// </summary>
        internal event EventHandler<long> FixedUpdate;
        #endregion

        #region Constructors

        internal Game()
        {
            _econProcessor = new EconProcessor(this);
            GlobalManager = new EntityManager(this);
        }

        public Game([NotNull] NewGameSettings newGameSettings) : this()
        {
            if (newGameSettings == null)
            {
                throw new ArgumentNullException(nameof(newGameSettings));
            }

            GalaxyGen = new GalaxyFactory(true);

            Settings = newGameSettings;
            CurrentDateTime = newGameSettings.StartDateTime;

            // Load Static Data
            if (newGameSettings.DataSets != null)
            {
                foreach (string dataSet in newGameSettings.DataSets)
                {
                    StaticDataManager.LoadData(dataSet, this);
                }
            }
            if (StaticData.LoadedDataSets.Count == 0)
            {
                StaticDataManager.LoadData("Pulsar4x", this);
            }
            
            // Create SM
            SpaceMaster = new Player("Space Master", newGameSettings.SMPassword);
            Players = new List<Player>();
            GameMasterFaction = FactionFactory.CreatePlayerFaction(this, SpaceMaster, "SpaceMaster Faction");

            if (newGameSettings.CreatePlayerFaction ?? false)
            {
                Player defaultPlayer = AddPlayer(newGameSettings.DefaultPlayerName, newGameSettings.DefaultPlayerPassword);

                if (newGameSettings.DefaultSolStart ?? false)
                {
                    DefaultStartFactory.DefaultHumans(this, defaultPlayer, newGameSettings.DefaultFactionName);
                }
                else
                {
                    FactionFactory.CreatePlayerFaction(this, defaultPlayer, newGameSettings.DefaultFactionName);
                }
            }

            // Temp: This will be reworked later.
            GenerateSystems(new AuthenticationToken(SpaceMaster, newGameSettings.SMPassword), newGameSettings.MaxSystems);

            // Fire PostLoad event
            PostLoad += (sender, args) => { InitializeProcessors(); };
        }

        #endregion

        #region Functions

        #region Internal Functions



        internal void PostGameLoad()
        {
            _pathfindingManager = new PathfindingManager(this);

            // Invoke the Post Load event down the chain.
            PostLoad?.Invoke(this, EventArgs.Empty);

            // set isLoaded to true:
            IsLoaded = true;

            // Post load event completed. Drop all handlers.
            PostLoad = null;
        }

        /// <summary>
        /// Prepares, and defines the order that processors are run in.
        /// </summary>
        private static void InitializeProcessors()
        {
            MovementProcessor.Initialize();
            //InstallationProcessor.Initialize();
        }

        #endregion

        #region Public API

        [PublicAPI]
        public void RunEngine()
        {
            const double _fixedUpdateFrequency = 1; // Update frequency in Hz. This will cause the engine to update 1 time each (real second * TimeMultiplier).

            lock (_syncRoot)
            {
                if (IsRunning)
                {
                    // Game loop is already running. Don't try to run another.
                    return;
                }
                IsRunning = true;
            }
            long currentPhysicsTime = 0;
            long frameAccumulation = 0;
            // Number of ticks for each fixedUpdate.
            long fixedUpdateTicks = Convert.ToInt64((1 / _fixedUpdateFrequency) * Stopwatch.Frequency);
            float timeSpanConversionFactor = (float)TimeSpan.TicksPerSecond / Stopwatch.Frequency;
            long currentRealTime = 0;

            var gameTime = new Stopwatch();

            // Game Loop
            gameTime.Start();
            while (!Quit)
            {
                // Current time
                long newTime = gameTime.ElapsedTicks;
                // How long the previous frame took
                long frameTicks = Convert.ToInt64((newTime - currentRealTime) * TimeMultiplier);
                currentRealTime = newTime;
                frameAccumulation += frameTicks;

                while (frameAccumulation >= fixedUpdateTicks)
                {
                    FixedUpdate?.Invoke(this, fixedUpdateTicks);
                    frameAccumulation -= fixedUpdateTicks;
                    currentPhysicsTime += fixedUpdateTicks;
                }

                FrameUpdate?.Invoke(this, frameTicks);
            }

            IsRunning = false;
        }

        /// <summary>
        /// Time advancement code. Attempts to advance time by the number of seconds
        /// passed to it.
        /// Interrupts may prevent the entire requested timeframe from being advanced.
        /// </summary>
        /// <param name="deltaSeconds">Time Advance Requested</param>
        /// <param name="progress">IProgress implementation to report progress.</param>
        /// <returns>Total Time Advanced</returns>
        /// <exception cref="OperationCanceledException">Thrown when a cancellation request is honored.</exception>
        [PublicAPI]
        public int AdvanceTime(int deltaSeconds, IProgress<double> progress = null)
        {
            return AdvanceTime(deltaSeconds, CancellationToken.None, progress);
        }

        /// <summary>
        /// Time advancement code. Attempts to advance time by the number of seconds
        /// passed to it.
        /// 
        /// Interrupts may prevent the entire requested timeframe from being advanced.
        /// </summary>
        /// <param name="deltaSeconds">Time Advance Requested</param>
        /// <param name="cancellationToken">Cancellation token for this request.</param>
        /// <param name="progress">IProgress implementation to report progress.</param>
        /// <exception cref="OperationCanceledException">Thrown when a cancellation request is honored.</exception>
        /// <returns>Total Time Advanced (in seconds)</returns>
        [PublicAPI]
        public int AdvanceTime(int deltaSeconds, CancellationToken cancellationToken, IProgress<double> progress = null)
        {
            int timeAdvanced = 0;

            // Clamp deltaSeconds to a multiple of our MinimumTimestep.
            deltaSeconds = deltaSeconds - deltaSeconds % GameConstants.MinimumTimestep;
            if (deltaSeconds == 0)
            {
                deltaSeconds = GameConstants.MinimumTimestep;
            }

            // Clear any interrupt flag before starting the pulse.
            CurrentInterrupt = null;
            while ((CurrentInterrupt == null) && (deltaSeconds > 0))
            {
                cancellationToken.ThrowIfCancellationRequested();
                int subpulseTime = Math.Min(NextSubpulse.MaxSeconds, deltaSeconds);
                // Set next subpulse to max value. If it needs to be shortened, it will
                // be shortened in the pulse execution.
                NextSubpulse.MaxSeconds = int.MaxValue;

                // Update our date.
                CurrentDateTime += TimeSpan.FromSeconds(subpulseTime);

                // Execute all processors. Magic happens here.
                RunProcessors(Systems.Values.ToList(), deltaSeconds);

                // Update our remaining values.
                deltaSeconds -= subpulseTime;
                timeAdvanced += subpulseTime;
                progress?.Report((double)timeAdvanced / deltaSeconds);
            }

            if (CurrentInterrupt != null)
            {
                // Gamelog?
            }
            return timeAdvanced;
        }

        /// <summary>
        /// Runs all processors on the list of systems provided.
        /// </summary>
        /// <param name="systems">Systems to have processors run on them.</param>
        /// <param name="deltaSeconds">Game-time to progress in the processors.</param>
        [PublicAPI]
        public void RunProcessors(List<StarSystem> systems, int deltaSeconds)
        {
            _orbitProcessor.Process(this, systems);
            MovementProcessor.Process(this, systems, deltaSeconds);
            
            _econProcessor.Process(this, systems, deltaSeconds);
        }

        [PublicAPI]
        [NotNull]
        public Player AddPlayer(string playerName, string playerPassword = "")
        {
            var player = new Player(playerName, playerPassword);
            Players.Add(player);
            return player;
        }

        [PublicAPI]
        public List<StarSystem> GetSystems(AuthenticationToken authToken)
        {
            Player player = GetPlayerForToken(authToken);
            var systems = new List<StarSystem>();

            if (player?.AccessRoles == null)
            {
                return systems;
            }

            foreach (KeyValuePair<Entity, AccessRole> accessRole in player.AccessRoles)
            {
                // TODO: Implement vision access roles.
                if ((accessRole.Value & AccessRole.FullAccess) == AccessRole.FullAccess)
                {
                    systems.AddRange(accessRole.Key.GetDataBlob<FactionInfoDB>().KnownSystems.Select(systemGuid => Systems[systemGuid]));
                }
            }
            return systems;
        }

        [PublicAPI]
        [CanBeNull]
        public StarSystem GetSystem(AuthenticationToken authToken, Guid systemGuid)
        {
            Player player = GetPlayerForToken(authToken);

            if (player?.AccessRoles == null)
            {
                return null;
            }

            foreach (KeyValuePair<Entity, AccessRole> accessRole in player.AccessRoles)
            {
                // TODO: Implement vision access roles.
                if ((accessRole.Value & AccessRole.FullAccess) == AccessRole.FullAccess)
                {
                    foreach (Guid system in accessRole.Key.GetDataBlob<FactionInfoDB>().KnownSystems.Where(system => system == systemGuid))
                    {
                        return Systems[system];
                    }
                }
            }

            return null;
        }

        [CanBeNull]
        [PublicAPI]
        public Player GetPlayerForToken(AuthenticationToken authToken)
        {
            if (SpaceMaster.IsTokenValid(authToken))
            {
                return SpaceMaster;
            }

            foreach (Player player in Players.Where(player => player.ID == authToken?.PlayerID))
            {
                return player.IsTokenValid(authToken) ? player : null;
            }

            return null;
        }

        [PublicAPI]
        public void GenerateSystems(AuthenticationToken authToken, int numSystems)
        {
            var systemSeeds = new List<int>(numSystems);

            for (int i = 0; i < numSystems; i++)
            {
                systemSeeds.Add(GalaxyGen.SeedRNG.Next());
            }

            GenerateSystems(authToken, systemSeeds);
        }

        [PublicAPI]
        public void GenerateSystems(AuthenticationToken authToken, List<int> systemSeeds)
        {
            if (SpaceMaster.IsTokenValid(authToken))
            {
                foreach (int systemSeed in systemSeeds)
                {
                    GalaxyGen.StarSystemFactory.CreateSystem(this, $"Star System #{Systems.Count + 1}", systemSeed);
                }
            }
        }

        #endregion

        #endregion
    }
}
