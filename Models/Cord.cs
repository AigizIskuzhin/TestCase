namespace TestCase.Models
{
    public class Cord
    {
        public float X { get; set; }
        public float Y { get; set; }
        public bool IsMarked { get; set; }
        public Cord(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}