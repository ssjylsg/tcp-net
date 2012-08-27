using System;

namespace MyMq
{
    internal interface ICommand
    {
        CommandFlags CommandName { get; }
        object Data { get; set; }
        string TopicName { get; set; }
    }
}
