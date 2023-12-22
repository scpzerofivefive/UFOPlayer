using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFOPlayer.Events
{
    public class DoubleEventArg : EventArgs
    {
        public double Value { get; set; }

        public DoubleEventArg(double v) {
            Value = v;
        }
    }
}
