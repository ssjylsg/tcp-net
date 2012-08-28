namespace MyMq
{
    /// <summary>
    /// 消息存储
    /// </summary>
    /// <typeparam name="T"></typeparam>
    interface IMessageStore<T>
    {
        void StoreMessage(T message);
    }
}
