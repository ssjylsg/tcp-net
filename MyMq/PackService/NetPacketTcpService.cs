using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace MyMq
{
    internal class NetPacketTcpService : NetPacketService
    {

        public NetPacketTcpService(NetworkStream netStream) : base(netStream) { }

        /// <summary>
        /// 提取一个完整网络包然后返回
        /// </summary>
        /// <returns></returns>
        public override NetPacket PickMessage()
        {
            #region 【接收Tcp包】

            while (true)
            {
                //判断是否满足一个完整封包大小
                if (IsFullNetPacket())//如果有完整封包就返回
                {
                    var packet = PickNetPacket();
                    return packet;
                }
                if (this.IsNetStreamCanRead == false) // 网络流关闭直接返回
                {
                    return null;
                }
                #region 【缓冲区不满足一个完整封包大小则继续从网络流读取数据】
                int readLen = _netStream.Read(_tempBuffer, 0, BUFFER_SIZE);
                //判断读取的字节数+缓冲区已有字节数是否超过缓冲区总大小
                if (readLen + _netDataOffset > _netDataBuffer.Length)
                {
                    Array.Resize<Byte>(ref _netDataBuffer, _netDataBuffer.Length + BUFFER_SIZE * 2);
                }
                //将新读取的数据拷贝到缓冲区
                Array.Copy(_tempBuffer, 0, _netDataBuffer, _netDataOffset, readLen);

                //修改"网络数据实际长度"
                _netDataOffset += readLen;
                #endregion
            }

            #endregion

        }

        /// <summary>
        /// 发包[发送Tcp包/发送Udp数据报]
        /// </summary>
        /// <param name="packet"></param>
        public override void SendMessage(NetPacket packet)
        {
            if (packet == null || packet.Command == null || packet.PacketHead == null)
                return;

            #region【计算包体长度】
            packet.PacketHead.Len = GetCanSerializableObjectSize(packet.Command);
            #endregion

            #region【写入包头】
            if (this.IsNetStreamCanWrite)
            {
                _netStream.Write(BitConverter.GetBytes(packet.PacketHead.Version), 0, Marshal.SizeOf(packet.PacketHead.Version));
                _netStream.Write(BitConverter.GetBytes((Int32)packet.PacketHead.PType), 0, sizeof(Int32));
                _netStream.Write(BitConverter.GetBytes(packet.PacketHead.Len), 0, Marshal.SizeOf(packet.PacketHead.Len));
            }
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

            if (buffer != null && this.IsNetStreamCanWrite)
                _netStream.Write(buffer, 0, buffer.Length);
            #endregion
        }
    }
}
