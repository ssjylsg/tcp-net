using System.Net;
using System.Net.Sockets;

namespace MyMq
{
    class NetHelper
    {
        /// <summary>
        /// 获取本地IP
        /// </summary>
        /// <returns></returns>
        public static IPAddress GetLocalMachineIP()
        {
            return IPAddress.Parse("127.0.0.1");
            IPAddress[] addr = Dns.GetHostEntry(Dns.GetHostName()).AddressList; 
            IPAddress ipV4 = null;
            foreach (IPAddress item in addr)
            {
                if (item.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipV4 = item;
                    break;
                }
            }
            return ipV4;
        }
    }
}
