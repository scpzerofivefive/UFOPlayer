using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFOPlayer.MediaSources;

namespace UFOPlayer.ViewModels
{
    public class AppSettings
    {
        public VlcConnectionSettings VlcConnectionSettings { get; set; } = new VlcConnectionSettings();

    }
}
