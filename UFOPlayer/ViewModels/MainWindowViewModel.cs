using Avalonia.Controls;
using System.Diagnostics;
using System.Runtime.Serialization;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia;
using System.Collections.ObjectModel;
using ReactiveUI;
using System.Reactive;
using DynamicData;
using System;
using Material.Icons;
using UFOPlayer.Scripts;

namespace UFOPlayer.ViewModels
{

    public delegate void SettingUpdate(SettingsViewModel settings);

    [DataContract]
    public partial class MainWindowViewModel : ObservableObject
    {
        [IgnoreDataMember]
        public ScriptHandler ScriptHandler {  get; }
        [IgnoreDataMember]
        public ScriptViewModel Script { get; }
        [IgnoreDataMember]
        public DeviceViewModel Device { get; }
        [IgnoreDataMember]
        public MediaSourceViewModel Media { get; }


        public static SettingUpdate onSettingsUpdate { get; set; }

        private static SettingsViewModel _settings = new SettingsViewModel();


        [DataMember]
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

        public void updateSetttings(SettingsViewModel settings)
        {
            Settings = settings;
            Media.settingsUpdatedHandler();
        }


        public MainWindowViewModel()
        {
            ScriptHandler = new ScriptHandler();
            Script = new ScriptViewModel(ScriptHandler);
            Device = new DeviceViewModel(ScriptHandler);
            Media = new MediaSourceViewModel(Script);
            onSettingsUpdate = updateSetttings;

            onVisualizerModePressed = ReactiveCommand.Create(cycleVisualizer);
        }

        public ReactiveCommand<Unit, Unit> onVisualizerModePressed { get; }

        private void cycleVisualizer()
        {
            if (Script.Mode == VisualizerMode.Line)
            {
                Script.Mode = VisualizerMode.Bar;
                IconKind = MaterialIconKind.ChartBar;

            } else
            {
                Script.Mode = VisualizerMode.Line;
                IconKind = MaterialIconKind.ChartLine;
            }
            Debug.WriteLine(Script.Mode);
        }


        [ObservableProperty]
        private MaterialIconKind _iconKind =  MaterialIconKind.ChartLine;
    }
}