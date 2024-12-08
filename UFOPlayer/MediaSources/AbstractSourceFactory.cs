

using Shared.MediaSources;

namespace UFOPlayer.MediaSources
{
    public abstract class AbstractSourceFactory
    {

        public AbstractSourceFactory() { }

        public abstract string Name { get; }

        public abstract string IconPath { get; }

        public abstract AbstractMediaSource Create();
    }
}