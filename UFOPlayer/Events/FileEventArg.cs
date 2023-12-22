using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFOPlayer.Events
{
    public class FileEventArg : EventArgs
    {

        public string Filepath { get; set; }
        public FileEventArg(String filepath) {
            Filepath = filepath;
        }
    }
}
