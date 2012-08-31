﻿using System.Net;
using System.Net.Sockets;

namespace MyMq
{
    /// <summary>
    /// 消息生产者
    /// </summary>
    public class SocketProduct : IProduct
    {
        private Socket _client;
        private EndPoint _remoteEndPoint;

        private static readonly SocketProduct _product;
        static SocketProduct()
        {
            _product = new SocketProduct();
        }
        public static SocketProduct Default
        {
            get { return _product; }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="serverIP">Server IP</param>
        /// <param name="serverPort">Server Port</param>
        public void Init(string serverIP, int serverPort)
        {
            IPAddress serverIPAddress = IPAddress.Parse(serverIP);
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _remoteEndPoint = new IPEndPoint(serverIPAddress, serverPort);
        }
        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="data"></param>
        public void Send(string topicName, object data)
        {
            ICommand command = new PublishCommand();
            command.Data = data;
            command.TopicName = topicName; 
            _client.SendTo(SerializeHelper.ObjectToBytes(command), _remoteEndPoint);
        }

        public event SendErrorHandler SendMessageError;
    }
}
