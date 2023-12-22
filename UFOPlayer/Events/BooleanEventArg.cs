using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFOPlayer.Events
{
    public class BooleanEventArg
    {
        public bool Value { get; set; }

        public BooleanEventArg(bool value) { Value = value; }
    }
}
