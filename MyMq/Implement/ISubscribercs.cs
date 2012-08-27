using System;
using System.Collections.Generic;
using System.Text;

namespace MyMq
{
    /// <summary>
    /// 订阅
    /// </summary>
    public interface ISubscribercs
    {
        /// <summary>
        /// 接受到信息事件
        /// </summary>
        event ReceiveMessageEventHandler OnReceiveMessageEventHandler;
        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="topicName"></param>
        void UnSubscribe(string topicName);
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="topicName"></param>
        void Subscribe(string topicName);
    }
}
