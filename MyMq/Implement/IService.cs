namespace MyMq
{
    /// <summary>
    /// 服务接口
    /// </summary>
    internal interface IService
    {
        /// <summary>
        /// 启动服务
        /// </summary>
        void StartService();
        /// <summary>
        /// 停止服务
        /// </summary>
        void Stop();
    }
}
