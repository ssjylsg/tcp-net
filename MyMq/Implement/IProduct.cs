using System;
using System.Collections.Generic;
using System.Text;

namespace MyMq
{
    /// <summary>
    /// 消息发布
    /// </summary>
   public interface IProduct
   {
       void Init(string serverIP, int serverPort);
       void Send(string topicName, object data);
   }
}
