using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MyMq.Excepions
{
    public class MqException:Exception
    {
        public MqException()
        {
            
        }
        public MqException(string message):base(message)
        {
            
        }
        public MqException(string message,Exception innerException):base(message,innerException)
        {
            
        }
        public MqException(SerializationInfo info, StreamingContext context):base(info,context)
        {
            
        }
    }
}
