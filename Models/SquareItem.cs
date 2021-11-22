using System;
using System.Collections.Generic;

namespace TestCase.Models
{
    public class SquareItem : MapItem
    {
        //float product(Cord P, Cord A, Cord B)
        //{
        //    return (B.X - A.X) * (P.Y - A.Y) - (B.Y - A.Y) * (P.X - A.X);
        //}
        //private bool IsPointInSquare(Cord point, IEnumerable<Cord> points)
        //{
        //    var array = points.ToArray();
            
        //    var p1 = product(point, array[0], array[1]);
        //    var p2 = product(point, array[1], array[2]);
        //    var p3 = product(point, array[2], array[3]);
        //    var p4 = product(point, array[3], array[0]);
                    
        //            // по часовой, все знаки положительные, иначе отрицательные
        //    return p1 > 0 && p2 > 0 && p3 > 0 && p4 > 0 ||
        //           p1 < 0 && p2 < 0 && p3 < 0 && p4 < 0;
        //}

        public void CheckIntersectionsWithLines(IEnumerable<LineItem> lines)
        {
            foreach (var line in lines)
            {
                var sLen = Segments.Count;
                var len = line.Segments.Count;
                for (int i = 0; i < len; i++)
                {
                    var lineSegment = line.Segments[i];
                    
                    for (int j = 0; j < sLen; j++)
                    {
                        var squareSegment = Segments[j];

                        // проверяем сегменты (отрезки)
                        if (!IsSegmentsIntersect(lineSegment, squareSegment)) continue;

                        // помечаем сегмент, который соприкасается с квадратом
                        line.Segments[i] = new Segment(lineSegment.Start, lineSegment.End, true);
                    }
                }
            }
        }
        public bool IsSegmentsIntersect(Segment A, Segment B)
        {
            return IsSegmentsIntersect(A.Start, A.End, B.Start, B.End,A, B);
            return IsSegmentsIntersect(A.Start, A.End, B.Start, B.End);
        }
        
        private bool IsSegmentsIntersect(Cord p1, Cord p2, Cord p3, Cord p4, Segment A, Segment B)
        {

            #region Проверка на потенциальный интервал для точки пересечения отрезков
            
            if (A.End.X < B.Start.X) return false;

            #endregion
            
            #region Если оба вертикальные
            
            if (A.IsVertical && B.IsVertical)
            {
                //если они лежат на одном X
                if (A.Start.X.Equals(A.End.X))
                    //проверим есть ли у них общий Y
                    if (!(A.Ymax < B.Ymin || A.Ymin > B.Ymin))
                        return true;
                return false;
            }
            #endregion

            //найдём коэффициенты уравнений, содержащих отрезки
            //f1(x) = A1*x + b1 = y
            //f2(x) = A2*x + b2 = y
            
            #region Если первый отрезок вертикальный
            if (A.IsVertical)
            {
                //найдём Xa, Ya - точки пересечения двух прямых
                float Xa = p1.X;
                float A2 = (p3.Y - p4.Y) / (p3.X - p4.X);
                float b2 = p3.Y - A2 * p3.X;
                float Ya = A2 * Xa + b2;

                return p3.X <= Xa && p4.X >= Xa && Math.Min(p1.Y, p2.Y) <= Ya && Math.Max(p1.Y, p2.Y) >= Ya;
            }
            #endregion
            
            #region Если второй отрезок вертикальный
            if (B.IsVertical)
            {
                //найдём Xa, Ya - точки пересечения двух прямых
                double Xa = p3.X;
                double A1 = (p1.Y - p2.Y) / (p1.X - p2.X);
                double b1 = p1.Y - A1 * p1.X;
                double Ya = A1 * Xa + b1;

                return p1.X <= Xa && p2.X >= Xa && B.Ymin <= Ya && B.Ymin >= Ya;
            }
            #endregion

            #region Если оба не вертикальные
            else
            {
                float A1 = (p1.Y - p2.Y) / (p1.X - p2.X);
                float A2 = (p3.Y - p4.Y) / (p3.X - p4.X);
                float b1 = p1.Y - A1 * p1.X;
                float b2 = p3.Y - A2 * p3.X;

                if (A1.Equals(A2)) return false; //отрезки параллельны

                //Xa - абсцисса точки пересечения двух прямых
                double Xa = (b2 - b1) / (A1 - A2);

                return !(Xa <= Math.Max(p1.X, p3.X)) && !(Xa >= Math.Min(p2.X, p4.X));
            } 
            #endregion
        }
        private bool IsSegmentsIntersect(Cord p1, Cord p2, Cord p3, Cord p4)
        {

            #region Проверка на потенциальный интервал для точки пересечения отрезков
            
            if (p2.X < p3.X) return false;

            #endregion
            
            #region Если оба вертикальные
            
            if (p1.X - p2.X == 0 && p3.X - p4.X == 0)
            {
                //если они лежат на одном X
                if (p1.X.Equals(p3.X))
                    //проверим есть ли у них общий Y
                    if (!(Math.Max(p1.Y, p2.Y) < Math.Min(p3.Y, p4.Y) ||
                          Math.Min(p1.Y, p2.Y) > Math.Max(p3.Y, p4.Y)))
                        return true;
                return false;
            }
            #endregion

            //найдём коэффициенты уравнений, содержащих отрезки
            //f1(x) = A1*x + b1 = y
            //f2(x) = A2*x + b2 = y
            
            #region Если первый отрезок вертикальный
            else if (p1.X - p2.X == 0)
            {
                //найдём Xa, Ya - точки пересечения двух прямых
                float Xa = p1.X;
                float A2 = (p3.Y - p4.Y) / (p3.X - p4.X);
                float b2 = p3.Y - A2 * p3.X;
                float Ya = A2 * Xa + b2;

                return p3.X <= Xa && p4.X >= Xa && Math.Min(p1.Y, p2.Y) <= Ya && Math.Max(p1.Y, p2.Y) >= Ya;
            }
            #endregion
            
            #region Если второй отрезок вертикальный
            else if (p3.X - p4.X == 0)
            {
                //найдём Xa, Ya - точки пересечения двух прямых
                double Xa = p3.X;
                double A1 = (p1.Y - p2.Y) / (p1.X - p2.X);
                double b1 = p1.Y - A1 * p1.X;
                double Ya = A1 * Xa + b1;

                return p1.X <= Xa && p2.X >= Xa && Math.Min(p3.Y, p4.Y) <= Ya && Math.Max(p3.Y, p4.Y) >= Ya;
            }
            #endregion

            #region Если оба не вертикальные
            else
            {
                float A1 = (p1.Y - p2.Y) / (p1.X - p2.X);
                float A2 = (p3.Y - p4.Y) / (p3.X - p4.X);
                float b1 = p1.Y - A1 * p1.X;
                float b2 = p3.Y - A2 * p3.X;

                if (A1.Equals(A2)) return false; //отрезки параллельны

                //Xa - абсцисса точки пересечения двух прямых
                double Xa = (b2 - b1) / (A1 - A2);

                return !(Xa <= Math.Max(p1.X, p3.X)) && !(Xa >= Math.Min(p2.X, p4.X));
            } 
            #endregion
        }
    }
}