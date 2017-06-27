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
                if(_updatables.ContainsKey(message.GetDataCode))
                {
                    foreach (var item in _updatables[message.GetDataCode])
                    {
                        item.Update(message);
                    }
                }                  
            }
        }

        public void Subscribe(SubscriptionRequestMessage message, IHandleMessage requestingVM)
        {
            if(!_updatables.ContainsKey(message.DataCode))
                _updatables.Add(message.DataCode, new List<IHandleMessage>(){requestingVM});
            else 
                _updatables[message.DataCode].Add(requestingVM);
            
            MessagePump.EnqueueIncomingMessage(message);
        }
        
        
    }

    public interface IHandleMessage
    {
        void Update(BaseToClientMessage message);
    }
}