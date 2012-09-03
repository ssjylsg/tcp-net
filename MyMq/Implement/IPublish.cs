namespace MyMq
{
    /// <summary>
    /// 消息发布
    /// </summary>
    public interface IPublish
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="serverIP"></param>
        /// <param name="serverPort"></param>
        void Init(string serverIP, int serverPort);
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="topicName">主题</param>
        /// <param name="data">数据</param>
        void Send(string topicName, object data);
        /// <summary>
        /// 发包失败
        /// </summary>
        event SendErrorHandler SendMessageError;
    }
}
