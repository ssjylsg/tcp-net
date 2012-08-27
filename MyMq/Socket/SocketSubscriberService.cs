using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MyMq
{
    /// <summary>
    /// 订阅服务
    /// </summary>
    public class SocketSubscriberService : IService
    {
        private Thread _thread;
        private bool _running;
        public void StartService()
        {
            Filter.SubscribersList.Clear();
            _thread = new Thread(new ThreadStart(HostSubscriberService));
            _thread.IsBackground = true;
            this._running = true;
            _thread.Start();
        }
        public void Stop()
        {
            this._running = false;
            this._thread.Join();
        }
        private void HostSubscriberService()
        {
            IPAddress ipV4 = NetHelper.GetLocalMachineIP();
            IPEndPoint localEP = new IPEndPoint(ipV4, 10001);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            server.Bind(localEP);
            StartListening(server);
        }

        private void StartListening(Socket server)
        {
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            int recv = 0;
            byte[] data = new byte[1024];
            while (_running)
            {
                recv = 0;
                data = new byte[1024];
                recv = server.ReceiveFrom(data, ref remoteEP);
                ICommand command = SerializeHelper.BytesToObject(data) as ICommand;
                if (command != null)
                {
                    switch (command.CommandName)
                    {
                        case CommandFlags.Subscribe:
                            Filter.AddSubscriber(command.TopicName, remoteEP);
                            break;
                        case CommandFlags.UnSubscribe:
                            Filter.RemoveSubscriber(command.TopicName, remoteEP);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

    }
}
