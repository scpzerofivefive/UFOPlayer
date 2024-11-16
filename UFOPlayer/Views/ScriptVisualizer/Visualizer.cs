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
using UFOPlayer.Scripts;

namespace UFOPlayer.Views.ScriptVisualizer
{
    public class Visualizer : ICustomDrawOperation
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

        public TimeSpan Position { get; set; } = new TimeSpan(0);

        public TimeSpan StartBound { get; set; } = new TimeSpan(0);

        public TimeSpan EndBound { get; set; } = new TimeSpan(0);

        public float getXCoordAt(TimeSpan time)
        {
            double t = time.TotalMilliseconds - StartBound.TotalMilliseconds;
            double end = EndBound.TotalMilliseconds - StartBound.TotalMilliseconds;
            float x = (float)(t * Bounds.Width / end);
            return x;
        }

        /* TODO
        public TimeSpan getTimeAt(float x)
        {

        }*/




        public UFOScript Actions { get; set; } = new UFOScript();

        public void Dispose() { }

        public bool Equals(ICustomDrawOperation? other) => other == this;

        public bool HitTest(Point p) { return true; }

        public virtual void Render(SKCanvas canvas)
        {
            DrawBackground(canvas);
            if (Position.TotalMilliseconds != 0 && EndBound.TotalMilliseconds != 0)
            {
                DrawScrubber(canvas);
            }

            DrawActions(canvas);

        }
        private void DrawScrubber(SKCanvas canvas)
        {
            float scrubberX = getXCoordAt(Position);

            canvas.DrawLine(scrubberX, 0, scrubberX, (float)Bounds.Height, white);
        }

        public virtual void DrawActions(SKCanvas canvas)
        {
            
        }

        public virtual void DrawBackground(SKCanvas canvas)
        {
            canvas.Clear(new SKColor(15, 15, 15));
            drawSecondsDivider(canvas);
            for (int i = 0; i < 10; i++)
            {
                canvas.DrawLine(0, i * (float)Bounds.Height / 10, (float)Bounds.Width, i * (float)Bounds.Height / 10, gray);
            }
        }
        public void drawSecondsDivider(SKCanvas canvas)
        {
            var paint = new SKPaint
            {
                Color = SKColors.Gray,
                StrokeWidth = 1,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };

            paint.PathEffect = SKPathEffect.CreateDash(new float[] { 1, 2 }, 0);

            double span = EndBound.TotalMilliseconds - StartBound.TotalMilliseconds;
            float numDividers = (float)span / 1000f;

            float interval = (float)Bounds.Width / numDividers;
            if (interval < 7)
            {
                return;
            }

            float x = (float)((1000 - (StartBound.TotalMilliseconds % 1000)) * Bounds.Width / span);
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
                

                Render(canvas);

                //TODO
                canvas.DrawLine(0, (float)Bounds.Height / 2, (float)Bounds.Width, (float)Bounds.Height / 2, white);


            }
        }
    }
}
