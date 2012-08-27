using System;

namespace MyMq
{
    /// <summary>
    /// 订阅命令
    /// </summary>
    [Serializable]
    class SubscriberCommand : Command
    {
        public SubscriberCommand()
            : base(CommandFlags.Subscribe)
        {

        }
    }
}
