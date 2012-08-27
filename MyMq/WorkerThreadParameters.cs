using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MyMq
{
   internal class WorkerThreadParameters
    {
       private TcpClient _tcpServer;
       private List<TcpClient> _tcpClientsSubscriberListForThisTopic;
       public System.Net.Sockets.Socket Server { get; set; }

        public object Message
        {
            get; set;
        }

        public List<EndPoint> SubscriberListForThisTopic
        { get; set; }

       public TcpClient TcpServer
       {
           get { return _tcpServer; }
           set { _tcpServer = value; }
       }

       public List<TcpClient> TcpClientsSubscriberListForThisTopic
       {
           get { return _tcpClientsSubscriberListForThisTopic; }
           set { _tcpClientsSubscriberListForThisTopic = value; }
       }
    }
}
