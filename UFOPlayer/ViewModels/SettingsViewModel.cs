using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UFOPlayer.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        public static string VERSION { get; } = "Version: " + Assembly.GetEntryAssembly().GetName().Version.ToString();

        [ObservableProperty]
        private string _vlcEndpoint = "127.0.0.1:8080";

        [ObservableProperty]
        private string _vlcPassword = "1234";

        [ObservableProperty]
        private string _deoVrEndpoint = "localhost:23554";

        public SettingsViewModel() {
        
        }

        public SettingsViewModel(SettingsViewModel settings) {
            VlcEndpoint = settings.VlcEndpoint;
            VlcPassword = settings.VlcPassword;
        }

        public override bool Equals(object obj)
        {
            if (obj is SettingsViewModel)
            {
                SettingsViewModel o = (SettingsViewModel)obj;
                return 
                    VlcEndpoint.Equals(o.VlcEndpoint) &&
                    VlcPassword.Equals(o.VlcPassword);
            } else
            {
                return false;
            }

        }
    }
}
