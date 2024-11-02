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

namespace UFOPlayer.Views.ScriptVisualizer
{
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

            canvas.DrawLine(scrubberX, 0, scrubberX, (float)Bounds.Height, white);
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

        void drawSecondsDivider(SKCanvas canvas)
        {
            var paint = new SKPaint
            {
                Color = SKColors.Gray,
                StrokeWidth = 1,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };

            paint.PathEffect = SKPathEffect.CreateDash(new float[] { 1, 2 }, 0);

            int span = EndBound - StartBound;
            float numDividers = (float)span / 1000f;

            float interval = (float)Bounds.Width / numDividers;
            if (interval < 7)
            {
                return;
            }

            float x = (float)((1000 - (StartBound % 1000)) * Bounds.Width / span);
            for (int i = 0; i < numDividers; i++)
            {
                canvas.DrawLine(x, 0, x, (float)Bounds.Height, paint);
                x += interval;
            }
        }


        void ICustomDrawOperation.Render(ImmediateDrawingContext context)
        {
            var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
            if (leaseFeature != null)
            {
                using var lease = leaseFeature.Lease();
                var canvas = lease.SkCanvas;
                canvas.Clear(new SKColor(15, 15, 15));
                drawSecondsDivider(canvas);
                for (int i = 0; i < 10; i++)
                {
                    canvas.DrawLine(0, i * (float)Bounds.Height / 10, (float)Bounds.Width, i * (float)Bounds.Height / 10, gray);
                }

                Render(canvas);
                canvas.DrawLine(0, (float)Bounds.Height / 2, (float)Bounds.Width, (float)Bounds.Height / 2, white);


            }
        }
    }
}
