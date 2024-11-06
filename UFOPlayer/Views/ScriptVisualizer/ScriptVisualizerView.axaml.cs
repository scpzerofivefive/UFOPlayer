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
        public static readonly StyledProperty<List<ScriptAction>> ActionsProperty =
            AvaloniaProperty.Register<ScriptVisualizerView, List<ScriptAction>>(nameof(Actions), defaultValue: new List<ScriptAction>());
        public static readonly StyledProperty<int> DurationProperty =
            AvaloniaProperty.Register<ScriptVisualizerView, int>(nameof(Duration), defaultValue: 1);
        public static readonly StyledProperty<int> ScrubberProperty =
            AvaloniaProperty.Register<ScriptVisualizerView, int>(nameof(Scrubber), defaultValue: 1);

        public static readonly StyledProperty<VisualizerMode> ModeProperty = 
            AvaloniaProperty.Register<ScriptVisualizerView, VisualizerMode>(nameof(Mode), defaultValue: VisualizerMode.Bar);

        public float zoom
        {
            get; set;
        } = 1.0f;

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

        public VisualizerMode Mode
        {
            get => (VisualizerMode)GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
        {
            base.OnPointerWheelChanged(e);

            float minZoom = (float)((1000/80) /( Duration / Width));

            if (Actions == null || Actions.Count == 0)
            {
                return;
            }

            // Adjust zoom increment based on the current zoom level
            float zoomIncrement = 0.1f * (zoom);

            // Apply the scroll delta with the adjusted increment
            zoom -= zoomIncrement * (float)e.Delta.Y;
            zoom = Math.Clamp(zoom, minZoom, 1f);

            if (zoom < 1.0f)
            {
                int scale = (int)Math.Round(zoom * Duration / 2.0f);
                visualizer.StartBound = Scrubber - scale;
                visualizer.EndBound = Scrubber + scale;
            }
            else
            {
                visualizer.StartBound = 0;
                visualizer.EndBound = Duration;
            }

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


            if (change.Property == ScrubberProperty || change.Property == ActionsProperty || 
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
                visualizer.Actions = Actions;

                if (zoom < 1.0f)
                {
                    int scale = (int)Math.Round(zoom * Duration / 2.0f);
                    visualizer.StartBound = Scrubber - scale;
                    visualizer.EndBound = Scrubber + scale;

                }
                else
                {
                    visualizer.StartBound = 0;
                    visualizer.EndBound = Duration;
                }
                InvalidateVisual();  // Triggers OnRender
            }


        }

        

        public override void Render(DrawingContext context)
        {
            visualizer.Bounds = new Rect(0, 0, this.Bounds.Width, this.Bounds.Height);
            context.Custom(visualizer);
        }
    }
}
