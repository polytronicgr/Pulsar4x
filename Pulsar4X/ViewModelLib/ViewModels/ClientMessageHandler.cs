using System;
using System.Collections.Generic;
using Pulsar4X.ECSLib;
using Pulsar4X.ECSLib.DataSubscription;

namespace Pulsar4X.ViewModel
{
    public class ClientMessageHandler
    {
        private MessagePumpServer MessagePump { get; }
        
        private readonly Dictionary<Type, List<IHandleMessage>> _updatables = new Dictionary<Type, List<IHandleMessage>>();

        public ClientMessageHandler(MessagePumpServer messagePump) { MessagePump = messagePump; }

        public void Read()
        {
            BaseToClientMessage message;
            while( MessagePump.TryDequeueOutgoingMessage(Guid.Empty, out message))
            {
                if(_updatables.ContainsKey(message.GetType()))
                {
                    foreach (var item in _updatables[message.GetType()])
                    {
                        item.Update(message);
                    }
                }                  
            }
        }

        public void Subscribe<T>(SubscriptionRequestMessage<T> message, IHandleMessage requestingVM) where T: BaseToClientMessage
        {
            if(!_updatables.ContainsKey(typeof(T)))
                _updatables.Add(typeof(T), new List<IHandleMessage>(){requestingVM});
            else 
                _updatables[typeof(T)].Add(requestingVM);
            
            MessagePump.EnqueueIncomingMessage(message);
        }     
    }

    public interface IHandleMessage
    {
        void Update(BaseToClientMessage message);
    }
}