using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Scripts
{
    public class MediaSyncController
    {

        private TimeSpan originTime { get; set; } = new TimeSpan();
        private double playback_rate { get; set; } = 1;
        private Stopwatch _stopwatch = new Stopwatch();

        public MediaSyncController()
        {

        }

        public void Play()
        {
            _stopwatch.Start();
        }

        public bool isPlaying()
        {
            return _stopwatch.IsRunning;
        }

        public void Pause()
        {
            _stopwatch.Stop();
            refresh();
        }

        public void SetPosition(TimeSpan position)
        {
            originTime = position;
            if (_stopwatch.IsRunning)
            {
                _stopwatch.Restart();
            }
            else
            {
                _stopwatch.Reset();
            }
        }

        public TimeSpan getTimeSpan()
        {
            return originTime + _stopwatch.Elapsed.Multiply(playback_rate);
        }

        public void setPlaybackRate(double e)
        {
            refresh();
            playback_rate = e;
        }

        private void refresh()
        {
            originTime = getTimeSpan();
            if (_stopwatch.IsRunning)
            {
                _stopwatch.Restart();
            }
            else
            {
                _stopwatch.Reset();
            }
        }

        public void reset()
        {
            originTime = new TimeSpan();
            _stopwatch.Reset();
            playback_rate = 1; //TODO
        }
    }
}
