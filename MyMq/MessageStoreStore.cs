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
        delegate void MethodEventHandler(object message);
        private Queue<T> _queueMessage;
        private Queue<ErrorSendMessage<TcpClient>> _errorQueue;
        private object _lockObject = new object();
        private Thread _thread;

        public MessageStoreStore()
        {
            _queueMessage = new Queue<T>();
            _errorQueue = new Queue<ErrorSendMessage<TcpClient>>();
        }

        private void ExecuteThread()
        {
            T message;
            while (_shouldStop == false)
            {
                message = this.GetNextMessage();
                if (message != null)
                {
                    //ThreadPool.QueueUserWorkItem(SendMessage, message);
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

        public void StartService()
        {
            _thread = new Thread(ExecuteThread);
            _thread.IsBackground = true;
            _thread.Start();
        }
        private volatile bool _shouldStop;
        public void Stop()
        {
            _shouldStop = true;
            lock (_lockObject)
            {
                this._queueMessage.Clear();
            }
        }
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
