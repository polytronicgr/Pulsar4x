using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public delegate void TickEventHandler(DateTime currentDateTime, int delta);

    [JsonObject(MemberSerialization.OptIn)]
    public class Game
    {
        #region Properties

        [PublicAPI]
        public string GameName
        {
            get { return _gameName; }
            internal set { _gameName = value; }
        }
        [JsonProperty]
        private string _gameName;

        [PublicAPI]
        public VersionInfo Version { get { return VersionInfo.PulsarVersionInfo;} }

        [PublicAPI]
        public bool IsLoaded { get; internal set; }

        [PublicAPI]
        public DateTime CurrentDateTime
        {
            get { return _currentDateTime; }
            internal set { _currentDateTime = value; }
        }
        [JsonProperty]
        private DateTime _currentDateTime;

        [JsonProperty]
        internal int NumSystems { get { return StarSystems.Count; } }

        [PublicAPI]
        public ReadOnlyCollection<StarSystem> Systems { get { return new ReadOnlyCollection<StarSystem>(StarSystems); } }

        [PublicAPI] 
        [JsonProperty]
        public Guid GameMasterFaction;

        /// <summary>
        /// Global Entity Manager.
        /// </summary>
        [PublicAPI]
        public EntityManager GlobalManager { get { return _globalManager; } }
        [JsonProperty]
        private readonly EntityManager _globalManager;

        [PublicAPI]
        [JsonProperty]
        public StaticDataStore StaticData { get; private set; }

        [CanBeNull]
        [PublicAPI]
        public PulseInterrupt CurrentInterrupt { get; private set; }

        [PublicAPI]
        public SubpulseLimit NextSubpulse { get; private set; }

        public event TickEventHandler TickEvent;

        [JsonProperty]
        internal GalaxyFactory GalaxyGen { get; private set; }

        /// <summary>
        /// List of StarSystems currently in the game.
        /// </summary>
        [JsonProperty]
        internal List<StarSystem> StarSystems { get; private set; }

        internal readonly Dictionary<Guid, EntityManager> GlobalGuidDictionary = new JDictionary<Guid, EntityManager>();
        internal readonly ReaderWriterLockSlim GuidDictionaryLock = new ReaderWriterLockSlim();
        
        [JsonProperty]
        public bool IsAuthoritative { get; private set; }

        #endregion

        #region Events
        /// <summary>
        /// PostLoad event fired when the game is loaded.
        /// Event is cleared each load.
        /// </summary>
        internal event EventHandler PostLoad;
        #endregion

        #region Constructors

        internal Game()
        {

            IsLoaded = false;
            _globalManager = new EntityManager(this);
            StarSystems = new List<StarSystem>();
            NextSubpulse = new SubpulseLimit();
            GalaxyGen = new GalaxyFactory(true);
            StaticData = new StaticDataStore();

            PostLoad += (sender, args) => { InitializeProcessors(); };
        }

        #endregion

        #region Functions

        #region Internal Functions

        internal void RunProcessors(List<StarSystem> systems, int deltaSeconds)
        {
            if (TickEvent != null)
            {
                TickEvent.Invoke(CurrentDateTime, deltaSeconds);
            }
            OrbitProcessor.Process(this, systems, deltaSeconds);
            ShipMovementProcessor.Process(this, systems,deltaSeconds);
            EconProcessor.Process(this, systems, deltaSeconds);
        }

        internal void PostGameLoad()
        {
            // Invoke the Post Load event down the chain.
            if (PostLoad != null)
                PostLoad(this, EventArgs.Empty);

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
            OrbitProcessor.Initialize();
            ShipMovementProcessor.Initialize();
            //InstallationProcessor.Initialize();
        }

        /// <summary>
        /// Small utility function to lookup a star system based on its ID.
        /// This is needed by datablobs who only stor a systems ID when they reference it.
        /// </summary>
        /// <param name="Id">Guid of the star system.</param>
        /// <returns>The star system.</returns>
        internal StarSystem LookupStarSystem(Guid Id)
        {
            return StarSystems.Single(i => i.Guid == Id);
        }

        #endregion

        #region Public API

        /// <summary>
        /// </summary>
        /// <param name="gameName"></param>
        /// <param name="startDateTime"></param>
        /// <param name="numSystems"></param>
        /// <param name="progress"></param>
        /// <param name="authoritative">false if this is a remote client</param>
        /// <exception cref="ArgumentNullException"><paramref name="gameName"/> is <see langword="null" />.</exception>
        /// <exception cref="StaticDataLoadException">Thrown in a variety of situations when StaticData could not be loaded.</exception>
        [PublicAPI]
        public static Game NewGame([NotNull] string gameName, DateTime startDateTime, int numSystems, IProgress<double> progress = null, bool authoritative = true, string gameMasterPass = "")
        {
            if (gameName == null)
            {
                throw new ArgumentNullException("gameName");
            }

            Game newGame = new Game {GameName = gameName, CurrentDateTime = startDateTime};
            newGame.IsAuthoritative = authoritative;
            // TODO: Provide options for loading other Static Data DataSets.
            if (authoritative)
            {
                FactionFactory.CreateGameMaster(newGame, gameMasterPass);  
            }
            
            newGame.StaticData = StaticDataManager.LoadFromDefaultDataDirectory();

            for (int i = 0; i < numSystems; i++)
            {
                StarSystem newSystem = newGame.GalaxyGen.StarSystemFactory.CreateSystem(newGame, "System #" + i);
                newGame.StarSystems.Add(newSystem);
                if (progress != null)
                {
                    progress.Report((double)newGame.StarSystems.Count / numSystems);
                }
            }

            newGame.PostGameLoad();
            
            return newGame;
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
            deltaSeconds = deltaSeconds - (deltaSeconds % GameConstants.MinimumTimestep);
            if (deltaSeconds == 0)
            {
                deltaSeconds = GameConstants.MinimumTimestep;
            }

            // Clear any interrupt flag before starting the pulse.
            CurrentInterrupt = null;

            while (CurrentInterrupt == null && deltaSeconds > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                int subpulseTime = Math.Min(NextSubpulse.MaxSeconds, deltaSeconds);
                // Set next subpulse to max value. If it needs to be shortened, it will
                // be shortened in the pulse execution.
                NextSubpulse.MaxSeconds = int.MaxValue;

                // Update our date.
                CurrentDateTime += TimeSpan.FromSeconds(subpulseTime);

                // Execute all processors. Magic happens here.
                RunProcessors(StarSystems, deltaSeconds);

                // Update our remaining values.
                deltaSeconds -= subpulseTime;
                timeAdvanced += subpulseTime;
                if (progress != null)
                {
                    progress.Report((double)timeAdvanced / deltaSeconds);
                }
            }

            if (CurrentInterrupt != null)
            {
                // Gamelog?
            }
            return timeAdvanced;
        }

        #endregion


        #endregion
    }
}
