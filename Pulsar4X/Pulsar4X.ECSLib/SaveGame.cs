using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Text;

namespace Pulsar4X.ECSLib
{
    /// <summary>
    /// This class is responsible for saving a game to/from disk.
    /// </summary>
    // use: http://www.newtonsoft.com/json/help/html/SerializationAttributes.htm
    public static class SaveGame
    {
        private static readonly JsonSerializer DefaultSerializer = new JsonSerializer { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.Indented, ContractResolver = new ForceUseISerializable() };

        internal static Game CurrentGame { get; private set; }
        internal static IProgress<double> Progress { get; private set; }
        internal static int ManagersProcessed { get; set; }
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// Saves the game to a file defined by filePath using the default serializer.
        /// </summary>
        [PublicAPI]
        public static void Save([NotNull] Game game, [NotNull] string filePath, IProgress<double> progress = null, bool compress = false)
        {
            CheckFile(filePath, FileAccess.Write);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                Save(game, fileStream, progress, compress);
            }
        }

        /// <summary>
        /// Saves the game to the provided stream using the default serializer.
        /// </summary>
        [PublicAPI]
        public static void Save([NotNull] Game game, [NotNull] Stream outputStream,  IProgress<double> progress = null, bool compress = false)
        {
            if (game == null)
            {
                throw new ArgumentNullException("game");
            }

            DefaultSerializer.Formatting = compress ? Formatting.None : Formatting.Indented;

            lock (SyncRoot)
            {
                Progress = progress;
                ManagersProcessed = 0;

                // Wrap the outputStream in a BufferedStream.
                // This will improves performance if the outputStream does not have an internal buffer. (E.G. NetworkStream)
                using (BufferedStream outputBuffer = new BufferedStream(outputStream))
                {
                    using (MemoryStream intermediateStream = new MemoryStream())
                    {
                        using (StreamWriter streamWriter = new StreamWriter(intermediateStream, Encoding.UTF8, 1024, true))
                        {
                            using (JsonWriter writer = new JsonTextWriter(streamWriter))
                            {
                                CurrentGame = game;
                                DefaultSerializer.Serialize(writer, game);
                                CurrentGame = null;
                            }
                        }

                        // Reset the MemoryStream's position to 0. CopyTo copies from Position to the end.
                        intermediateStream.Position = 0;

                        if (compress)
                        {
                            using (GZipStream compressionStream = new GZipStream(outputBuffer, CompressionLevel.Optimal))
                            {
                                intermediateStream.CopyTo(compressionStream);
                            }
                        }
                        else
                        {
                            intermediateStream.CopyTo(outputBuffer);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// saves an entity to a given stream
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="outputStream"></param>
        /// <param name="progress"></param>
        /// <param name="compress"></param>
        [PublicAPI]
        public static void ExportEntity([NotNull] Entity entity, [NotNull] Stream outputStream, IProgress<double> progress = null, bool compress = false)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            DefaultSerializer.Formatting = compress ? Formatting.None : Formatting.Indented;

            lock (SyncRoot)
            {
                Progress = progress;
                ManagersProcessed = 0;
                //entity.NumSystems = entity.StarSystems.Count;

                // Wrap the outputStream in a BufferedStream.
                // This will improves performance if the outputStream does not have an internal buffer. (E.G. NetworkStream)
                using (BufferedStream outputBuffer = new BufferedStream(outputStream))
                {
                    using (MemoryStream intermediateStream = new MemoryStream())
                    {
                        using (StreamWriter streamWriter = new StreamWriter(intermediateStream, Encoding.UTF8, 1024, true))
                        {
                            using (JsonWriter writer = new JsonTextWriter(streamWriter))
                            {
                                writer.WriteStartObject(); // Start the Entity.
                                writer.WritePropertyName("Guid"); // Write the Guid PropertyName
                                DefaultSerializer.Serialize(writer, entity.Guid); // Write the Entity's guid.
                                foreach (BaseDataBlob dataBlob in entity.DataBlobs)
                                {
                                    writer.WritePropertyName(dataBlob.GetType().Name); // Write the PropertyName of the dataBlob as the dataBlob's type.
                                    DefaultSerializer.Serialize(writer, dataBlob); // Serialize the dataBlob in this property.
                                }
                                writer.WriteEndObject(); // End then Entity.
                            }
                        }

                        // Reset the MemoryStream's position to 0. CopyTo copies from Position to the end.
                        intermediateStream.Position = 0;

                        if (compress)
                        {
                            using (GZipStream compressionStream = new GZipStream(outputBuffer, CompressionLevel.Optimal))
                            {
                                intermediateStream.CopyTo(compressionStream);
                            }
                        }
                        else
                        {
                            intermediateStream.CopyTo(outputBuffer);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// saves an entity to a given stream
        /// </summary>
        /// <param name="starSystem"></param>
        /// <param name="outputStream"></param>
        /// <param name="progress"></param>
        /// <param name="compress"></param>
        [PublicAPI]
        public static void ExportStarSystem([NotNull] StarSystem starSystem, [NotNull] Stream outputStream, IProgress<double> progress = null, bool compress = false)
        {
            if (starSystem == null)
            {
                throw new ArgumentNullException("starSystem");
            }

            DefaultSerializer.Formatting = compress ? Formatting.None : Formatting.Indented;

            lock (SyncRoot)
            {
                Progress = progress;
                ManagersProcessed = 0;
                //entity.NumSystems = entity.StarSystems.Count;

                // Wrap the outputStream in a BufferedStream.
                // This will improves performance if the outputStream does not have an internal buffer. (E.G. NetworkStream)
                using (BufferedStream outputBuffer = new BufferedStream(outputStream))
                {
                    using (MemoryStream intermediateStream = new MemoryStream())
                    {
                        using (StreamWriter streamWriter = new StreamWriter(intermediateStream, Encoding.UTF8, 1024, true))
                        {
                            using (JsonWriter writer = new JsonTextWriter(streamWriter))
                            {
                                writer.WriteStartObject(); // Start the Entity.
                                writer.WritePropertyName("Guid"); // Write the Guid PropertyName
                                DefaultSerializer.Serialize(writer, starSystem.Guid);
                                writer.WritePropertyName("StarSystem");
                                DefaultSerializer.Serialize(writer, starSystem);
                                writer.WriteEndObject(); // End then Entity.
                            }
                        }

                        // Reset the MemoryStream's position to 0. CopyTo copies from Position to the end.
                        intermediateStream.Position = 0;

                        if (compress)
                        {
                            using (GZipStream compressionStream = new GZipStream(outputBuffer, CompressionLevel.Optimal))
                            {
                                intermediateStream.CopyTo(compressionStream);
                            }
                        }
                        else
                        {
                            intermediateStream.CopyTo(outputBuffer);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Loads the game from the file at the provided filePath using the default serializer.
        /// </summary>
        [PublicAPI]
        public static Game Load([NotNull] string filePath, IProgress<double> progress = null)
        {
            CheckFile(filePath, FileAccess.Read);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return Load(fileStream, progress);
            }
        }

        /// <summary>
        /// Loads the game from the provided stream using the default serializer.
        /// </summary>
        [PublicAPI]
        private static Game Load(Stream inputStream, IProgress<double> progress = null)
        {
            Game game = new Game();

            lock (SyncRoot)
            {
                Progress = progress;
                ManagersProcessed = 0;
                CurrentGame = game;
                // Use a BufferedStream to allow reading and seeking from any stream.
                // Example: If inputStream is a NetworkStream, then we can only read once.
                using (BufferedStream inputBuffer = new BufferedStream(inputStream))
                {
                    // Check if our stream is compressed.
                    if (HasGZipHeader(inputBuffer))
                    {
                        // File is compressed. Decompress using GZip.
                        using (GZipStream compressionStream = new GZipStream(inputStream, CompressionMode.Decompress))
                        {
                            // Decompress into a MemoryStream.
                            using (MemoryStream intermediateStream = new MemoryStream())
                            {
                                // Decompress the file into an intermediate MemoryStream.
                                compressionStream.CopyTo(intermediateStream);

                                // Reset the position of the MemoryStream so it can be read from the beginning.
                                intermediateStream.Position = 0;

                                // Populate the game from the uncompressed MemoryStream.
                                PopulateGame(intermediateStream);
                            }
                        }
                    }
                    else
                    {
                        // Populate the game from the uncompressed inputStream.
                        PopulateGame(inputBuffer);
                    }
                }

                // check the version info:
                if (game.Version.IsCompatibleWith(VersionInfo.PulsarVersionInfo) == false)
                {
                    string e = string.Format("The save file is not supported. the save is from version {0}, the game only supports versions: {1}", game.Version.VersionString, VersionInfo.PulsarVersionInfo.CompatibleVersions);

                    throw new NotSupportedException(e);
                }

                // get the game to do its post load stuff
                game.PostGameLoad();
                CurrentGame = null;
            }
            return game;
        }


        /// <summary>
        /// Imports from the provided stream into the provided game using the default serializer.
        /// </summary>
        [PublicAPI]
        public static Entity ImportEntity(Game game, EntityManager manager, Stream inputStream, IProgress<double> progress = null)
        {

            Entity entity;// = new Entity(manager, guid);
            lock (SyncRoot)
            {
                Progress = progress;
                ManagersProcessed = 0;
                // Use a BufferedStream to allow reading and seeking from any stream.
                // Example: If inputStream is a NetworkStream, then we can only read once.
                using (BufferedStream inputBuffer = new BufferedStream(inputStream))
                {
                    // Check if our stream is compressed.
                    if (HasGZipHeader(inputBuffer))
                    {
                        // File is compressed. Decompress using GZip.
                        using (GZipStream compressionStream = new GZipStream(inputStream, CompressionMode.Decompress))
                        {
                            // Decompress into a MemoryStream.
                            using (MemoryStream intermediateStream = new MemoryStream())
                            {
                                // Decompress the file into an intermediate MemoryStream.
                                compressionStream.CopyTo(intermediateStream);

                                // Reset the position of the MemoryStream so it can be read from the beginning.
                                intermediateStream.Position = 0;

                                // Populate the game from the uncompressed MemoryStream.
                                //PopulateEntity(entity, intermediateStream);
                                entity = PopulateEntity(intermediateStream, game, manager);
                            }
                        }
                    }
                    else
                    {
                        // Populate the game from the uncompressed inputStream.
                        entity = PopulateEntity(inputStream, game, manager);
                    }
                }

            }
            return entity;
        }

        /// <summary>
        /// Imports from the provided stream into the provided game using the default serializer.
        /// </summary>
        [PublicAPI]
        public static StarSystem ImportStarSystem(Game game, Stream inputStream, IProgress<double> progress = null)
        {

            StarSystem system;// = new Entity(manager, guid);
            lock (SyncRoot)
            {
                Progress = progress;
                ManagersProcessed = 0;
                // Use a BufferedStream to allow reading and seeking from any stream.
                // Example: If inputStream is a NetworkStream, then we can only read once.
                using (BufferedStream inputBuffer = new BufferedStream(inputStream))
                {
                    // Check if our stream is compressed.
                    if (HasGZipHeader(inputBuffer))
                    {
                        // File is compressed. Decompress using GZip.
                        using (GZipStream compressionStream = new GZipStream(inputStream, CompressionMode.Decompress))
                        {
                            // Decompress into a MemoryStream.
                            using (MemoryStream intermediateStream = new MemoryStream())
                            {
                                // Decompress the file into an intermediate MemoryStream.
                                compressionStream.CopyTo(intermediateStream);

                                // Reset the position of the MemoryStream so it can be read from the beginning.
                                intermediateStream.Position = 0;

                                // Populate the game from the uncompressed MemoryStream.
                                //PopulateEntity(entity, intermediateStream);
                                system = PopulateStarSystem(intermediateStream, game);
                            }
                        }
                    }
                    else
                    {
                        // Populate the game from the uncompressed inputStream.
                        system = PopulateStarSystem(inputStream, game);
                    }
                }

            }
            return system;
        }


        /// <summary>
        /// Check if we have a valid file.
        /// </summary>
        /// <param name="filePath">Path to the file to check.</param>
        /// <param name="fileAccess">Type of access to check for.</param>
        private static void CheckFile(string filePath, FileAccess fileAccess)
        {
            // Test writing the file. If there's any issues with the file, this will cause them to throw.
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException("filePath", "No valid file path provided.");
            }

            if ((fileAccess & FileAccess.Write) == FileAccess.Write)
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    byte[] bytes = Encoding.ASCII.GetBytes("Pulsar4X write text.");
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
            if ((fileAccess & FileAccess.Read) == FileAccess.Read)
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    fs.ReadByte();
                }
            }
        }

        /// <summary>
        /// Checks the stream for compression by looking for GZip header numbers.
        /// </summary>
        /// <returns></returns>
        private static bool HasGZipHeader(BufferedStream inputStream)
        {
            var headerBytes = new byte[2];

            int numBytes = inputStream.Read(headerBytes, 0, 2);
            inputStream.Position = 0;
            if (numBytes != 2)
            {
                return false;
            }
            // First two bytes should be 31 and 139 according to the GZip file format.
            // http://www.gzip.org/zlib/rfc-gzip.html#header-trailer
            return headerBytes[0] == 31 && headerBytes[1] == 139;
        }

        /// <summary>
        /// Populates the currently loading game from the passed uncompressed inputStream.
        /// </summary>
        /// <param name="inputStream">Uncompressed stream containing the game data.</param>
        private static void PopulateGame(Stream inputStream)
        {
            using (StreamReader sr = new StreamReader(inputStream))
            {
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    DefaultSerializer.Populate(reader, CurrentGame);
                }
            }
        }

        /// <summary>
        /// DeSerialises a single entity and imports it into the given game.
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="game"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        private static Entity PopulateEntity(Stream inputStream, Game game, EntityManager manager)
        {
            CurrentGame = game;
            using (StreamReader sr = new StreamReader(inputStream))
            {
                using (JsonReader reader = new JsonTextReader(sr))
                {   
                    var pname = reader.Read(); // PropertyName Guid
                    var giud = reader.Read(); // Actual Guid
                    var something = reader.Read();
                    Guid entityGuid = DefaultSerializer.Deserialize<Guid>(reader); // Deserialize the Guid

                    // Deserialize the dataBlobs
                    var dataBlobs = new List<BaseDataBlob>();
                    reader.Read(); // PropertyName DATABLOB
                    while (reader.TokenType == JsonToken.PropertyName)
                    {
                        Type dataBlobType = Type.GetType("Pulsar4X.ECSLib." + (string)reader.Value);
                        reader.Read(); // StartObject (dataBlob)
                        BaseDataBlob dataBlob = (BaseDataBlob)DefaultSerializer.Deserialize(reader, dataBlobType); // EndObject (dataBlob)
                        dataBlobs.Add(dataBlob);
                        reader.Read(); // PropertyName DATABLOB OR EndObject (Entity)
                    }


                    // Attempt a global Guid lookup of the Guid.
                    Entity entity;
                    if (game.GlobalManager.FindEntityByGuid(entityGuid, out entity))
                    {
                        foreach (BaseDataBlob dataBlob in dataBlobs)
                        {
                            entity.SetDataBlob(dataBlob);
                        }
                    }
                    else
                    {
                        entity = new Entity(manager, entityGuid, dataBlobs);
                    }
                    ReCalcProcessor.ReCalcAbilities(entity);
                    return entity;
                }
            }
        }

        private static StarSystem PopulateStarSystem(Stream inputStream, Game game)
        {
            CurrentGame = game;
            using (StreamReader sr = new StreamReader(inputStream))
            {
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    reader.Read(); // PropertyName Guid
                    reader.Read();
                    reader.Read();
                    Guid guid = DefaultSerializer.Deserialize<Guid>(reader); // Deserialize the Guid
  
                    // Attempt a global Guid lookup of the Guid.
                    StarSystem system;
                    if (!Misc.FindStarSystem(game.Systems, guid, out system))
                    {
                        system = new StarSystem(guid);                       
                    }
                    else
                    {
                        system = game.LookupStarSystem(guid);
                    }
                    reader.Read();
                    reader.Read();
                    DefaultSerializer.Populate(reader, system);      
                    game.StarSystems.Add(system);
                    return system;
                }
            }
        }
    }




}
