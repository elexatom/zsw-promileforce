using Microsoft.Maui.Graphics;
using RangerFinder.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace RangerFinder
{
    public class SensorChartDrawable : IDrawable
    {
        public ObservableCollection<SensorData>? History { get; set; }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (History == null || History.Count < 2)
            {
                // Draw a placeholder background so it's not totally empty
                canvas.FillColor = Color.FromArgb("#fafffa");
                canvas.FillRectangle(dirtyRect);
                return;
            }

            var data = History.ToList(); // Snapshot for rendering
            
            float width = dirtyRect.Width;
            float height = dirtyRect.Height;
            float stepX = width / Math.Max(1, data.Count - 1);
            
            // Dynamically scale chart height based on max distance
            double maxValue = 2.0; // Start with minimum scale 2m
            foreach(var d in data) 
            {
                 maxValue = Math.Max(maxValue, Math.Max(d.Ultrasonic1, Math.Max(d.Ultrasonic2, Math.Max(d.Lidar1, d.Lidar2))));
            }
            
            // Add a small padding to the max value so lines don't hit the very top edge
            maxValue *= 1.1;

            canvas.FillColor = Colors.White;
            canvas.FillRectangle(dirtyRect);

            // Draw grid lines
            canvas.StrokeColor = Colors.LightGray;
            canvas.StrokeSize = 1;
            canvas.StrokeDashPattern = new float[] { 5, 5 };
            for(int i = 1; i <= 4; i++)
            {
                float yAxis = height - (float)(maxValue / 4 * i / maxValue * height);
                canvas.DrawLine(0, yAxis, width, yAxis);
            }
            canvas.StrokeDashPattern = null;

            void DrawSensorLine(Func<SensorData, double> selector, Color color)
            {
                var path = new PathF();
                for (int i = 0; i < data.Count; i++)
                {
                    float x = i * stepX;
                    float y = height - (float)(selector(data[i]) / maxValue * height);
                    if (y < 0) y = 0;
                    if (y > height) y = height;

                    if (i == 0)
                        path.MoveTo(x, y);
                    else
                        path.LineTo(x, y);
                }
                
                canvas.StrokeColor = color;
                canvas.StrokeSize = 3;
                canvas.DrawPath(path);
            }

            // Draw each sensor
            DrawSensorLine(d => d.Ultrasonic1, Colors.Red);
            DrawSensorLine(d => d.Ultrasonic2, Colors.Blue);
            DrawSensorLine(d => d.Lidar1, Colors.Green);
            DrawSensorLine(d => d.Lidar2, Colors.Orange);
        }
    }
}
