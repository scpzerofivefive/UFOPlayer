using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFOPlayer.MediaSources.Vlc
{
    public class VlcConnectionSettings
    {
        public string Password { get; set; } = "";
        public string IpAndPort { get; set; }

        public const string DefaultEndpoint = "127.0.0.1:8080";

        public VlcConnectionSettings()
        {
            IpAndPort = DefaultEndpoint;
        }
    }
}
