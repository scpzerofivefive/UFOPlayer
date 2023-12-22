using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaProgressRing;
using System.Diagnostics;
using System.Windows;

namespace UFOPlayer.Views
{
    public partial class StatusView : UserControl
    {

        PathIcon _icon;
        ProgressRing ProgressRing { get; set; }

        public static readonly StyledProperty<ConnectionStatus> StatusProperty =
        AvaloniaProperty.Register<StatusView, ConnectionStatus>(nameof(Status), defaultValue: ConnectionStatus.Connected);

        public static readonly StyledProperty<StreamGeometry> ConnectedProperty =
        AvaloniaProperty.Register<StatusView, StreamGeometry>(nameof(Connected));

        public static readonly StyledProperty<StreamGeometry> DisconnectedProperty =
        AvaloniaProperty.Register<StatusView, StreamGeometry>(nameof(Disconnected));


        public StatusView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Load XAML
            AvaloniaXamlLoader.Load(this);
            _icon = this.FindControl<PathIcon>("pathIcon");
            ProgressRing = this.FindControl<ProgressRing>("progress");
        }

        public ConnectionStatus Status
        {
            get => GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        public StreamGeometry Connected
        {
            get => GetValue(ConnectedProperty);
            set => SetValue(ConnectedProperty, value);
        }

        public StreamGeometry Disconnected
        {
            get => GetValue(DisconnectedProperty);
            set => SetValue(DisconnectedProperty, value);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            if (change.Property == StatusProperty)
            {
                Debug.WriteLine("Status: " +  Status);
                switch (Status)
                {
                    case ConnectionStatus.Connected:
                        _icon.IsVisible = true;
                        _icon.Data = Connected;
                        _icon.Foreground = Brushes.YellowGreen;
                        ProgressRing.IsVisible = false;
                        break;
                    case ConnectionStatus.Disconnected:
                        _icon.IsVisible = true;
                        _icon.Data = Disconnected;
                        _icon.Foreground = Brushes.Gray;
                        ProgressRing.IsVisible = false;
                        break;
                    default:
                        _icon.IsVisible = false;
                        ProgressRing.IsVisible = true;
                        break;

                }
            }
        }
    }
}
