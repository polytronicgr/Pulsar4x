using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
namespace Pulsar4X.ECSLib
{
    public class MessagePumpServer
    {

        ConcurrentQueue<string> IncommingMessages = new ConcurrentQueue<string>();
        ConcurrentDictionary<Guid, ConcurrentQueue<BaseMessage>> OutGoingQueus = new ConcurrentDictionary<Guid, ConcurrentQueue<BaseMessage>>();

        internal void HandleMessage(BaseMessage message)
        {
            message.HandleMessage();
        }

        public void ReadInLoop()
        {
            string messageStr;
            while(IncommingMessages.TryDequeue(out messageStr))
            {
                BaseMessage messageObj = ObjectSerializer.DeserializeObject<BaseMessage>(messageStr);
                messageObj.HandleMessage();
            }
        }
    }


    public abstract class BaseMessage
    {
        object message;
        Guid factionID;   
        public abstract void HandleMessage();
    }




}
