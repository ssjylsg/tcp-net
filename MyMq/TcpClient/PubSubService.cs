using System;
using System.Collections.Generic;
using System.Text;

namespace MyMq
{
    public class PubSubService
    {
        private IService _pubService;
        private IService _subService;
        /// <summary>
        /// 初始化TCP 服务
        /// </summary>
        public void InitTcpService()
        {
            this._pubService = new PublishTcpService();
            this._subService = new TcpSubscribersService();
        }
        /// <summary>
        /// 初始化Socket服务
        /// </summary>
        public void InitSocketService()
        {
            this._pubService = new ProducerSocketService();
            this._subService = new SocketSubscriberService();
        }
        /// <summary>
        /// 启动服务
        /// </summary>
        public void Start()
        {
            this._pubService.StartService();
            this._subService.StartService();
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            if (_pubService != null)
            {
                this._pubService.Stop();
            }
            if (_subService != null)
            {
                this._subService.Stop();
            }
        }
        /// <summary>
        /// 清空所有的订阅者
        /// </summary>
        public void ClearAllScriber()
        {
            TcpClientFilter.SubscribersList.Clear();
        }
        /// <summary>
        /// 测试代码
        /// </summary>
        /// <param name="handler"></param>
        public void TestForPublish(ReceiveMessageEventHandler handler)
        {
            PublishTcpService.ReceiveMessageEventHandler += handler;
        }
    }
}
