using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFOPlayer.Views.ScriptVisualizer
{
    internal class DoubleBarGraph : Visualizer
    {
        private SKPaint white = new SKPaint
        {
            Color = new SKColor(70, 70, 70),
            StrokeWidth = 1,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };


        public override void Render(SKCanvas canvas)
        {
            base.Render(canvas);
            canvas.DrawLine(0, (float)Bounds.Height / 2, (float)Bounds.Width, (float)Bounds.Height / 2, white);

        }

        public override void DrawActions(SKCanvas canvas)
        {
            if (Actions == null || Actions.Count == 0)
                return;

            // Paths for the left and right channels
            using var leftChannelPath = new SKPath();
            using var rightChannelPath = new SKPath();

            float prevX = 0;
            float prevYLeft = 0;
            float prevYRight = 0;

            foreach (var action in Actions)
            {
                int time = action.Timestamp - StartBound;
                int end = EndBound - StartBound;
                float x = (float)(time * Bounds.Width / end);
                float yLeft = (float)((action.Left * (Bounds.Height / 2) / 100));
                float yRight = (float)((action.Right * (Bounds.Height / 2) / 100));

                DrawSide(canvas, prevX, x, prevYRight, (float)Bounds.Height / 2);
                DrawSide(canvas, prevX, x, prevYLeft, (float) Bounds.Height);



                prevX = x;
                prevYLeft = yLeft;
                prevYRight = yRight;

                if (action.Timestamp > EndBound)
                {
                    break;
                }

            }


        }

        private void DrawSide(SKCanvas canvas, float prevX, float x, float y, float bottom)
        {
            if (y == 0)
            {
                return;
            }
            using var paint = new SKPaint
            {
                Color = y > 0 ? SKColors.LightGray : SKColors.Red,
                IsAntialias = false,
                Style = SKPaintStyle.Fill,
                BlendMode = SKBlendMode.Plus
            };

            y = Math.Abs(y);

            SKRect rect = new SKRect(prevX, bottom, x, bottom - y);

            canvas.DrawRect(rect, paint);
        }


    }

}
