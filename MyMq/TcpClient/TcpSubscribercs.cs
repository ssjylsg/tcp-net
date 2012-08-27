using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MyMq
{
    public class TcpSubscribercs : ISubscribercs
    {
        private TcpClient _client;
        private IPEndPoint _remoteEndPoint;
        private byte[] _data;
        private int _recv;
        private TimeSpan _sleepTimeSpan;
        private Boolean _isReceivingStarted = false;
        private object _objectLock = new object();

        /// <summary>
        /// 订阅者接受到信息事件
        /// </summary>
        public event ReceiveMessageEventHandler OnReceiveMessageEventHandler;
        /// <summary>
        /// 事件处理
        /// </summary>
        /// <param name="message"></param>
        private void OnOnReceiveMessageEventHandler(object message)
        {
            ReceiveMessageEventHandler handler = OnReceiveMessageEventHandler;
            if (handler != null) handler(message);
        }
        private void Send(byte[] buffer)
        {
            if (_client.Connected == false)
            {
                _client = new TcpClient();
                _client.Connect(_remoteEndPoint);
            }
            NetworkStream stream = _client.GetStream();
            if (stream.CanWrite)
            {
                stream.Write(buffer, 0, buffer.Length);
            }
        }
        /// <summary>
        /// 订阅构造函数
        /// </summary>
        /// <param name="serverIp">Server IP</param>
        /// <param name="serverPort">Server Port</param>
        /// <param name="sleepTimeSpan"></param>
        public TcpSubscribercs(string serverIp, int serverPort, TimeSpan sleepTimeSpan)
        {
            this._sleepTimeSpan = sleepTimeSpan;
            IPAddress serverIPAddress = IPAddress.Parse(serverIp);
            _remoteEndPoint = new IPEndPoint(serverIPAddress, serverPort);
            _client = new TcpClient();
            _client.Connect(_remoteEndPoint);
        }
        /// <summary>
        /// 订阅主题
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
                _data = new byte[1024];
                Thread thread1 = new Thread(new ThreadStart(ReceiveDataFromServer));
                thread1.IsBackground = true;
                thread1.Start();
            }
        }
        /// <summary>
        /// 接受事件
        /// </summary>
        private void ReceiveDataFromServer()
        {
            NetPacketTcpAsynService asynService = new NetPacketTcpAsynService(_client.GetStream());
            asynService.OnReceivedPacket += delegate(NetPacket packet)
                                                                                         {
                                                                                             if (packet != null && packet.Command != null && packet.Command.Data != null)
                                                                                             {
                                                                                                 OnOnReceiveMessageEventHandler(packet.Command.Data);
                                                                                             }
                                                                                         };
            asynService.PickMessage();



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
        }
        /// <summary>
        /// 取消订阅主题
        /// </summary>
        /// <param name="topicName"></param>
        public void UnSubscribe(string topicName)
        {

            ICommand command = new UnSubscribeCommand();
            command.TopicName = topicName;
            Send(SerializeHelper.ObjectToBytes(command));
            lock (_objectLock)
            {
                _isReceivingStarted = false;
            }
        }
    }
}
