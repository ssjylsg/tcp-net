using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MyMq
{
    /// <summary>
    /// 使用长连接
    /// </summary>
    public class TcpSubscriberService : IService
    {
        private Thread _thread;
        private volatile bool _shouldStop;
        public void StartService()
        {
            LogManger.Info("订阅服务开启", this.GetType());
            Filter.SubscribersList.Clear();
            _thread = new Thread(new ThreadStart(HostSubscriberService));
            _thread.IsBackground = true;
            _thread.Start();
        }

        public void Stop()
        {
            _server.Stop();
            _shouldStop = true;
        }

        public void ClearAllScriber()
        {
            TcpClientFilter.SubscribersList.Clear();
        }

        private TcpListener _server;
        private void HostSubscriberService()
        {
            IPAddress ipV4 = NetHelper.GetLocalMachineIP();
            IPEndPoint localEP = new IPEndPoint(ipV4, 10001);
            _server = new TcpListener(localEP);
            _server.Start();
            StartListening(_server);
        }

        private static void ClientReceiveMessage(object o)
        {
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            TcpClient client = (TcpClient)o;
            byte[] bytes;
            //while (true)
            //{
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
                                                                     LogManger.Error(e, typeof(TcpSubscribercs));
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
                                                                                     //Filter.Client = client;
                                                                                     TcpClientFilter.AddSubscriber(command.TopicName, remoteEP, client);
                                                                                     break;
                                                                                 case CommandFlags.UnSubscribe:
                                                                                     TcpClientFilter.RemoveSubscriber(command.TopicName, remoteEP);
                                                                                     break;
                                                                                 default:
                                                                                     LogManger.Warn("不支持的命令" + command.CommandName, typeof(TcpSubscriberService));
                                                                                     break;
                                                                             }
                                                                         }
                                                                     }));
                                                                 }
                                                                 if (client.Connected)
                                                                 {
                                                                     ClientReceiveMessage(client);
                                                                 }
                                                             }, client);
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
    }
}
