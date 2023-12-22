using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Diagnostics;
using UFOPlayer.ViewModels;
using UFOPlayer.Views;

namespace UFOPlayer
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            /*
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }*/
            
            
            var suspension = new AutoSuspendHelper(ApplicationLifetime);
            RxApp.SuspensionHost.CreateNewAppState = () => new MainWindowViewModel();
            RxApp.SuspensionHost.SetupDefaultSuspendResume(new NewtonsoftJsonSuspensionDriver("application_state.json"));
            suspension.OnFrameworkInitializationCompleted();

            var state = RxApp.SuspensionHost.GetAppState<MainWindowViewModel>();
            Debug.WriteLine(state);
            new MainWindow { DataContext = state }.Show();
            
            base.OnFrameworkInitializationCompleted();
            
        }


    }
}