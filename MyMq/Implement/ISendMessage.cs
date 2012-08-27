using System.Net.Sockets;

namespace MyMq
{
    /// <summary>
    /// 发送信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    interface ISendMessage<T>
    {
        void SendMessage(T data);
        void SendMessage(T data,NetworkStream networkStream);
    }
}
