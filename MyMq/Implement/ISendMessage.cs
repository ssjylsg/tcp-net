using System.Net.Sockets;

namespace MyMq
{
    interface ISendMessage<T>
    {
        void SendMessage(T data);
        void SendMessage(T data,NetworkStream networkStream);
    }
}
