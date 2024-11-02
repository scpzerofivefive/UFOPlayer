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
using Avalonia.Input;

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

        protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
        {
            base.OnPointerWheelChanged(e);
            // Change Scrubber based on scroll delta

            if (Actions == null || Actions.Count == 0)
            {
                return;
            }

            if (e.Delta.Y != 0)
            {
                zoom = zoom + (.025f * (float) e.Delta.Y);
            }

            zoom = Math.Max(.025f, Math.Min(zoom, 1f));


            if (zoom < 1.0f)
            {
                int scale = (int) Math.Round(zoom * Duration / 2.0f);
                visualizer.StartBound = Scrubber - scale;
                visualizer.EndBound = Scrubber + scale;

            } else
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

        class Visualizer : ICustomDrawOperation
        {
            private SKPaint black { get; } = new SKPaint
            {
                Color = SKColors.Black,
                StrokeWidth = 1,
                IsAntialias = true
            };

            private SKPaint gray { get; } = new SKPaint
            {
                Color = new SKColor(49, 49, 49),
                StrokeWidth = .5f,
                IsAntialias = true
            };

            private SKPaint white = new SKPaint
            {
                Color = new SKColor(70, 70, 70),
                StrokeWidth = 1,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };

            public Rect Bounds { get; set; }

            public int Position { get; set; } = 0;

            public int StartBound { get; set; } = 0;

            public int EndBound { get; set; } = 0;




            public List<ScriptAction> Actions { get; set; } = new List<ScriptAction>();

            public void Dispose() { }

            public bool Equals(ICustomDrawOperation? other) => other == this;

            // not sure what goes here....
            public bool HitTest(Point p) { return true; }

            private void Render(SKCanvas canvas)
            {

                if (Position != 0 && EndBound != 0)
                {
                    DrawScrubber(canvas);
                }

                DrawActions(canvas);

            }
            private void DrawScrubber(SKCanvas canvas)
            {
                int pos = Position - StartBound;
                int end = EndBound - StartBound;
                float scrubberX = (float)(pos * Bounds.Width / end);

                canvas.DrawLine(scrubberX, 0, scrubberX, (float)Bounds.Height, black);
            }

            private void DrawActions(SKCanvas canvas)
            {
                if (Actions == null || Actions.Count == 0)
                    return;

                using var redPaint = new SKPaint
                {
                    Color = SKColors.Red,
                    StrokeWidth = 1,
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    BlendMode = SKBlendMode.Plus
                };

                using var bluePaint = new SKPaint
                {
                    Color = new SKColor(47, 117, 245),
                    StrokeWidth = 1,
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    BlendMode = SKBlendMode.Plus
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
                    int time = action.Timestamp - StartBound;
                    int end = EndBound - StartBound;
                    float x = (float)(time * Bounds.Width / end);
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

                    if (action.Timestamp > EndBound)
                    {
                        break;
                    }

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
                    canvas.Clear(new SKColor(32, 32, 32));
                    for (int i = 0; i < 10; i++)
                    {
                        canvas.DrawLine(0, i * (float)Bounds.Height / 10, (float)Bounds.Width, i * (float)Bounds.Height / 10, gray);
                    }
                    Render(canvas);
                    canvas.DrawLine(0, (float)Bounds.Height / 2, (float)Bounds.Width, (float)Bounds.Height / 2, white);

                    
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
