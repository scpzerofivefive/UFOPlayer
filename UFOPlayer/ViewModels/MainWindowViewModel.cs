using Avalonia.Controls;
using System.Diagnostics;
using System.Runtime.Serialization;
using UFOPlayer.Events;
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

namespace UFOPlayer.ViewModels
{
    [DataContract]
    public partial class MainWindowViewModel : ObservableObject
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