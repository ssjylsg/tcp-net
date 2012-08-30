using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace MyMq
{
    /// <summary>
    /// 发包前事件
    /// </summary>
    /// <param name="packet"></param>
    internal delegate void BeforeSendPacketHandler(NetPacket packet);
    /// <summary>
    /// 发包后事件
    /// </summary>
    /// <param name="packetHead"></param>
    internal delegate void AfterSendPacketHandler(NetPacketHead packetHead);
    /// <summary>
    /// 网络异步包接收失败 
    /// </summary>
    /// <param name="reason"></param>
    public delegate void ReceiveErrorHandler(NetServiceErrorReason reason);
    /// <summary>
    /// 网络异步发送失败
    /// </summary>
    /// <param name="reason"></param>
    public delegate void SendErrorHandler(NetServiceErrorReason reason);
    /// <summary>
    /// 网络包接收/发送失败原因
    /// </summary>
    public class NetServiceErrorReason
    {
        private Exception _exception;
        private string _message;
        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception Exception
        {
            get { return _exception; }
            set { _exception = value; }
        }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message
        {
            get
            {
                if (string.IsNullOrEmpty(_message))
                {
                    return this._exception != null ? this._exception.Message : string.Empty;
                }
                return _message;
            }
            set { _message = value; }
        }
        /// <summary>
        /// 发送失败时的数据
        /// </summary>
        public object SendData
        {
            get { return _sendData; }
            set { _sendData = value; }
        }

        private object _sendData;
    }

    /// <summary>
    /// 网络封包Tcp异步服务类
    /// </summary>
    internal class NetPacketTcpAsynService : NetPacketService
    {
        #region event
        /// <summary>
        /// 发包前事件
        /// </summary>
        internal event BeforeSendPacketHandler OnBeforeSendPacket;
        /// <summary>
        /// 发包后事件
        /// </summary>
        internal event AfterSendPacketHandler OnAfterSendPacket;
        /// <summary>
        /// 接报失败
        /// </summary>
        internal event ReceiveErrorHandler OnReceiveErrorHandler;
        /// <summary>
        /// 发包失败
        /// </summary>
        internal event SendErrorHandler OnSendErrorHandler;
        /// <summary>
        /// 收到网络封包后的事件
        /// </summary>
        public event BeforeSendPacketHandler OnReceivedPacket;
        #endregion

        public NetPacketTcpAsynService(NetworkStream netStream)
            : base(netStream)
        {

        }

        #region 接收包
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reason"></param>
        private void ReceiveErrorHandler(NetServiceErrorReason reason)
        {
            ReceiveErrorHandler handler = OnReceiveErrorHandler;
            if (handler != null) handler(reason);
        }

        /// <summary>
        /// 该方法的返回值总是为空.提取一个完整网络包然后返回,返回的封包需要通过注册OnReceivedPacket事件的函数获取 
        /// </summary>
        /// <returns></returns>
        public override NetPacket PickMessage()
        {
            try
            {
                //判断是否满足一个完整封包大小
                if (IsFullNetPacket())//如果有完整封包就返回
                {
                    NetPacket packet = PickNetPacket();
                    if (OnReceivedPacket != null) //判断事件是否注册,如果注册调用回调函数传递收到的封包
                    {
                        OnReceivedPacket(packet);
                    }
                    //return null;//提取到一个封包后应该及时返回
                }
                if (_netStream.CanRead)
                {
                    //【缓冲区不满足一个完整封包大小则继续从网络流读取数据,异步读取】
                    _netStream.BeginRead(_tempBuffer, 0, BUFFER_SIZE, new AsyncCallback(AsyncCallbackReadFromNetStream), _netStream);
                }
                return null;
            }
            catch (Exception e)
            {
                NetServiceErrorReason reason = new NetServiceErrorReason();
                reason.Exception = e.InnerException ?? e;
                this.ReceiveErrorHandler(reason);
                return null;
            }
        }

        /// <summary>
        /// 从网络流异步读取数据回调函数
        /// </summary>
        /// <param name="result"></param>
        private void AsyncCallbackReadFromNetStream(IAsyncResult result)
        {
            try
            {
                NetworkStream netStream = (NetworkStream)result.AsyncState;
                int readLen = netStream.EndRead(result);
                //判断读取的字节数+缓冲区已有字节数是否超过缓冲区总大小
                if (readLen + _netDataOffset > _netDataBuffer.Length)
                {
                    if (IsFullNetPacketHead())//如果缓冲区数据满足一个包头数据大小,则可以计算出本次接收的包需要的缓冲区大小,从而实现一次调整大小
                    {
                        Array.Resize<Byte>(ref _netDataBuffer, FullNetPacketSize);
                    }
                    else //不满足一个完整的网络封包的大小
                    {
                        Array.Resize<Byte>(ref _netDataBuffer, _netDataBuffer.Length + BUFFER_SIZE * 2);
                    }
                }
                //将新读取的数据拷贝到缓冲区
                Array.Copy(_tempBuffer, 0, _netDataBuffer, _netDataOffset, readLen);
                //修改"网络数据实际长度"
                _netDataOffset += readLen;

                //PickMessage();
            }
            catch (Exception e)
            {
                NetServiceErrorReason reason = new NetServiceErrorReason();
                reason.Exception = e.InnerException ?? e;
                this.ReceiveErrorHandler(reason);
            }
            finally
            {
                PickMessage();
            }
        }
        #endregion

        #region 发包
        /// <summary>
        /// 发包 
        /// </summary>
        /// <param name="packet"></param>
        public override void SendMessage(NetPacket packet)
        {
            if (packet == null || packet.Command == null || packet.PacketHead == null)
                return;

            if (OnBeforeSendPacket != null)
                OnBeforeSendPacket(packet);

            MemoryStream mStream = new MemoryStream();
            try
            {
                #region【计算包体长度】
                if (packet.PacketHead.Len == 0)
                {
                    packet.PacketHead.Len = GetCanSerializableObjectSize(packet.Command);
                }

                #endregion

                #region【写入包头】
                mStream.Write(BitConverter.GetBytes(packet.PacketHead.Version), 0, Marshal.SizeOf(packet.PacketHead.Version));
                mStream.Write(BitConverter.GetBytes((Int32)packet.PacketHead.PType), 0, sizeof(Int32));
                mStream.Write(BitConverter.GetBytes(packet.PacketHead.Len), 0, Marshal.SizeOf(packet.PacketHead.Len));
                #endregion

                #region【写入包体】
                byte[] buffer = null;

                using (MemoryStream m = new MemoryStream())
                {
                    SerializeHelper<BinarySerializeHelper>().Serialize(m, packet.Command);
                    m.Position = 0;
                    buffer = new byte[m.Length];
                    m.Read(buffer, 0, (Int32)m.Length);
                }

                mStream.Write(buffer, 0, buffer.Length);

                #endregion

                #region【将内存流一次写入网络流,异步写入】
                mStream.Seek(0, SeekOrigin.Begin);
                _netStream.BeginWrite(mStream.GetBuffer(), 0, (Int32)mStream.Length, new AsyncCallback(AsyncCallbackWriteToNetStream), new WriteNetStreamASyncCallbackParam { NetStream = _netStream, Packet = packet });
                #endregion
            }
            catch (Exception e)
            {
                NetServiceErrorReason reason = new NetServiceErrorReason();
                reason.Exception = e.InnerException ?? e;
                reason.SendData = packet.Command.Data;
                this.SendErrorHandler(reason);
            }
            mStream.Close();
        }

        /// <summary>
        /// 写入网络流异步回调函数
        /// </summary>
        /// <param name="result"></param>
        private void AsyncCallbackWriteToNetStream(IAsyncResult result)
        {
            WriteNetStreamASyncCallbackParam p = (WriteNetStreamASyncCallbackParam)result.AsyncState;
            try
            {
                p.NetStream.EndWrite(result);
            }
            catch (Exception e)
            {
                NetServiceErrorReason reason = new NetServiceErrorReason();
                reason.Exception = e.InnerException ?? e;
                reason.SendData = p.Packet.Command.Data;
                this.SendErrorHandler(reason);
                LogManger.Warn(e, this.GetType());
                p.NetStream.Close();
                return;
            }

            if (OnAfterSendPacket != null)
                OnAfterSendPacket(p.Packet.PacketHead);
        }
        /// <summary>
        /// 发包失败事件
        /// </summary>
        /// <param name="reason"></param>
        private void SendErrorHandler(NetServiceErrorReason reason)
        {
            SendErrorHandler handler = OnSendErrorHandler;
            if (handler != null) handler(reason);
        }
        #endregion
    }

    /// <summary>
    /// 写入网络流异步回调参数
    /// </summary>
    sealed class WriteNetStreamASyncCallbackParam
    {
        /// <summary>
        /// 网络流
        /// </summary>
        internal NetworkStream NetStream;
        /// <summary>
        /// 包头
        /// </summary>
        internal NetPacket Packet;
    }
}
