using Shared.MediaSources;

namespace UFOPlayer.MediaSources.Dummy
{
    class DummySourceFactory : AbstractSourceFactory
    {
        public override string Name => "";

        public override string IconPath => "/Assets/play-network.svg";

        public override AbstractMediaSource Create()
        {
            return new DummyMediaSource();
        }
    }
}
