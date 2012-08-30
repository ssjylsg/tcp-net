using System;
using System.Collections.Generic;
using System.Text;

namespace MyMq
{
    /// <summary>
    /// 网络包
    /// </summary>
    internal class NetPacket
    {
        public NetPacket()
        {
            this._packetHead = new NetPacketHead();
            this._packetHead.Version = 1;
            this._packetHead.PType = PacketType.COMPLEX;
        }
        private ICommand _command;
        private NetPacketHead _packetHead;
        /// <summary>
        /// 包体
        /// </summary>
        public ICommand Command
        {
            get { return _command; }
            set { _command = value; }
        }
        /// <summary>
        /// 包头
        /// </summary>
        public NetPacketHead PacketHead
        {
            get { return _packetHead; }
            set { _packetHead = value; }
        }
    }
}
