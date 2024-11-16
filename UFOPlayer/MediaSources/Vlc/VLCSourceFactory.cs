using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFOPlayer.ViewModels;

namespace UFOPlayer.MediaSources.Vlc
{
    public class VLCSourceFactory : AbstractSourceFactory
    {
        public override string Name => "VLC";

        public override string IconPath => "/Assets/vlc-logo.svg";

        public override VLCMediaSource Create()
        {
            return new VLCMediaSource(new VlcConnectionSettings
            {
                IpAndPort = MainWindowViewModel.Settings.VlcEndpoint,
                Password = MainWindowViewModel.Settings.VlcPassword
            });
        }
    }
}
