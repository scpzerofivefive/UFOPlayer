using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace UFOPlayer.MediaSources
{

    public delegate void ConnectedEventHandler(ConnectionStatus e);
    public delegate void IsPlayingEventHandler(bool e);
    public delegate void TimeSpanEventHandler(TimeSpan e);
    public delegate void PlaybackRateChangedEventHandler(double rate);
    public delegate void FileOpenedEventHandler(string filePath);


    public abstract class AbstractMediaSource : IDisposable
    {

        public event IsPlayingEventHandler IsPlayingChangedEvent;
        public event TimeSpanEventHandler ProgressChangedEvent;
        public event PlaybackRateChangedEventHandler PlaybackRateChangedEvent;
        public event TimeSpanEventHandler DurationChangedEvent;
        public event FileOpenedEventHandler FileOpenedEvent;
        public event ConnectedEventHandler ConnectionChangedEvent;

        public bool _isConnected {  get; set; }


        public bool IsConnected { 
            get
            {
                return _isConnected;
            } 
            set
            {
                _isConnected = value;
                if (!_isConnected)
                {
                    OnIsPlaying(false);
                }
                ConnectionChangedEvent?.Invoke(value ? ConnectionStatus.Connected : ConnectionStatus.Connecting);
            }
        }

        public abstract void Dispose();

        protected virtual void OnFileOpened(string e)
        {
            FileOpenedEvent?.Invoke(e);
        }

        protected virtual void OnDurationChanged(TimeSpan e)
        {
            DurationChangedEvent?.Invoke(e);
        }

        protected virtual void OnIsPlaying(bool e)
        {
            IsPlayingChangedEvent?.Invoke(e);
        }

        protected virtual void OnProgressChanged(TimeSpan progress)
        {
            ProgressChangedEvent?.Invoke(progress);
        }

        protected virtual void OnPlaybackRateChanged(double rate)
        {
            PlaybackRateChangedEvent?.Invoke(rate);
        }




    }
}
