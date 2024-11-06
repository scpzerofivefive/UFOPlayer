using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFOPlayer.Views.ScriptVisualizer
{
    internal class ColorGraphVisualizer : Visualizer
    {
        private SKPaint white = new SKPaint
        {
            Color = new SKColor(0, 0, 0),
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


            drawSide(canvas, false);
        }

        private void drawSide(SKCanvas canvas, bool side)
        {
            float prevX = (float)Bounds.Width;
            float prevLeftAction = 0;

            byte i;
            SKPaint lpaint;
            float h1;
            SKRect lRect;

            for (int j = Actions.Count - 1; j >= 0; j--)
            {
                var action = Actions[j];
                int pow = side ? action.Left : action.Right;
                int time = action.Timestamp - StartBound;
                int end = EndBound - StartBound;
                float x = (float)(time * Bounds.Width / end);

                if (pow == prevLeftAction) { continue; }
                // Add line segments to the paths

                i = (byte)(Math.Abs(pow) * 2.56);


                lpaint = new SKPaint
                {
                    Color = pow < 0 ? new SKColor(i, 0, 0) : new SKColor(i, i, i),
                    IsAntialias = true,         // Enable anti-aliasing for smooth edges
                    Style = SKPaintStyle.Fill    // Set the paint style to fill
                };

                h1 = side ? (float)Bounds.Height : 0;
                lRect = new SKRect(prevX, h1, x, (float)Bounds.Height / 2);

                canvas.DrawRect(lRect, lpaint);

                if (action.Timestamp < StartBound)
                {
                    break;
                }
                prevX = x;
                prevLeftAction = pow;

            }

            i = (byte)(Math.Abs(prevLeftAction) * 2.56);


            lpaint = new SKPaint
            {
                Color = new SKColor(0, 0, 0),
                IsAntialias = true,         // Enable anti-aliasing for smooth edges
                Style = SKPaintStyle.Fill    // Set the paint style to fill
            };

            h1 = side ? (float)Bounds.Height : 0;
            lRect = new SKRect(prevX, h1, 0, (float)Bounds.Height / 2);

            canvas.DrawRect(lRect, lpaint);


        }




    }
}
