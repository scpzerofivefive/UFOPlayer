using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
