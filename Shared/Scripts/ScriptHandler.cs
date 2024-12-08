using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using Shared.MediaSources;

namespace Shared.Scripts
{

    public delegate void ScriptCommandEventHandler(ScriptCommand cmd);

    public partial class ScriptHandler : ObservableObject
    {
        public event ScriptCommandEventHandler ScriptCommandRaised;

        private CancellationTokenSource _cancellationTokenSource;

        private AbstractMediaSource _mediaSource;

        public AbstractMediaSource MediaSource
        {
            get { return _mediaSource; }
            set
            {
                if (_mediaSource != null)
                {
                    _mediaSource.ProgressChangedEvent -= OnProgressChanged;
                    _mediaSource.DurationChangedEvent -= OnDurationChanged;
                    _mediaSource.IsPlayingChangedEvent -= OnPlayingChanged;
                    _mediaSource.PlaybackRateChangedEvent -= OnPlaybackRateChanged;
                }
                _mediaSource = value;
                if (_mediaSource != null)
                {
                    _mediaSource.ProgressChangedEvent += OnProgressChanged;
                    _mediaSource.DurationChangedEvent += OnDurationChanged;
                    _mediaSource.IsPlayingChangedEvent += OnPlayingChanged;
                    _mediaSource.PlaybackRateChangedEvent += OnPlaybackRateChanged;

                }
            }
        }

        public MediaSyncController PlaybackClock { get; } = new MediaSyncController();

        [ObservableProperty]
        private UFOScript _script = new UFOScript();

        [ObservableProperty]
        private TimeSpan _elapsedTime = new TimeSpan(0);


        [ObservableProperty]
        private TimeSpan _mediaDuration = new TimeSpan(0);

        public ScriptHandler()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            // Start the script loop in a new task
            Task.Run(async () =>
            {
                await ScriptLoop(_cancellationTokenSource.Token);
            });

        }


        public async Task ScriptLoop(CancellationToken cancellationToken)
        {
            ScriptCommand prevCmd = null;
            while (!cancellationToken.IsCancellationRequested)
            {
                TimeSpan timeSpan = PlaybackClock.getTimeSpan();
                ElapsedTime = timeSpan;
                // Process actions based on current playback time
                ScriptCommand cmd = Script.goTo(timeSpan);
                if (!cmd.Equals(prevCmd))
                {
                    RaiseScriptCommand(cmd);
                }

                prevCmd = cmd;
                await Task.Delay(10);
            }
        }


        protected virtual void RaiseScriptCommand(ScriptCommand e)
        {
            ScriptCommandRaised?.Invoke(e);
        }

        private void OnProgressChanged(TimeSpan arg)
        {

            PlaybackClock.SetPosition(arg);
            Script.goTo(arg);
            ElapsedTime = arg;
            // TODO Is Paused check
        }

        public void OnPlayingChanged(bool arg)
        {
            if (arg)
            {
                PlaybackClock.Play();
            }
            else
            {
                PlaybackClock.Pause();
                ScriptCommandRaised?.Invoke(new ScriptCommand(0, 0));
            }
        }

        public void OnPlaybackRateChanged(double arg)
        {
            PlaybackClock.setPlaybackRate(arg);
        }


        public void OnDurationChanged(TimeSpan arg)
        {
            MediaDuration = arg;
        }
    }
}
