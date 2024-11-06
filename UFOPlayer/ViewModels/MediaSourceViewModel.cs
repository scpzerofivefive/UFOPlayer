using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using UFOPlayer.Events;
using UFOPlayer.MediaSources;
using UFOPlayer.MediaSources.Dummy;
using UFOPlayer.MediaSources.Vlc;

namespace UFOPlayer.ViewModels
{

    public delegate AbstractMediaSource MediaSource();
    public partial class MediaSourceViewModel : ObservableObject, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; }

        public List<AbstractSourceFactory> SourceList { get; }
            = new List<AbstractSourceFactory> { new VLCSourceFactory() };

        [ObservableProperty]
        private ConnectionStatus _status = ConnectionStatus.Disconnected;


        public MediaSourceViewModel() {
            EventBus.SettingsUpdatedEvent += settingsUpdatedHandler;
        }

        [ObservableProperty]
        private AbstractSourceFactory _selectedSourceFactory = new DummySourceFactory();
        

        private AbstractMediaSource _source {  get; set; }
        public AbstractMediaSource Source {
            get
            {
                return _source;
            }
            set
            {
                if (_source != null) {
                    _source.Dispose();
                    _source.ConnectionChangedEvent -= connectionChangedHandler;
                }
                Status = ConnectionStatus.Connecting;
                //Clear source
                _source = value;
                _source.ConnectionChangedEvent += connectionChangedHandler;
            }
        }

        private void connectionChangedHandler(object sender, ConnectionEventArg e)
        {
            Status = e.status;
        }

        private void settingsUpdatedHandler(object sender, EventArgs e)
        {
            //PlaybackModeChanged(Mode);
            Debug.WriteLine("Source Settings NOT Updated");
        }

        partial void OnSelectedSourceFactoryChanged(AbstractSourceFactory value)
        {
            Source = value.Create();
        }


        [ObservableProperty]
        private bool _isConnected = false;

        partial void OnStatusChanged(ConnectionStatus status)
        {
            IsConnected = status == ConnectionStatus.Connected;
        }

    }
}
