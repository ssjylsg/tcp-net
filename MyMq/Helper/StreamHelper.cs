using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace MyMq
{
    internal class StreamHelper
    {
        public static Int32 GetCanSerializableObjectSize(object graph)
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                SerializeHelper<BinarySerializeHelper>().Serialize(mStream, graph);
                return (Int32)mStream.Length;
            }
        }
       
       
        public static ISerializeHelper SerializeHelper<T>() where T : ISerializeHelper
        {
            return Activator.CreateInstance<T>();
        }
        public static void SendData(NetPacket packet, NetworkStream networkStream)
        {

            #region【计算包体长度】
            switch (packet.PacketHead.PType)
            {
                case PacketType.STRING:
                    packet.PacketHead.Len = Encoding.Default.GetBytes(Convert.ToString(packet.Command)).Length;
                    break;
                case PacketType.BINARY:
                    //packet.PacketHead.Len = ((Byte[])packet.Command).Length;
                    break;
                case PacketType.COMPLEX:
                    packet.PacketHead.Len = StreamHelper.GetCanSerializableObjectSize(packet.Command);
                    break;
                default:
                    break;
            }

            #endregion

            #region【写入包头】
            networkStream.Write(BitConverter.GetBytes(packet.PacketHead.Version), 0, Marshal.SizeOf(packet.PacketHead.Version));
            networkStream.Write(BitConverter.GetBytes((Int32)packet.PacketHead.PType), 0, sizeof(Int32));
            networkStream.Write(BitConverter.GetBytes(packet.PacketHead.Len), 0, Marshal.SizeOf(packet.PacketHead.Len));
            #endregion

            #region【写入包体】
            byte[] buffer = null; 
            MemoryStream m = new MemoryStream();
            StreamHelper.SerializeHelper<BinarySerializeHelper>().Serialize(m, packet.Command);
            m.Position = 0;
            buffer = new byte[m.Length];
            m.Read(buffer, 0, (Int32)m.Length);
            
            networkStream.Write(buffer, 0, buffer.Length);
            #endregion
        }
    }
}
