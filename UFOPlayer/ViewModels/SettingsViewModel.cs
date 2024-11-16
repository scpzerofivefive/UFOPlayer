using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UFOPlayer.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        public static string VERSION { get; } = "Version: " + Assembly.GetEntryAssembly().GetName().Version.ToString();

        private static readonly string defaultregex = "^(.*ufo.*{0}.*|.*{0}.*ufo.*)$";

        private static readonly string defaultVlcEndpoint = "127.0.0.1:8080";

        private static readonly string defaultVlcPassword = "1234";

        private static readonly string defaultDeoVrEndpoint = "localhost:23554";



        [ObservableProperty]
        private string _regex = defaultregex;


        [ObservableProperty]
        private string _vlcEndpoint = defaultVlcEndpoint;

        [ObservableProperty]
        private string _vlcPassword = defaultVlcPassword;



        [ObservableProperty]
        private string _deoVrEndpoint = defaultDeoVrEndpoint;

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
