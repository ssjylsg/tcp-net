using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MyMq
{
    public delegate void ReceiveMessageEventHandler(object message);
    /// <summary>
    /// 订阅者
    /// </summary>
    internal class SocketSubscriber : ISubscribers
    {
        private Socket _client;
        private EndPoint _remoteEndPoint;
        private byte[] _data;
        private int _recv;
        private TimeSpan _sleepTimeSpan;
        private Boolean _isReceivingStarted = false;
        /// <summary>
        /// 订阅者接受到信息事件
        /// </summary>
        public event ReceiveMessageEventHandler ReceiveMessage;

        public event ReceiveErrorHandler ReceiveMessageError;
        public event EventHandler ServerClosed;

        /// <summary>
        /// 事件处理
        /// </summary>
        /// <param name="message"></param>
        private void OnOnReceiveMessageEventHandler(byte[] message)
        {
            ReceiveMessageEventHandler handler = ReceiveMessage;
            if (handler != null) handler(SerializeHelper.BytesToObject(message));
        }
        /// <summary>
        /// 订阅构造函数
        /// </summary>
        /// <param name="serverIp">Server IP</param>
        /// <param name="serverPort">Server Port</param>
        /// <param name="sleepTimeSpan"></param>
        public SocketSubscriber(string serverIp, int serverPort, TimeSpan sleepTimeSpan)
        {
            this._sleepTimeSpan = sleepTimeSpan;
            IPAddress serverIPAddress = IPAddress.Parse(serverIp);
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _remoteEndPoint = new IPEndPoint(serverIPAddress, serverPort);
        }
        /// <summary>
        /// 订阅主题
        /// </summary>
        /// <param name="topicName"></param>
        public void Subscribe(string topicName)
        {
            ICommand command = new SubscriberCommand();
            command.TopicName = topicName;
            _client.SendTo(SerializeHelper.ObjectToBytes(command), _remoteEndPoint);

            if (_isReceivingStarted == false)
            {
                _isReceivingStarted = true;
                _data = new byte[1024];
                Thread thread1 = new Thread(new ThreadStart(ReceiveDataFromServer));
                thread1.IsBackground = true;
                thread1.Start();
            }
        }

        public bool IsClientConnected
        {
            get { throw new NotImplementedException(); }
        }

        public void ReConnectServer()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 接受事件
        /// </summary>
        private void ReceiveDataFromServer()
        {
            EndPoint publisherEndPoint = _client.LocalEndPoint;
            while (_isReceivingStarted)
            {
                _recv = _client.ReceiveFrom(_data, ref publisherEndPoint);
                if (_recv > 0)
                {
                    byte[] receiveData = new byte[_recv];
                    Array.Resize(ref receiveData,_recv);
                    OnOnReceiveMessageEventHandler(receiveData);
                }
            }
        }
        /// <summary>
        /// 取消订阅主题
        /// </summary>
        /// <param name="topicName"></param>
        public void UnSubscribe(string topicName)
        {
            ICommand command = new UnSubscribeCommand();
            command.TopicName = topicName;
            _client.SendTo(SerializeHelper.ObjectToBytes(command), _remoteEndPoint);
            _isReceivingStarted = false;
        }
    }
}
