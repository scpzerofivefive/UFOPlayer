using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using SkiaSharp;
using Avalonia.Skia;
using System;
using System.Collections.Generic;
using UFOPlayer.Script;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using System.Diagnostics;

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


        Visualizer visualizer;

        public ScriptVisualizerView()
        {
            InitializeComponent();

            visualizer = new Visualizer();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);


            if (change.Property == ScrubberProperty || change.Property == ActionsProperty || change.Property == DurationProperty)
            {
                visualizer.Scrubber = Scrubber;
                visualizer.EndBound = Duration;
                visualizer.Actions = Actions;
                InvalidateVisual();  // Triggers OnRender
            }


        }

        class Visualizer : ICustomDrawOperation
        {
            private SKPaint black { get; } = new SKPaint
            {
                Color = SKColors.Black,
                StrokeWidth = 1,
                IsAntialias = true
            };

            public Rect Bounds { get; set; }

            public int Scrubber { get; set; } = 0;

            public int StartBound { get; set; } = 0;

            public int EndBound { get; set; } = 0;

            public List<ScriptAction> Actions { get; set; } = new List<ScriptAction>();

            public void Dispose() { }

            public bool Equals(ICustomDrawOperation? other) => other == this;

            // not sure what goes here....
            public bool HitTest(Point p) { return false; }

            private void Render(SKCanvas canvas)
            {

                if (Scrubber != 0 && EndBound != 0)
                {
                    DrawScrubber(canvas);
                }

                if (Actions.Count != 0)
                {
                    DrawActions(canvas);
                }
                
            }
            private void DrawScrubber(SKCanvas canvas)
            {
                float scrubberX = (float)(Scrubber * Bounds.Width / EndBound);

                canvas.DrawLine(scrubberX, 0, scrubberX, (float)Bounds.Height, black);
            }

            private void DrawActions(SKCanvas canvas)
            {
                if (Actions == null || Actions.Count == 0)
                    return;

                using var redPaint = new SKPaint
                {
                    Color = SKColors.Red.WithAlpha(128),
                    StrokeWidth = 1,
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke
                };

                using var bluePaint = new SKPaint
                {
                    Color = SKColors.Blue.WithAlpha(128),
                    StrokeWidth = 1,
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke
                };

                // Paths for the left and right channels
                using var leftChannelPath = new SKPath();
                using var rightChannelPath = new SKPath();

                float prevX = 0;
                float prevYLeft = (float)(Bounds.Height / 2);
                float prevYRight = (float)(Bounds.Height / 2);

                // Move to starting points in the paths
                leftChannelPath.MoveTo(0, prevYLeft);
                rightChannelPath.MoveTo(0, prevYRight);
                foreach (var action in Actions)
                {
                    float x = (float)(action.Timestamp * Bounds.Width / EndBound);
                    float yLeft = (float)((action.Left * (Bounds.Height / 2) / 100) + (Bounds.Height / 2));
                    float yRight = (float)((action.Right * (Bounds.Height / 2) / 100) + (Bounds.Height / 2));

                    

                    // Add line segments to the paths
                    leftChannelPath.LineTo(x, prevYLeft);
                    leftChannelPath.LineTo(x, yLeft);

                    rightChannelPath.LineTo(x, prevYRight);
                    rightChannelPath.LineTo(x, yRight);

                    prevX = x;
                    prevYLeft = yLeft;
                    prevYRight = yRight;
                }

                leftChannelPath.LineTo((float)Bounds.Width, (float)(Bounds.Height / 2));

                rightChannelPath.LineTo((float)Bounds.Width, (float)(Bounds.Height / 2));

                // Draw the entire paths as continuous lines
                canvas.DrawPath(leftChannelPath, redPaint);
                canvas.DrawPath(rightChannelPath, bluePaint);
            }
            

            void ICustomDrawOperation.Render(ImmediateDrawingContext context)
            {
                var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
                if (leaseFeature != null)
                {
                    using var lease = leaseFeature.Lease();
                    var canvas = lease.SkCanvas;
                    canvas.Clear(SKColors.White);
                    Render(canvas);
                    canvas.DrawLine(0, (float)Bounds.Height / 2, (float)Bounds.Width, (float)Bounds.Height / 2, black);
                }
            }
        }

        public override void Render(DrawingContext context)
        {
            visualizer.Bounds = new Rect(0, 0, this.Bounds.Width, this.Bounds.Height);
            

            context.Custom(visualizer);

        }
    }
}
