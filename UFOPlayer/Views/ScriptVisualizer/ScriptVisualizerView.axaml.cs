using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using SkiaSharp;
using Avalonia.Skia;
using System;
using System.Collections.Generic;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using System.Diagnostics;
using Avalonia.Input;
using UFOPlayer.Views.ScriptVisualizer;
using UFOPlayer.Scripts;

namespace UFOPlayer.Views
{
    public partial class ScriptVisualizerView : UserControl
    {
        public static readonly StyledProperty<UFOScript> ScriptProperty =
            AvaloniaProperty.Register<ScriptVisualizerView,UFOScript>(nameof(Script), defaultValue: new UFOScript());
        public static readonly StyledProperty<TimeSpan> DurationProperty =
            AvaloniaProperty.Register<ScriptVisualizerView, TimeSpan>(nameof(Duration), defaultValue: new TimeSpan(0));
        public static readonly StyledProperty<TimeSpan> ScrubberProperty =
            AvaloniaProperty.Register<ScriptVisualizerView, TimeSpan>(nameof(Scrubber), defaultValue: new TimeSpan(0));

        public static readonly StyledProperty<VisualizerMode> ModeProperty = 
            AvaloniaProperty.Register<ScriptVisualizerView, VisualizerMode>(nameof(Mode), defaultValue: VisualizerMode.Bar);

        public float zoom
        {
            get; set;
        } = 1.0f;

        public UFOScript Script
        {
            get => GetValue(ScriptProperty);
            set => SetValue(ScriptProperty, value);
        }

        public TimeSpan Duration
        {
            get => GetValue(DurationProperty);
            set => SetValue(DurationProperty, value);
        }
        

        public TimeSpan Scrubber
        {
            get => GetValue(ScrubberProperty);
            set => SetValue(ScrubberProperty, value);
        }

        public VisualizerMode Mode
        {
            get => (VisualizerMode)GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
        {
            base.OnPointerWheelChanged(e);

            float minZoom = (float)((1000/80) /( Duration.TotalMilliseconds / Width));

            if (Script == null || Duration.TotalMilliseconds == 0)
            {
                return;
            }

            // Adjust zoom increment based on the current zoom level
            float zoomIncrement = 0.1f * (zoom);

            // Apply the scroll delta with the adjusted increment
            zoom -= zoomIncrement * (float)e.Delta.Y;
            zoom = Math.Clamp(zoom, minZoom, 1f);

            setScaledBounds();

            InvalidateVisual();
        }



        Visualizer visualizer;

        public ScriptVisualizerView()
        {
            InitializeComponent();

            if (Mode == VisualizerMode.Line)
            {
                visualizer = new LineGraphVisualizer();
            }
            else
            {
                visualizer = new DoubleBarGraph();
            }

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);


            if (change.Property == ScrubberProperty || change.Property == ScriptProperty || 
                change.Property == DurationProperty || change.Property == ModeProperty)
            {
                if (change.Property == ModeProperty)
                {
                    if (Mode == VisualizerMode.Line)
                    {
                        visualizer = new LineGraphVisualizer();
                    }
                    else
                    {
                        visualizer = new DoubleBarGraph();
                    }
                }
                visualizer.Position = Scrubber;
                visualizer.EndBound = Duration;
                visualizer.Actions = Script;

                setScaledBounds();
                InvalidateVisual();  // Triggers OnRender
            }


        }

        private void setScaledBounds()
        {
            if (zoom < 1.0f)
            {
                int scale = (int)Math.Round(zoom * Duration.TotalMilliseconds / 2.0f);
                visualizer.StartBound = TimeSpan.FromMilliseconds(Scrubber.TotalMilliseconds - scale);
                visualizer.EndBound = TimeSpan.FromMilliseconds(Scrubber.TotalMilliseconds + scale);
            }
            else
            {
                visualizer.StartBound = new TimeSpan(0);
                visualizer.EndBound = Duration;
            }
        }

        

        public override void Render(DrawingContext context)
        {
            visualizer.Bounds = new Rect(0, 0, this.Bounds.Width, this.Bounds.Height);
            context.Custom(visualizer);
        }
    }
}
