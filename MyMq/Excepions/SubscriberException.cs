using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MyMq.Excepions
{
    public class ConnectException:MqException
    {
        public ConnectException()
            : this("连接断开或尚未建立连接，请建立连接")
        {

        }
        public ConnectException(string message)
            : base(message)
        {

        }
        public ConnectException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
        public ConnectException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }

    /// <summary>
    /// 发布者异常
    /// </summary>
    public class ProducerException : MqException
    {
        public ProducerException()
        {

        }
        public ProducerException(string message)
            : base(message)
        {

        }
        public ProducerException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
        public ProducerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
    /// <summary>
    /// 订阅者异常
    /// </summary>
    public class SubscriberException : MqException
    {
        public SubscriberException()
        {

        }
        public SubscriberException(string message)
            : base(message)
        {

        }
        public SubscriberException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
        public SubscriberException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
