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
        private Queue<T> _queue;
        private object _lockObject = new object();
        private Thread _thread;
        private ISendMessage<NetPacket> _sendMessageService;
        public MessageStoreStore()
        {

            _queue = new Queue<T>();
        }

        private void ExecuteThread()
        {
            T message;
            while (true)
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
                            //ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object o)
                            //                                                  {
                            //                                                      StreamHelper.SendData(packet, stream);
                            //                                                  }));
                            new NetPacketTcpAsynService(stream).SendMessage(packet);
                        }
                        else
                        {
                            LogManger.Warn("不能访问", this.GetType());
                            System.Diagnostics.Trace.WriteLine("不能访问");
                        }
                    }
                }
                catch (Exception e)
                {
                    LogManger.Error(e, this.GetType());
                    System.Diagnostics.Trace.WriteLine(e + "出现错误");
                }
            }
        }
        private T GetNextMessage()
        {
            lock (_lockObject)
            {
                if (_queue.Count != 0)
                {
                    return _queue.Dequeue();
                }
                return null;
            }
        }
        private void RemoveLastMessage()
        {
            lock (_lockObject)
            {
                if (_queue.Count != 0)
                {
                    _queue.Dequeue();
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
                _queue.Enqueue(message);
            }
        }

        public void StartService()
        {
            _thread = new Thread(ExecuteThread);
            _thread.IsBackground = true;
            _thread.Start();
        }

        public void Stop()
        {
            _thread.Join();
            lock (_thread)
            {
                this._queue.Clear();
            }
        }
    }

    class  ErrorSendMessage<T>
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
