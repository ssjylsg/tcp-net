﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MyMq
{
    [Serializable]
    internal class ServerClosed : Command
    {
        public ServerClosed()
            : base(CommandFlags.ServerClosed)
        {

        }
    }
}
