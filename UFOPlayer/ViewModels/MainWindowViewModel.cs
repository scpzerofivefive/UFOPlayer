using System.Diagnostics;
using System.Runtime.Serialization;
using UFOPlayer.Events;

namespace UFOPlayer.ViewModels
{
    [DataContract]
    public class MainWindowViewModel : ViewModelBase
    {
        [IgnoreDataMember]
        public ScriptViewModel Script { get; } = new ScriptViewModel();

        [IgnoreDataMember]
        public DeviceViewModel Device { get; } = new DeviceViewModel();

        [IgnoreDataMember]
        public MediaSourceViewModel Media { get; }

        [DataMember]
        private static SettingsViewModel _settings = new SettingsViewModel();
        public static SettingsViewModel Settings {
            get
            {
                return _settings;
            }
            set
            {
                _settings = value;

            }
        }

        public static void updateSetttings(SettingsViewModel settings)
        {
            Settings = settings;
            EventBus.Instance.InvokeSetttingsUpdate();
        }

        public MainWindowViewModel()
        {
            Media = new MediaSourceViewModel();
        }

    }
}