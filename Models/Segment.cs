namespace TestCase.Models
{
    public readonly struct Segment
    {
        public bool IsMarked { get; }
        public Cord Start { get; }
        public Cord End { get; }

        public Segment(Cord start, Cord end, bool isMarked = false)
        {
            // сортируем и получаем ориентированный сегмент Start -> End
            if (start.X > end.X || start.Y > end.Y) (end, start) = (start, end);

            Start = start;
            End = end;
            
            // Т.к. у нас нету задачи изменять каким-либо образом входные данные, мы заполняем их при создании
            IsVertical = start.X - end.X == 0;
            IsHorizontal = start.Y - end.Y == 0;

            // выглядит страшно, но в итоге получаем меньше инструкций :D
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

            IsMarked = isMarked;
        }

        public bool IsVertical { get; }
        public bool IsHorizontal { get; }
        public float Xmax { get; }
        public float Ymax { get; }
        public float Xmin { get; }
        public float Ymin { get; }
    }
}