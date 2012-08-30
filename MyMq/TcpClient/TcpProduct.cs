using System;
using System.Net;
using System.Net.Sockets;
using MyMq.Excepions;

namespace MyMq
{
    /// <summary>
    /// 消息发布者
    /// </summary>
    public class TcpProduct : IProduct
    {
        private IPEndPoint _remoteEndPoint;
        /// <summary>
        /// 发送失败事件
        /// </summary>
        public event SendErrorHandler OnSendErrorHandler;

        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="serverIP">Server IP</param>
        /// <param name="serverPort">Server Port</param>
        public void Init(string serverIP, int serverPort)
        {
            IPAddress serverIPAddress = IPAddress.Parse(serverIP);
            _remoteEndPoint = new IPEndPoint(serverIPAddress, serverPort);
        }
        #endregion

        #region 异步发送数据
        /// <summary>
        /// 创建数据包
        /// </summary>
        private NetPacket CreateNetPacket(string topicName, object data)
        {
            ICommand command = new PublishCommand();
            command.Data = data;
            command.TopicName = topicName;
            NetPacket packet = new NetPacket();
            packet.Command = command;
            return packet;
        }
        private TcpClient ConnectRemote(IPEndPoint ipEndPoint)
        {
            TcpClient client = new TcpClient();
            try
            {
                client.Connect(_remoteEndPoint);
            }
            catch (SocketException socketException)
            {
                if (socketException.SocketErrorCode == SocketError.ConnectionRefused)
                {
                    throw new ProducerException("服务端拒绝连接",
                                                  socketException.InnerException ?? socketException);
                }
                if (socketException.SocketErrorCode == SocketError.HostDown)
                {
                    throw new ProducerException("订阅者服务端尚未启动",
                                                  socketException.InnerException ?? socketException);
                }
                if (socketException.SocketErrorCode == SocketError.TimedOut)
                {
                    throw new ProducerException("网络超时",
                                                  socketException.InnerException ?? socketException);
                }
                throw new ProducerException("未知错误",
                                                  socketException.InnerException ?? socketException);
            }
            catch (Exception e)
            {
                throw new SubscriberException("未知错误", e.InnerException ?? e);
            }
            return client;
        }
        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="topicName">主题</param>
        /// <param name="data">数据</param>
        public void Send(string topicName, object data)
        {
            TcpClient client = ConnectRemote(this._remoteEndPoint);
            NetworkStream stream = client.GetStream();
            if (stream.CanWrite)
            {
                NetPacketTcpAsynService asynService = new NetPacketTcpAsynService(stream);
                asynService.OnAfterSendPacket += new AfterSendPacketHandler(delegate(NetPacketHead h)
                                                                                {
                                                                                    client.Close();
                                                                                    stream.Close();
                                                                                });
                asynService.OnSendErrorHandler += new SendErrorHandler(asynService_OnSendErrorHandler);
                asynService.SendMessage(CreateNetPacket(topicName, data));
            }
        }

        void asynService_OnSendErrorHandler(NetServiceErrorReason reason)
        {
            if (OnSendErrorHandler != null)
            {
                OnSendErrorHandler(reason);
            }
        }
        #endregion

    }
}
