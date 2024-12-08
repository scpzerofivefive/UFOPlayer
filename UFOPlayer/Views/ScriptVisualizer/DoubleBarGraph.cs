using Shared.Scripts;
using SkiaSharp;
using System;
using System.Collections.Generic;

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
            if (Actions == null)
                return;

            DrawBars(canvas, Actions.RightActions, (float)Bounds.Height / 2);
            DrawBars(canvas, Actions.LeftActions, (float)Bounds.Height);

        }

        private void DrawBars(SKCanvas canvas, LinkedList<ScriptAction> actions, float bottom)
        {
            float prevX = 0;
            float prevHeight = 0;
            foreach (ScriptAction a in actions)
            {
                float x = getXCoordAt(a.Timestamp);
                float height = (float)((a.Intensity * (Bounds.Height / 2) / 100));

                DrawSide(canvas, prevX, x, prevHeight, bottom);

                prevX = x;
                prevHeight = height;

                if (a.Timestamp > EndBound)
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
