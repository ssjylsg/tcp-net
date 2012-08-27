using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MyMq
{

    /// <summary>
    /// 使用短连接
    /// </summary>
    public class ProducerTcpService : IService
    {

        class ThreadParmeter<T> where T : class
        {
            private IMessageStore<T> _messageStoreStore1;
            private TcpClient _client;

            public IMessageStore<T> MessageStoreStore
            {
                get { return _messageStoreStore1; }
                set { _messageStoreStore1 = value; }
            }

            public TcpClient Client
            {
                get { return _client; }
                set { _client = value; }
            }
        }

        private MessageStoreStore<NetPacket> _messageStoreStore;
        /// <summary>
        /// 测试使用
        /// </summary>
        public static event ReceiveMessageEventHandler ReceiveMessageEventHandler;

        private static void InvokeReceiveMessageEventHandler(object e)
        {
            ReceiveMessageEventHandler handler = ReceiveMessageEventHandler;
            if (handler != null) handler(e);
        }

        /// <summary>
        /// 启动发布服务
        /// </summary>
        public void StartService()
        {

            Thread th = new Thread(new ThreadStart(HostPublisherService));
            _messageStoreStore = new MessageStoreStore<NetPacket>();
            _messageStoreStore.StartService();
            th.IsBackground = true;
            th.Start();
            LogManger.Info("发布服务开启", this.GetType());
        }

        public void Stop()
        {
            _messageStoreStore.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        private void HostPublisherService()
        {
            IPAddress ipV4 = NetHelper.GetLocalMachineIP();
            IPEndPoint localEP = new IPEndPoint(ipV4, SystemConfig.PublishServerPort);
            TcpListener tcpListener = new TcpListener(localEP);
            tcpListener.Start();
            StartListening(tcpListener);
        }
        /// <summary>
        /// 接受数据
        /// </summary>
        /// <param name="o"></param>
        private static void ReceiveClient(object o)
        {
            #region 异步方法
            ThreadParmeter<NetPacket> parmeter = (ThreadParmeter<NetPacket>)o;
            TcpClient client = parmeter.Client;
            NetworkStream netstream = client.GetStream();
            NetPacketTcpAsynService service = new NetPacketTcpAsynService(netstream);
            service.OnReceivedPacket += delegate(NetPacket netPacket)
            {
                if (netPacket != null)
                {
                    InvokeReceiveMessageEventHandler(netPacket.Command.Data);
                    parmeter.MessageStoreStore.StoreMessage(netPacket);
                }
                netstream.Close();
                client.Close();
            };
            service.PickMessage();
            #endregion

            #region 同步方法
            //using (TcpClient client = parmeter.Client)
            //using (NetworkStream netstream = client.GetStream())
            //{
            //    NetPacket netPacket = new NetPacketTcpService(netstream).PickMessage();
            //    if (netPacket != null)
            //    {
            //        InvokeReceiveMessageEventHandler(netPacket.Command.Data);
            //        parmeter.MessageStoreStore.StoreMessage(netPacket);
            //    }
            //}
            #endregion
        }

        /// <summary>
        /// 开始监听
        /// </summary>
        /// <param name="server"></param>
        private void StartListening(TcpListener server)
        {
            server.BeginAcceptTcpClient(delegate(IAsyncResult result)
            {
                TcpClient client = server.EndAcceptTcpClient(result);
                ThreadParmeter<NetPacket> parmeter = new ThreadParmeter<NetPacket>();
                parmeter.Client = client;
                parmeter.MessageStoreStore = _messageStoreStore;
                ThreadPool.QueueUserWorkItem(ReceiveClient, parmeter);
                StartListening(server);
            }, server);

            #region 同步
            //EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            //int recv = 0;
            //byte[] data = new byte[1024];
            //while (true)
            //{
            //    try
            //    {

            //        TcpClient client = server.AcceptTcpClient();
            //        ThreadParmeter<NetPacket> parmeter = new ThreadParmeter<NetPacket>();
            //        parmeter.Client = client;
            //        parmeter.MessageStoreStore = _messageStoreStore;
            //        ThreadPool.QueueUserWorkItem(ReceiveClient, parmeter);  
            //    }
            //    catch (Exception e)
            //    {
            //        LogManger.Error(e, this.GetType());
            //        Console.WriteLine(e);
            //    }
            //}
            #endregion
        }
    }
}
