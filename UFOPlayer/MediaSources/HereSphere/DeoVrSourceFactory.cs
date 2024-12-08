using Shared.MediaSources;
using UFOPlayer.ViewModels;

namespace UFOPlayer.MediaSources.HereSphere
{
    public class DeoVrSourceFactory : AbstractSourceFactory
    {
        public override string Name => "DeoVr";

        public override string IconPath => "/Assets/deo-icon.svg";

        public override AbstractMediaSource Create()
        {
            return new HereSphereMediaSource(new SimpleTcpConnectionSettings
            {
                IpAndPort = MainWindowViewModel.Settings.DeoVrEndpoint
            });
        }
    }
}
