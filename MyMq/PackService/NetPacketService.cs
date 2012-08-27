using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MyMq
{
    /// <summary>
    /// 网络封包服务类[提供封包的发送和读取]
    /// </summary>
    internal abstract class NetPacketService : IReceiveMessage<NetPacket>, ISendMessage<NetPacket>
    {
        /// <summary>
        /// Tcp传输调用该构造方法
        /// </summary> 
        protected NetPacketService(NetworkStream netStream)
        {
            this._netStream = netStream;
            this._protocolType = System.Net.Sockets.ProtocolType.Tcp;
        }

        /// <summary>
        /// Tcp传输调用该构造方法
        /// </summary> 
        protected NetPacketService(Socket tcpSocket)
        {
            this._tcpSocket = tcpSocket;
            this._netStream = new NetworkStream(this._tcpSocket);
            this._protocolType = System.Net.Sockets.ProtocolType.Tcp;
        }
        /// <summary>
        /// 提取一个完整网络包然后返回[Tcp收包和Udp收包的行为不同,故转到子类实现]
        /// </summary>
        /// <returns></returns>
        public abstract NetPacket PickMessage();
        /// <summary>
        /// 序列化Helper
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ISerializeHelper SerializeHelper<T>() where T : ISerializeHelper
        {
            return Activator.CreateInstance<T>();
        }
        /// <summary>
        /// 判断NetStream 是否可读
        /// </summary>
        public bool IsNetStreamCanRead
        {
            get
            {
                if (this._netStream == null)
                {
                    return true;
                }
                try
                {
                    return _netStream.CanRead;
                }
                catch (Exception e) 
                {
                    LogManger.Error(e,this.GetType());
                    return false;
                }
               
            }
        }
        /// <summary>
        /// 判断NetStream 是否可读
        /// </summary>
        public bool IsNetStreamCanWrite
        {
            get
            {
                if (this._netStream == null)
                {
                    return true;
                }
                try
                {
                    return _netStream.CanWrite;
                }
                catch (Exception e)
                {
                    LogManger.Error(e, this.GetType());
                    return false;
                }
            }
        }
        /// <summary>
        /// 缓冲区大小1024字节
        /// </summary>
        public const Int32 BUFFER_SIZE = 1024;

        /// <summary>
        /// Udp数据报最大值
        /// </summary>
        protected const Int32 UDP_BUFFER_SIZE = 512;

        private ProtocolType _protocolType = ProtocolType.Tcp;
        /// <summary>
        /// 协议类型[默认为Tcp]
        /// </summary>
        public ProtocolType ProtocolType
        {
            set
            {
                _protocolType = value;
            }
        }

        /// <summary>
        /// 接收网络数据的缓冲区
        /// </summary>
        protected Byte[] _netDataBuffer = new Byte[BUFFER_SIZE * 64];

        /// <summary>
        /// 缓冲区实际数据长度
        /// </summary>
        protected int _netDataOffset = 0;

        /// <summary>
        /// 网络流
        /// </summary>
        protected  NetworkStream _netStream = null;

        /// <summary>
        /// 缓冲区
        /// </summary>
        protected Byte[] _tempBuffer = new Byte[BUFFER_SIZE];
        /// <summary>
        /// Tcp套接字
        /// </summary>
        protected Socket _tcpSocket;

        /// <summary>
        /// Udp发送的目标端点
        /// </summary>
        protected EndPoint _udpRemoteEndPoint;

        public EndPoint UdpRemoteEndPoint
        {
            get { return _udpRemoteEndPoint; }
            set { _udpRemoteEndPoint = value; }
        }

        /// <summary>
        /// 发包[Tcp发包和Udp发包行为略有不同,故转到子类实现]
        /// </summary>
        /// <param name="packet"></param>
        public abstract void SendMessage(NetPacket packet);

        /// <summary>
        /// 缓冲区是否满足一个完整包头
        /// </summary>
        /// <returns></returns>
        protected Boolean IsFullNetPacketHead()
        {
            return _netDataOffset >= NetPacketHead.HEAD_SIZE;
        }

        /// <summary>
        /// 一个完整的网络封包的大小
        /// </summary>
        protected Int32 FullNetPacketSize
        {
            get
            {
                if (!IsFullNetPacketHead())
                    throw new Exception("由于没有缓冲区数据不足一个包头大小因此不能获得完整网络封包大小");
                return NetPacketHead.HEAD_SIZE + BitConverter.ToInt32(_netDataBuffer, 8);
            }

        }

        /// <summary>
        /// 缓冲区是否满足一个完整封包
        /// </summary>
        /// <returns></returns>
        protected Boolean IsFullNetPacket()
        {
            return _netDataOffset >= NetPacketHead.HEAD_SIZE + BitConverter.ToInt32(_netDataBuffer, 8);
        }

        /// <summary>
        /// 获得可序列化得对象的大小
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        protected Int32 GetCanSerializableObjectSize(object graph)
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                SerializeHelper<BinarySerializeHelper>().Serialize(mStream, graph);
                return (Int32)mStream.Length;
            }
        }

        /// <summary>
        /// 从缓冲区提取一个包头,但并不从缓冲区删除该包头
        /// </summary>
        /// <returns></returns>
        protected NetPacketHead PeekNetPacket()
        {
            NetPacketHead packetHead = new NetPacketHead();
            packetHead.Version = BitConverter.ToInt32(_netDataBuffer, 0);//版本
            packetHead.PType = (PacketType)BitConverter.ToInt32(_netDataBuffer, 4);//封包类型
            packetHead.Len = BitConverter.ToInt32(_netDataBuffer, 8);
            return packetHead;
        }

        /// <summary>
        /// 从缓冲区提取一个网络包
        /// </summary>
        /// <returns></returns>
        protected NetPacket PickNetPacket()
        {

            #region【提取包头】
            NetPacketHead packetHead = PeekNetPacket();
            #endregion

            if (packetHead.Len <= 0)
            {
                return null;
            }


            #region【提取包体】
            NetPacket packet = new NetPacket();
            packet.PacketHead = packetHead;
            Byte[] buffer = new Byte[packetHead.Len];
            Array.Copy(_netDataBuffer, NetPacketHead.HEAD_SIZE, buffer, 0, packetHead.Len);

            using (MemoryStream mStream = new MemoryStream())
            {
                mStream.Write(buffer, 0, buffer.Length);
                mStream.Position = 0;
                packet.Command = SerializeHelper<BinarySerializeHelper>().DeSerialize<ICommand>(mStream);
            }
            #endregion

            #region【从缓冲区删除该数据封包】
            Array.Copy(_netDataBuffer, NetPacketHead.HEAD_SIZE + packetHead.Len, _netDataBuffer, 0, _netDataOffset - (NetPacketHead.HEAD_SIZE + packetHead.Len));
            _netDataOffset -= (NetPacketHead.HEAD_SIZE + packetHead.Len);//缓冲区实际数据长度减去一个完整封包长度
            #endregion

            return packet;
        } 
        /// <summary>
        /// 发送网络包
        /// </summary>
        /// <param name="data"></param>
        /// <param name="networkStream"></param>
        public void SendMessage(NetPacket data, NetworkStream networkStream)
        {
            this._netStream = networkStream;
            this.SendMessage(data);
        } 
    }
}
