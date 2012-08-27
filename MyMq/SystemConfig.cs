using System;
using System.Collections.Generic;
using System.Text;

namespace MyMq
{
    /// <summary>
    /// 系统配置参数
    /// </summary>
    internal class SystemConfig
    {
        /// <summary>
        /// 发布服务端口
        /// </summary>
        public static int PublishServerPort
        {
            get { return ConvertToInt(Resource.ProduceServerPort, 10002); }
        }
        private static int ConvertToInt(string input, int defaultValue)
        {
            int outNum;
            if (int.TryParse(input, out outNum))
            {
                return outNum;
            }
            return defaultValue;
        }
        /// <summary>
        /// 订阅服务端口
        /// </summary>
        public static int SubscriberServerPort
        {
            get { return ConvertToInt(Resource.SubscriberServerPort, 10001); }
        }
    }
}
