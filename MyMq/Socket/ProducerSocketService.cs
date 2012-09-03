using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MyMq
{
    /// <summary>
    /// 消息生产者中心服务
    /// </summary>
    internal class ProducerSocketService : IService
    {
        private volatile bool _shouldStop;
        /// <summary>
        /// 启动发布服务
        /// </summary>
        public void StartService()
        {
            Thread th = new Thread(new ThreadStart(HostPublisherService));
            th.IsBackground = true;
            th.Start();
        }

        public void Stop()
        {
            _shouldStop = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void HostPublisherService()
        {
            IPAddress ipV4 = NetHelper.GetLocalMachineIP();
            IPEndPoint localEP = new IPEndPoint(ipV4, 10002);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            server.Bind(localEP);
            StartListening(server);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="server"></param>
        private void StartListening(Socket server)
        {
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            int recv = 0;
            byte[] data = new byte[1024];
            while (_shouldStop == false)
            {
                try
                {
                    recv = 0;
                    data = new byte[1024];
                    recv = server.ReceiveFrom(data, ref remoteEP);
                    ICommand command = SerializeHelper.BytesToObject(data) as ICommand;
                    if (command != null)
                    {
                        if (command.CommandName == CommandFlags.Publish)
                        {
                            if (!string.IsNullOrEmpty(command.TopicName))
                            {
                                List<EndPoint> subscriberListForThisTopic = Filter.GetSubscribers(command.TopicName);
                                WorkerThreadParameters workerThreadParameters = new WorkerThreadParameters();
                                workerThreadParameters.Server = server;
                                workerThreadParameters.Message = command.Data;
                                workerThreadParameters.SubscriberListForThisTopic = subscriberListForThisTopic;

                                ThreadPool.QueueUserWorkItem(new WaitCallback(Publish), workerThreadParameters);
                            }
                        }
                    }
                }
                catch
                { }

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stateInfo"></param>
        private static void Publish(object stateInfo)
        {
            WorkerThreadParameters workerThreadParameters = (WorkerThreadParameters)stateInfo;
            Socket server = workerThreadParameters.Server;
            object message = workerThreadParameters.Message;
            List<EndPoint> subscriberListForThisTopic = workerThreadParameters.SubscriberListForThisTopic;
            if (subscriberListForThisTopic != null)
            {
                foreach (EndPoint endPoint in subscriberListForThisTopic)
                {
                    server.SendTo(SerializeHelper.ObjectToBytes(message), SocketFlags.None, endPoint);
                }
            }
        }
    }
}
