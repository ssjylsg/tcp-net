using System;
using System.Collections.Generic;
using System.Text;

namespace MyMq
{
    /// <summary>
    /// 接受信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    interface IReceiveMessage<T>
    {
        T PickMessage();
    }
}
