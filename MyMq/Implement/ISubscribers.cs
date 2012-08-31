namespace MyMq
{
    /// <summary>
    /// 订阅
    /// </summary>
    public interface ISubscribers
    {
        /// <summary>
        /// 接受到信息事件
        /// </summary>
        event ReceiveMessageEventHandler ReceiveMessage;
        /// <summary>
        /// 接收失败事件
        /// </summary>
        event ReceiveErrorHandler ReceiveMessageError;
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
        /// <summary>
        /// 客户端是否连接
        /// </summary>
        bool IsClientConnected { get; }
        /// <summary>
        /// 使用原IP+Port 重新连接
        /// </summary>
        void ReConnectServer();
    }
}
