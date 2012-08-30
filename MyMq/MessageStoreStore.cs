using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
namespace MyMq
{

    /// <summary>
    /// 消息存储+消息发送
    /// </summary>
    internal class MessageStoreStore<T> : IService, IMessageStore<T> where T : class
    {
        #region 私有变量
        delegate void MethodEventHandler(object message);
        private Queue<T> _queueMessage;
        private Queue<ErrorSendMessage<TcpClient>> _errorQueue;
        private object _lockObject = new object();
        private Thread _thread;
        private volatile bool _shouldStop;
        #endregion

        public MessageStoreStore()
        {
            _queueMessage = new Queue<T>();
            _errorQueue = new Queue<ErrorSendMessage<TcpClient>>();
        }

        #region 线程发送数据
        private void ExecuteThread()
        {
            T message;
            while (_shouldStop == false)
            {
                message = this.GetNextMessage();
                if (message != null)
                {
                    MethodEventHandler methodEventHandler = new MethodEventHandler(this.SendMessage);
                    methodEventHandler.BeginInvoke(message, null, null);
                }
                Thread.Sleep(10);
            }
        }

        private void SendMessage(object message)
        {
            NetPacket packet = message as NetPacket;
            if (packet == null)
            {
                return;
            }
            ICommand command = packet.Command;
            foreach (TcpClient endPoint in TcpClientFilter.GetSubscribers(command.TopicName))
            {
                try
                {
                    if (endPoint.Connected == false)
                    {
                        continue;
                    }
                    NetworkStream stream = endPoint.GetStream();
                    {
                        if (stream.CanWrite)
                        {

                            new NetPacketTcpAsynService(stream).SendMessage(packet);
                        }
                        else
                        {
                            ErrorSendMessage<TcpClient> errorSendMessage = new ErrorSendMessage<TcpClient>();
                            errorSendMessage.Packet = packet;
                            errorSendMessage.TargetPoin = endPoint;

                            _errorQueue.Enqueue(errorSendMessage);
                            LogManger.Warn("不能访问", this.GetType());

                        }
                    }
                }
                catch (Exception e)
                {
                    ErrorSendMessage<TcpClient> errorSendMessage = new ErrorSendMessage<TcpClient>();
                    errorSendMessage.Packet = packet;
                    errorSendMessage.TargetPoin = endPoint;
                    _errorQueue.Enqueue(errorSendMessage);
                    LogManger.Error(e, this.GetType());
                }
            }
        }
        /// <summary>
        /// 获取消息
        /// </summary>
        /// <returns></returns>
        private T GetNextMessage()
        {
            lock (_lockObject)
            {
                if (_queueMessage.Count != 0)
                {
                    return _queueMessage.Dequeue();
                }
                return null;
            }
        }
        private void RemoveLastMessage()
        {
            lock (_lockObject)
            {
                if (_queueMessage.Count != 0)
                {
                    _queueMessage.Dequeue();
                }
            }
        }
        #endregion

        #region 存储消息
        /// <summary>
        /// 存储消息
        /// </summary>
        /// <param name="message"></param>
        public void StoreMessage(T message)
        {
            if (message == null)
            {
                return;
            }
            lock (_lockObject)
            {
                _queueMessage.Enqueue(message);
            }
        }
        #endregion

        #region 开始服务
        /// <summary>
        /// 开始服务
        /// </summary>
        public void StartService()
        {
            _thread = new Thread(ExecuteThread);
            _thread.IsBackground = true;
            _thread.Start();
        }
        #endregion
        
        #region 停止服务
        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            _shouldStop = true;
            lock (_lockObject)
            {
                this._queueMessage.Clear();
            }
        }
        #endregion
    }

    class ErrorSendMessage<T>
    {
        private NetPacket _packet;
        private T _targetPoin;
        /// <summary>
        /// 消息目的地
        /// </summary>
        public T TargetPoin
        {
            get { return _targetPoin; }
            set { _targetPoin = value; }
        }
        /// <summary>
        /// 消息
        /// </summary>
        public NetPacket Packet
        {
            get { return _packet; }
            set { _packet = value; }
        }
    }
}
