using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Diagnostics;
using UFOPlayer.ViewModels;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;


namespace UFOPlayer.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void OpenSettings(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindowView(
                new SettingsViewModel(MainWindowViewModel.Settings));
            var result = await settingsWindow.ShowDialog<bool>(this);
            if (result == true) { }
        }

    }
}