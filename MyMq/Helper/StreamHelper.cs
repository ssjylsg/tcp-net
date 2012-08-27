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
       
    }
}
