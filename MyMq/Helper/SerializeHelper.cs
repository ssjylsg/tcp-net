using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MyMq
{
    internal static class SerializeHelper
    {
        /// <summary>
        /// 序列化
        /// </summary> 
        public static byte[] ObjectToBytes(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.TypeFormat = System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways;
                formatter.Serialize(ms, obj);
                return ms.GetBuffer();
            }
        }

        /// <summary>
        ///  反序列化
        /// </summary> 
        public static object BytesToObject(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(bytes,0,bytes.Length);
                ms.Position = 0;
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.TypeFormat = System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways;
                ms.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(ms); 
            }
        }
    }
}
