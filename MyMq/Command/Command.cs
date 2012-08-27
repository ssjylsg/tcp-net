using System;

namespace MyMq
{
    /// <summary>
    /// MQ命令
    /// </summary>
    [Serializable]
    internal abstract class Command : ICommand
    {
        private CommandFlags _commandName;
        /// <summary>
        /// 命令标志
        /// </summary>
        /// <param name="commandName"></param>
        protected Command(CommandFlags commandName)
        {
            _commandName = commandName; 
        } 
        /// <summary>
        /// 命令枚举
        /// </summary>
        public CommandFlags CommandName
        {
            get { return this._commandName; }
        }

        private object _data;
        /// <summary>
        /// 发送数据包
        /// </summary>
        public object Data
        {
            get { return this._data; }
            set { this._data = value; }
        }

        private string _topicName;
        /// <summary>
        /// 主题名称
        /// </summary>
        public string TopicName
        {
            get { return this._topicName; }
            set { this._topicName = value; }
        }
    }
}
