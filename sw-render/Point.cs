using System.Numerics;

class Point
{
    public int X { get; set; }
    public int Y { get; set; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Point(float x, float y)
    {
        X = (int)x;
        Y = (int)y;
    }

    public Point(Vector2 v)
    {
        X = (int)v.X;
        Y = (int)v.Y;
    }

    public Vector2 ToVector2()
    {
        return new Vector2(X, Y);
    }
}
