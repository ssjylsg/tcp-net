using System;
using System.Collections.Generic;
using System.Text;

namespace MyMq
{
    interface IReceiveMessage<T>
    {
        T PickMessage();
    }
}
