using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace PubSubServer
{
    class SubscriberService
    {

        public void StartSubscriberService()
        {
            Thread th = new Thread(new ThreadStart(HostSubscriberService));
            th.IsBackground = true;
            th.Start();
        }

        private void HostSubscriberService()
        {
            IPAddress ipV4 = IPAddress.Parse("127.0.0.1");// ReturnMachineIP(); if you need machine ip then use this method.The method is available in PublishService.cs            

            IPEndPoint localEP = new IPEndPoint(ipV4, 10001);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            server.Bind(localEP);


            StartListening(server);

        }

        private static void StartListening(Socket server)
        {
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            int recv = 0;
            byte[] data = new byte[1024];
            while (true)
            {

                recv = 0;
                data = new byte[1024];
                recv = server.ReceiveFrom(data, ref remoteEP);
                string messageSendFromClient = Encoding.ASCII.GetString(data, 0, recv);
                string[] messageParts = messageSendFromClient.Split(",".ToCharArray());

                if (!string.IsNullOrEmpty(messageParts[0]))
                {
                    switch (messageParts[0])
                    {
                        case "Subscribe":

                            if (!string.IsNullOrEmpty(messageParts[1]))
                            {
                                Filter.AddSubscriber(messageParts[1], remoteEP);
                            }


                            break;
                        case "UnSubscribe":

                            if (!string.IsNullOrEmpty(messageParts[1]))
                            {
                                Filter.RemoveSubscriber(messageParts[1], remoteEP);
                            }
                            break;
                    }
                }

            }
        }

    }
}
