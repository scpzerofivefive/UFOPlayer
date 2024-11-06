using Material.Icons.Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFOPlayer.MediaSources.Dummy
{
    class DummySourceFactory : AbstractSourceFactory
    {
        public override string Name => "";

        public override string IconPath => "/Assets/play-network.svg";

        public override AbstractMediaSource Create()
        {
            throw new NotImplementedException();
        }
    }
}
