namespace TestCase.Models
{
    public struct Segment
    {
        public bool IsMarked { get; set; }
        public Cord Start { get; set; } 
        public Cord End { get; set; }

        public Segment(Cord start, Cord end, bool isMarked = false)
        {

            if (start.X > end.X || start.Y > end.Y) (end, start) = (start, end);

            Start = start;
            End = end;

            IsVertical = start.X - end.X == 0;
            IsHorizontal = start.Y - end.Y == 0;

            if (start.X > end.X)
            {
                Xmax = start.X;
                Xmin = end.X;
            }
            else
            {
                Xmax = end.X;
                Xmin = start.X;
            }

            if (start.Y > end.Y)
            {
                Ymax = start.Y;
                Ymin = end.Y;
            }
            else
            {
                Ymax = end.Y;
                Ymin = start.Y;
            }

            //Xmax = start.X > end.X ? start.X : end.X;
            //Ymax = start.Y > end.Y ? start.Y : end.Y;
            //Xmin = start.X < end.X ? start.X : end.X;
            //Ymin = start.Y < end.Y ? start.Y : end.Y;

            IsMarked = isMarked;
        }

        public void Mark() => IsMarked = true;

        public bool IsVertical { get; }
        public bool IsHorizontal { get; }
        public float Xmax { get; }
        public float Ymax { get; }
        public float Xmin { get; }
        public float Ymin { get; }

        //public bool IsVertical => Start.X - End.X == 0;
        //public bool IsHorizontal => Start.Y - End.Y == 0;
        //public float Xmax => Start.X > End.X ? Start.X : End.X;
        //public float Ymax => Start.Y > End.Y ? Start.Y : End.Y;
        //public float Xmin => Start.X < End.X ? Start.X : End.X;
        //public float Ymin => Start.Y < End.Y ? Start.Y : End.Y;
    }
}