using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFOPlayer.Events
{
    public class TimeSpanEventArg : EventArgs
    {
        public TimeSpan time { get; set; }
        public TimeSpanEventArg(TimeSpan time) {
            this.time = time;
        }
    }
}
