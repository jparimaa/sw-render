
using System.Collections.Generic;

class BoundingBox
{
    public int Left { get; set; }
    public int Top { get; set; }
    public int Right { get; set; }
    public int Bottom { get; set; }

    public BoundingBox(List<Point> points, Point min, Point max)
    {
        Left = max.X;
        Top = max.Y;
        Right = min.X;
        Bottom = min.Y;

        foreach (Point p in points)
        {
            Left = System.Math.Min(Left, p.X);
            Top = System.Math.Min(Top, p.Y);
            Right = System.Math.Max(Right, p.X);
            Bottom = System.Math.Max(Bottom, p.Y);
        }
        Left = System.Math.Max(Left, min.X);
        Top = System.Math.Max(Top, min.Y);
        Right = System.Math.Min(Right, max.X);
        Bottom = System.Math.Min(Bottom, max.Y);
    }

}
