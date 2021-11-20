using System.Collections.Generic;
using System.Drawing;

namespace TestCase.Models
{
    public class Map
    {
        public float Xmax { get; set; }
        public float Xmin { get; set; }
        public float Ymax { get; set; }
        public float Ymin { get; set; }

        public double Width { get; set; }
        public double Height { get; set; }
        public List<MapItem> Items { get; set; } = new();
        
        public void CalculateExtremes(float X, float Y)
        {
            Xmax = X > Xmax ? X : Xmax;
            Ymax = X > Ymax ? X : Ymax;
            Xmin = X < Xmin ? X : Xmin;
            Ymin = Y < Ymin ? Y : Ymin;
        }
        public void CalculateExtremes(PointF point)
        {
            Xmax = point.X > Xmax ? point.X : Xmax;
            Ymax = point.X > Ymax ? point.X : Ymax;
            Xmin = point.X < Xmin ? point.X : Xmin;
            Ymin = point.Y < Ymin ? point.Y : Ymin;
        }

        public PointF ScaledRatio { get; set; } = new (1, 1);
        public PointF GetScaledRatio()
        {
            double ratioScaleX = Width / (Xmax - Xmin);
            double ratioScaleY = Height / (Ymax - Ymin);
            ratioScaleX = ratioScaleX < 0 ? 1 : ratioScaleX;
            ratioScaleY = ratioScaleY < 0 ? 1 : ratioScaleY;
            var p = new PointF((float)ratioScaleX, (float)ratioScaleY);
            ScaledRatio = p;
            return p;
        }
        public double GetScaledRatioDouble()
        {
            double ratioScaleX = Height / (Xmax - Xmin);
            double ratioScaleY = Width / (Ymax - Ymin);
            ratioScaleX = ratioScaleX < 0 ? 1 : ratioScaleX;
            ratioScaleY = ratioScaleY < 0 ? 1 : ratioScaleY;

            return ratioScaleY > ratioScaleX ? ratioScaleX*.6 : ratioScaleY*.6;
        }
    }
}
