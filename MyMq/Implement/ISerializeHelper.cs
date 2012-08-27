using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace MyMq
{
    internal interface ISerializeHelper
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="item"></param>
        void Serialize<T>(System.IO.Stream stream, T item);

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        T DeSerialize<T>(System.IO.Stream stream);


    }

   internal class BinarySerializeHelper : ISerializeHelper
    {

        public void Serialize<T>(System.IO.Stream stream, T item)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            formatter.Serialize(stream, item);
        }

        public T DeSerialize<T>(System.IO.Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
        }
    }
}
