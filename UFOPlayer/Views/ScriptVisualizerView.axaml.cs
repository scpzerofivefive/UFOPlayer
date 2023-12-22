using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using UFOPlayer.Script;
using UFOPlayer.ViewModels;

namespace UFOPlayer.Views
{
    public partial class ScriptVisualizerView : UserControl
    {
        public static readonly StyledProperty<List<ScriptAction>> ActionsProperty =
        AvaloniaProperty.Register<ScriptVisualizerView, List<ScriptAction>>(nameof(Actions), defaultValue: new List<ScriptAction>());
        public static readonly StyledProperty<int> DurationProperty =
        AvaloniaProperty.Register<ScriptVisualizerView, int>(nameof(Duration), defaultValue: 1);
        public static readonly StyledProperty<int> ScrubberProperty =
          AvaloniaProperty.Register<ScriptVisualizerView, int>(nameof(Scrubber), defaultValue: 1);

        public int Duration
        {
            get => GetValue(DurationProperty);
            set => SetValue(DurationProperty, value);
        }
        public List<ScriptAction> Actions
        {
            get => GetValue(ActionsProperty);
            set => SetValue(ActionsProperty, value);
        }
        
        public int Scrubber
        {
            get => GetValue<int>(ScrubberProperty);
            set => SetValue(ScrubberProperty, value);
        }

        private Canvas scrubLayer;
        private Canvas actionLayer;
        public ScriptVisualizerView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Load XAML
            AvaloniaXamlLoader.Load(this);
            scrubLayer = this.FindControl<Canvas>("ScrubberLayer");
            actionLayer = this.FindControl<Canvas>("ActionLayer");
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if ( change.Property == ScrubberProperty)
            {
                
                scrubLayer.Children.Clear();
                
                
                int scrubberx = (int) (Scrubber * Width / Duration);
                scrubLayer.Children.Add(new Line
                {
                    StartPoint = new Avalonia.Point(scrubberx, 0),
                    EndPoint = new Avalonia.Point(scrubberx, Height),
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                });
            } else if (change.Property == ActionsProperty || change.Property == DurationProperty)
            {
                drawActions();
                actionLayer.Children.Add(new Line
                {
                    StartPoint = new Avalonia.Point(0, 20),
                    EndPoint = new Avalonia.Point(Width, 20),
                    Stroke = Brushes.Black,
                    StrokeThickness = .5,
                });
            }


        }

        public async void drawActions()
        {
            actionLayer.Children.Clear();
            double xo = 0;
            double lyo = Height / 2;
            foreach (ScriptAction action in Actions)
            {
                double lyf = (action.Left * (Height / 2) / 100) + (Height / 2);

                if (Math.Abs(lyf - lyo) < 1)
                {
                    continue;
                }
                double x = action.Timestamp * Width / Duration;
                
                Line hline = new Line
                {
                    StartPoint = new Avalonia.Point(xo, lyo),
                    EndPoint = new Avalonia.Point(x, lyo),
                    Stroke = Brushes.Red,
                    StrokeThickness = 1,
                };
                actionLayer.Children.Add(hline);
                Line vline = new Line
                {
                    StartPoint = new Avalonia.Point(x, lyo),
                    EndPoint = new Avalonia.Point(x, lyf),
                    Stroke = Brushes.Red,
                    StrokeThickness = .1,
                    Opacity = 1
                };
                actionLayer.Children.Add(vline);
                lyo = lyf;
                xo = x;
            }
            xo = 0;
            lyo = Height / 2;
            foreach (ScriptAction action in Actions)
            {
                double lyf = (action.Right * (Height / 2) / 100) + (Height / 2);

                if (Math.Abs(lyf - lyo) < 1)
                {
                    continue;
                }
                double x = action.Timestamp * Width / Duration;
                
                Line hline = new Line
                {
                    StartPoint = new Avalonia.Point(xo, lyo),
                    EndPoint = new Avalonia.Point(x, lyo),
                    Stroke = Brushes.Blue,
                    StrokeThickness = 1,
                };
                actionLayer.Children.Add(hline);
                Line vline = new Line
                {
                    StartPoint = new Avalonia.Point(x, lyo),
                    EndPoint = new Avalonia.Point(x, lyf),
                    Stroke = Brushes.Blue,
                    StrokeThickness = .1,
                    Opacity = 1
                };
                actionLayer.Children.Add(vline);
                lyo = lyf;
                xo = x;

            }
        }
    }
}
