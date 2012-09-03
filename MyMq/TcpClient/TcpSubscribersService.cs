using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MyMq
{
    /// <summary>
    ///订阅服务 
    /// </summary>
    internal class TcpSubscribersService : IService
    {
        private Thread _thread;
        private volatile bool _shouldStop;
        private TcpListener _server;

        #region 启动服务
        public void StartService()
        {
            _thread = new Thread(new ThreadStart(HostSubscriberService));
            _thread.IsBackground = true;
            this._shouldStop = false;
            _thread.Start();
            LogManger.Info("订阅服务开启", this.GetType());
        }
        #endregion

        #region 停止服务
        /// <summary>
        /// TcpListener 停止 清空已经订阅的客户
        /// </summary>
        public void Stop()
        {
            _server.Stop();
            _shouldStop = true;
            this.ClearAllScriber();
            LogManger.Info("订阅服务停止", this.GetType());
        }
        #endregion

        #region 清空所有订阅者
        public void ClearAllScriber()
        {
            TcpClientFilter.SubscribersList.Clear();
        }
        #endregion

        #region TcpListener 监听订阅者，当收到数据时，查看命令确认是订阅/取消订阅事件
        private void HostSubscriberService()
        {
            IPAddress ipV4 = NetHelper.GetLocalMachineIP();
            IPEndPoint localEP = new IPEndPoint(ipV4, SystemConfig.SubscriberServerPort);
            _server = new TcpListener(localEP);
            _server.Start();
            StartListening(_server);
        }

        private static void ClientReceiveMessage(object o)
        {
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            TcpClient client = (TcpClient)o;
            byte[] bytes;

            if (client.Connected == false)
            {
                return;
            }
            NetworkStream stream = client.GetStream();
            if (stream.CanRead)
            {

                bytes = new byte[client.ReceiveBufferSize];
                stream.BeginRead(bytes, 0, bytes.Length, delegate(IAsyncResult result)
                                                             {
                                                                 int readCount = 0;
                                                                 try
                                                                 {
                                                                     readCount = stream.EndRead(result);
                                                                 }
                                                                 catch (Exception e)
                                                                 {
                                                                     LogManger.Error(e, typeof(TcpSubscribers));
                                                                 }
                                                                 if (readCount > 0)
                                                                 {
                                                                     byte[] buffer = new byte[readCount];
                                                                     Array.Copy(bytes, buffer, readCount);
                                                                     bytes = null;
                                                                     ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object obj)
                                                                     {
                                                                         ICommand command = SerializeHelper.BytesToObject(buffer) as ICommand;
                                                                         remoteEP = client.Client.RemoteEndPoint;
                                                                         if (command != null)
                                                                         {
                                                                             switch (command.CommandName)
                                                                             {
                                                                                 case CommandFlags.Subscribe:
                                                                                     TcpClientFilter.AddSubscriber(command.TopicName, remoteEP, client);
                                                                                     break;
                                                                                 case CommandFlags.UnSubscribe:
                                                                                     TcpClientFilter.RemoveSubscriber(command.TopicName, remoteEP);
                                                                                     break;
                                                                                 default:
                                                                                     LogManger.Warn("不支持的命令" + command.CommandName, typeof(TcpSubscribersService));
                                                                                     break;
                                                                             }
                                                                         }
                                                                     }));
                                                                 }

                                                                 if (client.Connected) // 处理完一个请求，然后在等待客户端发送的数据
                                                                 {
                                                                     ClientReceiveMessage(client);
                                                                 }
                                                             }, client);
                #region 同步
                //        if (readCount == 0)
                //        {
                //            Thread.Sleep(50);
                //            continue;
                //        }
                //        byte[] buffer = new byte[readCount];
                //        Array.Copy(bytes, buffer, readCount);
                //        bytes = null;
                //        ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object obj)
                //                                                          {
                //                                                              ICommand command = SerializeHelper.BytesToObject(buffer) as ICommand;
                //                                                              remoteEP = client.Client.RemoteEndPoint;
                //                                                              if (command != null)
                //                                                              {
                //                                                                  switch (command.CommandName)
                //                                                                  {
                //                                                                      case CommandFlags.Subscribe:
                //                                                                          Filter.Client = client;
                //                                                                          TcpClientFilter.AddSubscriber(command.TopicName, remoteEP, client);
                //                                                                          break;
                //                                                                      case CommandFlags.UnSubscribe:
                //                                                                          TcpClientFilter.RemoveSubscriber(command.TopicName, remoteEP);
                //                                                                          break;
                //                                                                      default:
                //                                                                          break;
                //                                                                  }
                //                                                              }
                //                                                          }));
                //    }
                //    Thread.Sleep(50);
                #endregion
            }
        }

        private void StartListening(TcpListener server)
        {
            if (_shouldStop)
            {
                return;
            }
            server.BeginAcceptTcpClient(new AsyncCallback(delegate(IAsyncResult result)
                                                              {
                                                                  TcpClient client = null;
                                                                  try
                                                                  {
                                                                      client = server.EndAcceptTcpClient(result);
                                                                  }
                                                                  catch (Exception e)
                                                                  {
                                                                      LogManger.Error(e, this.GetType());
                                                                  }
                                                                  if (client != null)
                                                                  {
                                                                      ThreadPool.QueueUserWorkItem(ClientReceiveMessage, client);
                                                                  }

                                                                  StartListening(server);
                                                              }), server);
        }
        #endregion
    }
}
