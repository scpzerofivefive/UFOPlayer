using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive;
using UFOPlayer.MediaSources.Vlc;
using UFOPlayer.ViewModels;

namespace UFOPlayer;

public partial class SettingsWindowView : Window
{
    public SettingsWindowView(SettingsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void BtnVlcDefault_Click(object sender, RoutedEventArgs e)
    {
        ((SettingsViewModel)DataContext).VlcEndpoint = VlcConnectionSettings.DefaultEndpoint;
    }

    private void BtnDeoVRDefault_Click(object sender, RoutedEventArgs e)
    {
        ((SettingsViewModel)DataContext).DeoVrEndpoint = new SettingsViewModel().DeoVrEndpoint;
    }

    private void Btn_Reset(object sender, RoutedEventArgs e)
    {
        DataContext = new SettingsViewModel();
    }

    private void Btn_Confirm(object sender, RoutedEventArgs e)
    {
        MainWindowViewModel.updateSetttings((SettingsViewModel)DataContext);
        Close();
    }

    private void Btn_Cancel(object sender, RoutedEventArgs e)
    {
        Close();
    }
}