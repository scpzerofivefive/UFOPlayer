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
    class LineGraphVisualizer : Visualizer
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

            using var redPaint = new SKPaint
            {
                Color = SKColors.Red,
                StrokeWidth = 1,
                IsAntialias = false,
                Style = SKPaintStyle.Stroke,
                BlendMode = SKBlendMode.Lighten
            };

            using var bluePaint = new SKPaint
            {
                Color = new SKColor(47, 117, 245),
                StrokeWidth = 1,
                IsAntialias = false,
                Style = SKPaintStyle.Stroke,
                BlendMode = SKBlendMode.Lighten
            };

            drawPath(canvas, redPaint, Actions.RightActions);
            drawPath(canvas, bluePaint, Actions.LeftActions);
        }

        private void drawPath(SKCanvas canvas, SKPaint paint, LinkedList<ScriptAction> list)
        {
            // Paths for the left and right channels
            using var channelPath = new SKPath();

            float prevX = 0;
            float prevYLeft = (float)(Bounds.Height / 2);

            // Move to starting points in the paths
            channelPath.MoveTo(0, prevYLeft);


            foreach (ScriptAction action in list)
            {
                float x = getXCoordAt(action.Timestamp);
                float y = (float)(Bounds.Height - ((action.Intensity * (Bounds.Height / 2) / 100) + (Bounds.Height / 2)));

                // Add line segments to the paths
                channelPath.LineTo(x, prevYLeft);
                channelPath.LineTo(x, y);

                prevX = x;
                prevYLeft = y;

                if (action.Timestamp > EndBound)
                {
                    break;
                }

            }

            channelPath.LineTo((float)Bounds.Width, (float)(Bounds.Height / 2));
            canvas.DrawPath(channelPath, paint);
        }

       
    }
}
