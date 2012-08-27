using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace MyMq
{
    public class TcpProduct : IProduct
    {
        private TcpClient _client;
        private IPEndPoint _remoteEndPoint;

        private static readonly TcpProduct Product;
        static TcpProduct()
        {
            Product = new TcpProduct();
        }
        public static TcpProduct Default
        {
            get { return Product; }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="serverIP">Server IP</param>
        /// <param name="serverPort">Server Port</param>
        public void Init(string serverIP, int serverPort)
        {
            IPAddress serverIPAddress = IPAddress.Parse(serverIP);
            _client = new TcpClient();
            _remoteEndPoint = new IPEndPoint(serverIPAddress, serverPort);
            _client.Connect(_remoteEndPoint);
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
            NetPacket packet = new NetPacket();
            packet.Command = command;

            if (_client.Connected == false)
            {
                _client = new TcpClient();
                _client.Connect(_remoteEndPoint);
            }
            NetworkStream stream = _client.GetStream();
            //using (NetworkStream stream = _client.GetStream())
            {
                if (stream.CanWrite)
                {
                NetPacketTcpAsynService asynService =     new NetPacketTcpAsynService(stream);
                    asynService.OnAfterSendPacket += new TcpAsynHandler2( delegate(NetPacketHead h) { stream.Close();});
                asynService.SendMessage(packet);
                }
            }
        }
    }
}
