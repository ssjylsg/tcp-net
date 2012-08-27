using System;
using System.Collections.Generic;
using System.Text;

namespace MyMq
{
    interface IMessageStore<T>
    {
        void StoreMessage(T message);
    }
}
