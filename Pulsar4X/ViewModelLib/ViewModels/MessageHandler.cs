using System;
using System.Collections.Generic;
using Pulsar4X.ECSLib;
using Pulsar4X.ECSLib.DataSubscription;

namespace Pulsar4X.ViewModel
{
    public class MessageHandler
    {
        private MessagePumpServer MessagePump { get; }
        
        private readonly Dictionary<string, IUpdateable> _updatables = new Dictionary<string, IUpdateable>();

        public MessageHandler(MessagePumpServer messagePump) { MessagePump = messagePump; }

        public void Read()
        {
            BaseToClientMessage message;
            while( MessagePump.TryDequeueOutgoingMessage(Guid.Empty, out message))
            {
                if(message is UIData)
                    UpdateVM((UIData)message);
            }
        }

        public void Subscribe(SubscriptionRequestMessage message, IUpdateable requestingVM)
        {
            _updatables.Add(message.DataCode, requestingVM);
            MessagePump.EnqueueIncomingMessage(message);
        }

        public void UpdateVM(UIData uidata)
        {
            _updatables[uidata.GetDataCode].Update();
        }
    }

    public interface IUpdateable
    {
        void Update();
    }
}