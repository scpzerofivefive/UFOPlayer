using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFOPlayer.Scripts;

namespace UFOPlayer.Events
{
    public class ActionEventArgs : EventArgs
    {
        public ScriptAction ScriptAction { get; set; }

        public ActionEventArgs(ScriptAction scriptAction)
        {
            ScriptAction = scriptAction;
        }
    }
}
