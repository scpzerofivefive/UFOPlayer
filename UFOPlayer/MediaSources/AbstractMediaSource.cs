using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UFOPlayer.Events;

namespace UFOPlayer.MediaSources
{

    public delegate void ConnectedEventHandler(object sender, ConnectionEventArg e);
    public abstract class AbstractMediaSource : IDisposable
    {
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
                    EventBus.Instance.InvokePlayingChanged(false);
                }
                ConnectionChangedEvent?.Invoke(this,
                    new ConnectionEventArg(value ? ConnectionStatus.Connected : ConnectionStatus.Connecting));
            }
        }

        public abstract void Dispose();

        protected virtual void OnFileOpened(string e)
        {
            EventBus.Instance.InvokeFileChanged(e);
        }

        protected virtual void OnDurationChanged(TimeSpan e)
        {
            EventBus.Instance.InvokeDurationChanged(e);
        }

        protected virtual void OnIsPlayingChanged(bool e)
        {
            EventBus.Instance.InvokePlayingChanged(e);
        }

        protected virtual void OnProgressChanged(TimeSpan progress)
        {
            EventBus.Instance.InvokeProgressChanged(progress);
        }

        protected virtual void OnPlaybackRateChanged(double rate)
        {
            EventBus.Instance.InvokePlaybackRateChanged(rate);
        }


        public event ConnectedEventHandler ConnectionChangedEvent;


    }
}
