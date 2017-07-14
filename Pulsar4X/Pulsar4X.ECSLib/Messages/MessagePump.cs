#region Copyright/License
// Copyright© 2017 Daniel Phelps
//     This file is part of Pulsar4x.
// 
//     Pulsar4x is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     Pulsar4x is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with Pulsar4x.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using System;
using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    public class MessagePump
    {
        #region Fields
        private readonly ConcurrentQueue<BaseToServerMessage> _incomingMessages = new ConcurrentQueue<BaseToServerMessage>();
        private readonly ConcurrentDictionary<Guid, ConcurrentQueue<string>> _outgoingQueues = new ConcurrentDictionary<Guid, ConcurrentQueue<string>>();
        #endregion

        #region Constructors
        public MessagePump()
        {
            //local UIconnection is an empty guid.
            AddNewConnection(Guid.Empty);
        }
        #endregion

        #region Public Methods
        public void AddNewConnection(Guid connectionID) { _outgoingQueues.TryAdd(connectionID, new ConcurrentQueue<string>()); }

        public void RemoveConnection(Guid connectionID) { _outgoingQueues.TryRemove(connectionID, out ConcurrentQueue<string> unusedValue); }

        /// <summary>
        /// takes a seralised (derived type of)BaseToServerMessage as a string, deserialises it and enqueues it.
        /// </summary>
        /// <param name="message"></param>
        public void EnqueueIncomingMessage(string message)
        {
            EnqueueIncomingMessage(ObjectSerializer.DeserializeObject<BaseToServerMessage>(message));
        }

        /// <summary>
        /// Enqueus an message object
        /// </summary>
        /// <param name="message"></param>
        public void EnqueueIncomingMessage(BaseToServerMessage message)
        {
            _incomingMessages.Enqueue(message);
        }

        public void EnqueueOutgoingMessage(Guid toConnection, string message)
        {
            if (_outgoingQueues.ContainsKey(toConnection))
            {
                _outgoingQueues[toConnection].Enqueue(message);
            }
        }

        public bool TryPeekOutgoingMessage(Guid connction, out string message) { return _outgoingQueues[connction].TryPeek(out message); }

        public bool TryDequeueOutgoingMessage(Guid connection, out string message) { return _outgoingQueues[connection].TryDequeue(out message); }

        public void ReadIncomingMessages(Game game)
        {
            while (_incomingMessages.TryDequeue(out BaseToServerMessage message))
            {
                message.HandleMessage(game);
            }
        }
        #endregion
    }


    public abstract class BaseToServerMessage
    {
        #region Properties
        [JsonProperty]
        public Guid FactionGuid { get; set; }
        [JsonProperty]
        public Guid ConnectionID { get; set; }
        #endregion

        #region Internal Methods
        internal abstract void HandleMessage(Game game);
        #endregion
    }

    public abstract class BaseToClientMessage
    {
        #region Properties
        public abstract string GetDataCode { get; }
        #endregion
    }

    public class GameOrder : BaseToServerMessage
    {
        #region Properties
        [JsonProperty]
        public IncomingMessageType MessageType { get; set; }
        [JsonProperty]
        private string Message { get; }
        #endregion

        #region Constructors
        public GameOrder(IncomingMessageType messageType) { MessageType = messageType; }
        #endregion

        #region Internal Methods
        internal override void HandleMessage(Game game)
        {
            switch (MessageType)
            {
                case IncomingMessageType.Exit:
                    game.ExitRequested = true;
                    break;
                case IncomingMessageType.ExecutePulse:
                    // TODO: Pulse length parsing
                    game.GameLoop.TimeStep();
                    break;
                case IncomingMessageType.StartRealTime:
                    float timeMultiplier;
                    if (float.TryParse(Message, out timeMultiplier))
                    {
                        game.GameLoop.TimeMultiplier = timeMultiplier;
                    }
                    game.GameLoop.StartTime();
                    break;
                case IncomingMessageType.StopRealTime:
                    game.GameLoop.PauseTime();
                    break;
                case IncomingMessageType.Echo:
                    //UIInfoMessage newMessage = new UIInfoMessage("Echo from " + ConnectionID);
                    //game.MessagePump.EnqueueOutgoingMessage(ConnectionID, newMessage);
                    break;

                // This message may be getting too complex for this handler.
                /*
                case IncomingMessageType.GalaxyQuery:
                    var systemGuids = new StringBuilder();
                    foreach (StarSystem starSystem in game.GetSystems(authToken))
                    {
                        systemGuids.Append($"{starSystem.Guid:N},");
                    }

                    game.MessagePump.EnqueueOutgoingMessage(OutgoingMessageType.GalaxyResponse, systemGuids.ToString(0, systemGuids.Length - 1));
                    break;
                    */
            }
        }
        #endregion
    }
}