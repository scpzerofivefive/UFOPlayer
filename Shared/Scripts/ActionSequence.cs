using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Scripts
{
    internal class ActionSequence : LinkedList<ScriptAction>
    {

        string Name { get; set; }
    }
}
