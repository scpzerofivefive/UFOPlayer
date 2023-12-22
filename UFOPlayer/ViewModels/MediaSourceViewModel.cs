using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using UFOPlayer.Events;
using UFOPlayer.MediaSources;

namespace UFOPlayer.ViewModels
{

    public delegate AbstractMediaSource MediaSource();
    public partial class MediaSourceViewModel : ObservableObject, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; }

        public PlaybackMode[] PlaybackModes { get; } = ((PlaybackMode[])Enum.GetValues(typeof(PlaybackMode))).Skip(1).ToArray();

        [ObservableProperty]
        private ConnectionStatus _status = ConnectionStatus.Disconnected;

        public MediaSourceViewModel() {
            EventBus.SettingsUpdatedEvent += settingsUpdatedHandler;
        }

        private PlaybackMode mode = PlaybackMode.None;
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
            PlaybackModeChanged(mode);
            Debug.WriteLine("Source Settings Updated");
        }

        public void PlaybackModeChanged(PlaybackMode newValue)
        {
            mode = newValue;
            try
            {

                switch (newValue)
                {
                    /*
                    
                    case PlaybackMode.Whirligig:
                        {
                            HideBanner();

                            if (string.IsNullOrWhiteSpace(Settings.WhirligigEndpoint))
                            {
                                WhirligigConnectionSettings settings =
                                    OnRequestWhirligigConnectionSettings(new WhirligigConnectionSettings
                                    {
                                        IpAndPort = WhirligigConnectionSettings.DefaultEndpoint
                                    });

                                if (settings == null)
                                {
                                    PlaybackMode = PlaybackMode.Local;
                                    return;
                                }

                                Settings.WhirligigEndpoint = settings.IpAndPort;
                            }

                            TimeSource = new WhirligigTimeSource(new DispatcherClock(
                                Dispatcher.FromThread(Thread.CurrentThread),
                                TimeSpan.FromMilliseconds(10)), new WhirligigConnectionSettings
                                {
                                    IpAndPort = Settings.WhirligigEndpoint
                                });

                            ((WhirligigTimeSource)TimeSource).FileOpened += OnVideoFileOpened;

                            RefreshManualDuration();
                            break;
                        }
                    case PlaybackMode.ZoomPlayer:
                        {
                            HideBanner();

                            if (string.IsNullOrWhiteSpace(Settings.ZoomPlayerEndpoint))
                            {
                                ZoomPlayerConnectionSettings settings =
                                    OnRequestZoomPlayerConnectionSettings(new ZoomPlayerConnectionSettings
                                    {
                                        IpAndPort = ZoomPlayerConnectionSettings.DefaultEndpoint
                                    });

                                if (settings == null)
                                {
                                    PlaybackMode = PlaybackMode.Local;
                                    return;
                                }

                                Settings.ZoomPlayerEndpoint = settings.IpAndPort;
                            }

                            TimeSource = new ZoomPlayerTimeSource(new DispatcherClock(
                                Dispatcher.FromThread(Thread.CurrentThread),
                                TimeSpan.FromMilliseconds(10)), new ZoomPlayerConnectionSettings
                                {
                                    IpAndPort = Settings.ZoomPlayerEndpoint
                                });

                            ((ZoomPlayerTimeSource)TimeSource).FileOpened += OnVideoFileOpened;

                            RefreshManualDuration();
                            break;
                        }*/
                    case PlaybackMode.VLC:
                        {

                            if (string.IsNullOrWhiteSpace(MainWindowViewModel.Settings.VlcEndpoint) ||
                                string.IsNullOrWhiteSpace(MainWindowViewModel.Settings.VlcPassword))
                            {
                                //reset to default
                            }

                            Source = new VLCMediaSource(new VlcConnectionSettings
                                    {
                                        IpAndPort = MainWindowViewModel.Settings.VlcEndpoint,
                                        Password = MainWindowViewModel.Settings.VlcPassword
                                    });


                            break;
                        }
                        
                    case PlaybackMode.DeoVR:
                        {
                            if (string.IsNullOrWhiteSpace(MainWindowViewModel.Settings.DeoVrEndpoint))
                            {
                                /*
                                if (settings == null)
                                {
                                    PlaybackMode = PlaybackMode.Local;
                                    return;
                                }
                                */

                            }

                            Source = new HereSphereMediaSource( new SimpleTcpConnectionSettings
                                    {
                                        IpAndPort = MainWindowViewModel.Settings.DeoVrEndpoint
                                    });


                            break;
                        }
                        /*
                    case PlaybackMode.MpcHc:
                        {
                            HideBanner();

                            if (string.IsNullOrWhiteSpace(Settings.MpcHcEndpoint))
                            {
                                SimpleTcpConnectionSettings settings =
                                    OnRequestSimpleTcpConnectionSettings(new SimpleTcpConnectionSettings
                                    {
                                        IpAndPort = MpcTimeSource.DefaultEndpoint
                                    });

                                if (settings == null)
                                {
                                    PlaybackMode = PlaybackMode.Local;
                                    return;
                                }

                                Settings.MpcHcEndpoint = settings.IpAndPort;
                            }

                            TimeSource = new MpcTimeSource(
                            new DispatcherClock(Dispatcher.FromThread(Thread.CurrentThread),
                                TimeSpan.FromMilliseconds(10)), new SimpleTcpConnectionSettings
                                {
                                    IpAndPort = Settings.MpcHcEndpoint
                                });

                            ((MpcTimeSource)TimeSource).FileOpened += OnVideoFileOpened;

                            break;
                        }
                    case PlaybackMode.Kodi:
                        {
                            HideBanner();
                            if (Settings.KodiHttpPort == 0)
                            {
                                KodiConnectionSettings settings =
                                    OnRequestKodiConnectionSettings(new KodiConnectionSettings
                                    {
                                        HttpPort = KodiConnectionSettings.DefaultHttpPort,
                                        TcpPort = KodiConnectionSettings.DefaultTcpPort,
                                        Ip = KodiConnectionSettings.DefaultIp,
                                        Password = KodiConnectionSettings.DefaultPassword,
                                        User = KodiConnectionSettings.DefaultUser
                                    });

                                if (settings == null)
                                {
                                    PlaybackMode = PlaybackMode.Local;
                                    return;
                                }
                                Settings.KodiIp = settings.Ip;
                                Settings.KodiHttpPort = settings.HttpPort;
                                Settings.KodiTcpPort = settings.TcpPort;
                                Settings.KodiUser = settings.User;
                                Settings.KodiPassword = settings.Password;

                            }
                            TimeSource = new KodiTimeSource(
                                new DispatcherClock(Dispatcher.FromThread(Thread.CurrentThread),
                                    TimeSpan.FromMilliseconds(10)), new KodiConnectionSettings
                                    {
                                        Ip = Settings.KodiIp,
                                        HttpPort = Settings.KodiHttpPort,
                                        TcpPort = Settings.KodiTcpPort,
                                        User = Settings.KodiUser,
                                        Password = Settings.KodiPassword
                                    });

                            ((KodiTimeSource)TimeSource).FileOpened += OnVideoFileOpened;
                            RefreshManualDuration();
                            break;
                        }
                    */
                    default:
                        mode = PlaybackMode.None;
                        Status = ConnectionStatus.Disconnected;
                        break;
                }
            }
            finally
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }

    }
}
