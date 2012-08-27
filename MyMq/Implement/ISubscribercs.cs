using System;
using System.Collections.Generic;
using System.Text;

namespace MyMq
{
    public interface ISubscribercs
    {
        event ReceiveMessageEventHandler OnReceiveMessageEventHandler;
        void UnSubscribe(string topicName);
        void Subscribe(string topicName);
    }
}
