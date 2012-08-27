using System;

namespace MyMq
{
    /// <summary>
    /// 取消订阅命令
    /// </summary>
    [Serializable]
    internal class UnSubscribeCommand : Command
    {
        public UnSubscribeCommand()
            : base(CommandFlags.UnSubscribe)
        {

        }
    }
    /// <summary>
    /// 命令枚举
    /// </summary>
    [Flags]
    internal enum CommandFlags
    {
        UnSubscribe,
        Subscribe,
        Publish
    }
}
