using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json.Linq;
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
using UFOPlayer.MediaSources;
using UFOPlayer.MediaSources.Dummy;
using UFOPlayer.MediaSources.HereSphere;
using UFOPlayer.MediaSources.Vlc;
using UFOPlayer.Scripts;

namespace UFOPlayer.ViewModels
{

    public partial class MediaSourceViewModel : ObservableObject, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; }

        private readonly ScriptViewModel scriptHandler;


        public List<AbstractSourceFactory> SourceList { get; }
            = new List<AbstractSourceFactory> { new VLCSourceFactory(), new DeoVrSourceFactory()};

        [ObservableProperty]
        private ConnectionStatus _status = ConnectionStatus.Disconnected;


        public MediaSourceViewModel(ScriptViewModel script) {

            scriptHandler = script;
        }

        [ObservableProperty]
        private AbstractSourceFactory _selectedSourceFactory = new DummySourceFactory();

        partial void OnSelectedSourceFactoryChanged(AbstractSourceFactory value)
        {
            Source = value.Create();
        }


        private AbstractMediaSource _source { get; set; }
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
                if (_source != null)
                {
                    _source.ConnectionChangedEvent += connectionChangedHandler;

                    scriptHandler.MediaSource = _source;
                }
            }
        }

        private void connectionChangedHandler(ConnectionStatus e)
        {
            Status = e;
        }

        public void settingsUpdatedHandler()
        {
            //PlaybackModeChanged(Mode);
            Source = SelectedSourceFactory.Create();
            Debug.WriteLine("Source Settings Updated");
        }

        


        [ObservableProperty]
        private bool _isConnected = false;

        partial void OnStatusChanged(ConnectionStatus status)
        {
            IsConnected = status == ConnectionStatus.Connected;
        }

    }
}
