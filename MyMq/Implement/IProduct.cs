namespace MyMq
{
    /// <summary>
    /// 消息发布
    /// </summary>
    public interface IProduct
    {
        void Init(string serverIP, int serverPort);
        void Send(string topicName, object data);
        /// <summary>
        /// 发包失败
        /// </summary>
        event SendErrorHandler OnSendErrorHandler;
    }
}
