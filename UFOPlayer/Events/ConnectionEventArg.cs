﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFOPlayer.Events
{
    public class ConnectionEventArg
    {
        public ConnectionStatus status;
        public ConnectionEventArg(ConnectionStatus status) {  this.status = status; }
    }
}
