using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MyMq.Excepions
{
   public class SubscriberException:MqException
    {
        public SubscriberException()
        {
            
        }
        public SubscriberException(string message):base(message)
        {
            
        }
        public SubscriberException(string message,Exception innerException):base(message,innerException)
        {
            
        }
        public SubscriberException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
        }
    }
}
