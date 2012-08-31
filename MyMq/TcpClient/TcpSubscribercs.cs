using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MyMq.Excepions;

namespace MyMq
{
    /// <summary>
    /// 消息订阅者 使用长连接
    /// </summary>
    public class TcpSubscribercs : ISubscribercs
    {
        #region 私有变量
        private TcpClient _client;
        private IPEndPoint _remoteEndPoint;
        private Boolean _isReceivingStarted = false;
        private object _objectLock = new object();
        #endregion

        #region 事件
        /// <summary>
        /// 订阅者接受到信息事件
        /// </summary>
        public event ReceiveMessageEventHandler OnReceiveMessageEventHandler;
        /// <summary>
        /// 接收失败事件
        /// </summary>
        public event ReceiveErrorHandler OnReceiveErrorHandler;

        private void ReceiveErrorHandler(NetServiceErrorReason reason)
        {
            ReceiveErrorHandler handler = OnReceiveErrorHandler;
            if (handler != null) handler(reason);
        }

        /// <summary>
        /// 事件处理
        /// </summary>
        /// <param name="message"></param>
        private void OnOnReceiveMessageEventHandler(object message)
        {
            ReceiveMessageEventHandler handler = OnReceiveMessageEventHandler;
            if (handler != null) handler(message);
        }
        #endregion

        #region 发送订阅/取消订阅数据
        /// <summary>
        /// 客户端是否连接
        /// </summary>
        public bool IsClientConnected
        {
            get
            {
                lock (_objectLock)
                {
                    return this._client.Connected;
                }
            }
        }
        /// <summary>
        /// 判断连接是否正常，如果断开抛出SubscriberException
        /// </summary>
        private void CheckConnectionSession()
        {
            if (this.IsClientConnected == false)
            {
                throw new ConnectException("连接断开或尚未建立连接，请建立连接");
            }
        }
        private void Send(byte[] buffer)
        {
            CheckConnectionSession();
            NetworkStream stream = _client.GetStream();
            if (stream.CanWrite)
            {
                CheckConnectionSession();
                stream.Write(buffer, 0, buffer.Length);
            }
        }
        #endregion

        #region 订阅构造函数
        /// <summary>
        /// 订阅构造函数
        /// </summary>
        /// <param name="serverIp">Server IP</param>
        /// <param name="serverPort">Server Port</param>
        /// <param name="sleepTimeSpan"></param>
        public TcpSubscribercs(string serverIp, int serverPort, TimeSpan sleepTimeSpan)
        {
            if (string.IsNullOrEmpty(serverIp))
            {
                throw new SubscriberException("订阅ServerIP 不能为空");
            }
            if (serverPort == 0)
            {
                throw new SubscriberException("订阅Server Port 不正确");
            }

            IPAddress serverIPAddress = IPAddress.Parse(serverIp);
            _remoteEndPoint = new IPEndPoint(serverIPAddress, serverPort);
            _client = new TcpClient();
            ReConnectServer();
        }
        #endregion

        #region 重新连接订阅服务
        /// <summary> 
        /// 重新连接订阅服务
        /// </summary>
        public void ReConnectServer()
        {
            try
            {
                _client.Connect(_remoteEndPoint);
            }
            catch (SocketException socketException)
            {
                if (socketException.SocketErrorCode == SocketError.ConnectionRefused)
                {
                    throw new SubscriberException("服务端拒绝连接",
                                                  socketException.InnerException ?? socketException);
                }
                if (socketException.SocketErrorCode == SocketError.HostDown)
                {
                    throw new SubscriberException("订阅者服务端尚未启动",
                                                  socketException.InnerException ?? socketException);
                }
                if (socketException.SocketErrorCode == SocketError.TimedOut)
                {
                    throw new SubscriberException("网络超时",
                                                  socketException.InnerException ?? socketException);
                }
                throw new SubscriberException("未知错误",
                                                  socketException.InnerException ?? socketException);
            }
            catch (Exception e)
            {
                throw new SubscriberException("未知错误", e.InnerException ?? e);
            }
        }
        #endregion

        #region 订阅主题
        /// <summary>
        /// 订阅主题 异常信息[ConnectException] 尚未建立建立
        /// </summary>
        /// <param name="topicName"></param>
        public void Subscribe(string topicName)
        {
            ICommand command = new SubscriberCommand();
            command.TopicName = topicName;
            Send(SerializeHelper.ObjectToBytes(command));

            if (_isReceivingStarted == false)
            {
                _isReceivingStarted = true;
                Thread thread1 = new Thread(new ThreadStart(ReceiveDataFromServer));
                thread1.IsBackground = true;
                thread1.Start();
            }
        }
        #endregion

        #region 订阅主题后从服务端接收订阅数据
        /// <summary>
        /// 接受事件
        /// </summary>
        private void ReceiveDataFromServer()
        {
            NetPacketTcpAsynService asynService = new NetPacketTcpAsynService(_client.GetStream());
            asynService.OnReceivedPacket += ClientReceivedPacket;
            asynService.OnReceiveErrorHandler += delegate(NetServiceErrorReason reason)
                                                     {
                                                         LogManger.Error(reason,this.GetType());
                                                         ReceiveErrorHandler(reason);
                                                     };
            asynService.PickMessage();
            #region 同步
            //while (_isReceivingStarted)
            //{
            //    try
            //    {

            //        NetworkStream stream = _client.GetStream();
            //        NetPacket packet = new NetPacketTcpService(stream).ReceivePacket();
            //        if (packet != null && packet.Command != null && packet.Command.Data != null)
            //        {
            //            OnOnReceiveMessageEventHandler(packet.Command.Data);
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        LogManger.Error(e, this.GetType());
            //    }
            //    //Thread.Sleep(50);
            //}
            #endregion
        }
        private void ClientReceivedPacket(NetPacket packet)
        {
            if (packet != null && packet.Command != null && packet.Command.Data != null)
            {
                OnOnReceiveMessageEventHandler(packet.Command.Data);
            }
        }
        #endregion

        #region 取消订阅主题
        /// <summary>
        /// 取消订阅主题
        /// </summary>
        /// <param name="topicName"></param>
        public void UnSubscribe(string topicName)
        {
            if (this.IsClientConnected == false)
            {
                lock (_objectLock)
                {
                    _isReceivingStarted = false;
                }
                return;
            }
            ICommand command = new UnSubscribeCommand();
            command.TopicName = topicName;
            Send(SerializeHelper.ObjectToBytes(command));
            lock (_objectLock)
            {
                _isReceivingStarted = false;
            }
        }
        #endregion
    }
}
