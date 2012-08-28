using System;
using System.Collections.Generic;
using System.Text;

namespace MyMq
{
    /// <summary>
    /// 包类型
    /// </summary>
    internal enum PacketType
    {
        /// <summary>
        /// 简单字符串传输
        /// </summary>
        STRING = 0,
        /// <summary>
        /// 二进制[文件传输]
        /// </summary>
        BINARY = 1,
        /// <summary>
        /// 复杂对象传输[使用序列化和反序列化]
        /// </summary>
        COMPLEX = 2
    }
    /// <summary>
    /// 包头
    /// </summary>
    internal class NetPacketHead
    {
        /// <summary>
        /// 包头大小
        /// </summary>
        public const Int32 HEAD_SIZE = 4 * 3;

        private Int32 _version = 1;

        /// <summary>
        /// 版本
        /// </summary>
        public Int32 Version
        {
            get { return _version; }
            set { _version = value; }
        }
        private Int32 _len = 0;
        /// <summary>
        /// 包体长度[决定后面data数据的长度]
        /// </summary>
        public Int32 Len
        {
            get { return _len; }
            set { _len = value; }
        }
        private PacketType _pType = PacketType.STRING;
        /// <summary>
        /// 包类型[决定如何解包]
        /// </summary>
        public PacketType PType
        {
            get { return _pType; }
            set { _pType = value; }
        }
    }
}
