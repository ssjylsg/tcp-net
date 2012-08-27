using System;

namespace MyMq
{
    /// <summary>
    /// 发布命令
    /// </summary>
    [Serializable]
    internal class PublishCommand : Command
    {
        public PublishCommand()
            : base(CommandFlags.Publish)
        {
            
        }
    }
}
