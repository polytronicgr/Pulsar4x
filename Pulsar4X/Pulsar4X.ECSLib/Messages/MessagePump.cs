using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Pulsar4X.ECSLib.DataSubscription;

namespace Pulsar4X.ECSLib
{
    public class MessagePump
    {
        private readonly ConcurrentQueue<BaseToServerMessage> _incomingMessages = new ConcurrentQueue<BaseToServerMessage>();
        private readonly ConcurrentDictionary<Guid, ConcurrentQueue<BaseToClientMessage>> _outgoingQueues = new ConcurrentDictionary<Guid, ConcurrentQueue<BaseToClientMessage>>();

        public MessagePump()
        {
            //local UIconnection is an empty guid.
            AddNewConnection(Guid.Empty);
        }

        public void AddNewConnection(Guid connectionID) => _outgoingQueues.TryAdd(connectionID, new ConcurrentQueue<BaseToClientMessage>());
        public void RemoveConnection(Guid connectionID) => _outgoingQueues.TryRemove(connectionID, out ConcurrentQueue<BaseToClientMessage> unusedValue);

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

        public void EnqueueOutgoingMessage(Guid toConnection, BaseToClientMessage message)
        {
            if(_outgoingQueues.ContainsKey(toConnection))
                _outgoingQueues[toConnection].Enqueue(message);
            else
            {
                //TODO: log NoConnection message.
            }
        }

        public bool TryPeekOutgoingMessage(Guid connction, out BaseToClientMessage message) => _outgoingQueues[connction].TryPeek(out message);
        public bool TryDequeueOutgoingMessage(Guid connection, out BaseToClientMessage message) => _outgoingQueues[connection].TryDequeue(out message);

        public void ReadIncomingMessages(Game game)
        {
            while (_incomingMessages.TryDequeue(out BaseToServerMessage message))
            {
                message.HandleMessage(game);
            }
        }
    }


    public abstract class BaseToServerMessage
    {
        [JsonProperty]
        public Guid FactionGuid { get; set; }
        [JsonProperty]
        public Guid ConnectionID { get; set; }
        
        internal abstract void HandleMessage(Game game);
    }

    public abstract class BaseToClientMessage
    {
        public abstract string GetDataCode { get; }
    }
    
    public class GameOrder : BaseToServerMessage
    {
        [JsonProperty]
        public IncomingMessageType MessageType { get; set; }
        [JsonProperty]
        private string Message { get; }
        
        public GameOrder(IncomingMessageType messageType) { MessageType = messageType; }
        
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
    }



}
