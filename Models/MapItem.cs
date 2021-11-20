using System.Collections.Generic;

namespace TestCase.Models
{
    public abstract class MapItem
    {
        public List<Cord> Points { get; set; } = new();
        public List<Segment> Segments { get; set; } = new();
        public void BuildSegments()
        {
            var len = Points.Count;
            for (int i = 0; i < len; i++)
            {
                var currentPoint = Points[i];
                var nextPoint = i < len - 1 ? Points[i + 1] : Points[0];
                Segments.Add(new Segment(currentPoint,nextPoint));
                if(len==2)
                    break;
            }
        }
    }
}