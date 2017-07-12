using System;
using System.Collections.Generic;
using Pulsar4X.ECSLib;
using Pulsar4X.ECSLib.DataSubscription;

namespace Pulsar4X.ViewModel
{
    public class ClientMessageHandler
    {
        private MessagePumpServer MessagePump { get; }
        
        private readonly Dictionary<string, List<IHandleMessage>> _updatables = new Dictionary<string, List<IHandleMessage>>();

        public ClientMessageHandler(MessagePumpServer messagePump) { MessagePump = messagePump; }

        public void Read()
        {
            BaseToClientMessage message;
            while( MessagePump.TryDequeueOutgoingMessage(Guid.Empty, out message))
            {
                if(_updatables.ContainsKey(message.ResponseCode))
                {
                    foreach (var item in _updatables[message.ResponseCode])
                    {
                        item.Update(message);
                    }
                }              
            }
        }

        public void Subscribe<T>(SubscriptionRequestMessage<T> message, IHandleMessage requestingVM) where T: SubscribableDatablob
        {
            if(!_updatables.ContainsKey(message.ResponseCode))
                _updatables.Add(message.ResponseCode, new List<IHandleMessage>(){requestingVM});
            else 
                _updatables[message.ResponseCode].Add(requestingVM);
            
            MessagePump.EnqueueIncomingMessage(message);
        }
    }

    public interface IHandleMessage
    {
        void Update(BaseToClientMessage message);
    }
}