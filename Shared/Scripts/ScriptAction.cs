using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Scripts
{
    public class ScriptAction
    {
        public sbyte Intensity { get; set; }

        private TimeSpan _timeSpan;
        public TimeSpan Timestamp
        {
            get
            {
                return TimeSpan.FromTicks(_timeSpan.Ticks);
            }
            set
            {
                _timeSpan = value;
            }
        }


        public ScriptAction(TimeSpan timestamp, sbyte intensity)
        {
            Timestamp = TimeSpan.FromTicks(timestamp.Ticks);
            Intensity = intensity;
        }
    }
}
