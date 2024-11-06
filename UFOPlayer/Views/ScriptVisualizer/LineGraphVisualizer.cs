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
            if (Actions == null || Actions.Count == 0)
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
                float yLeft = (float)(Bounds.Height - ((action.Left * (Bounds.Height / 2) / 100) + (Bounds.Height / 2)));
                float yRight = (float)(Bounds.Height - ((action.Right * (Bounds.Height / 2) / 100) + (Bounds.Height / 2)));
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

       
    }
}
