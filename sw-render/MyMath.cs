using System.Collections.Generic;
using System.Numerics;

class MyMath
{
    public static Vector3 GetBarycentric(List<Point> points, Vector2 p)
    {
        System.Diagnostics.Debug.Assert(points.Count == 3);
        Vector3 a = new Vector3(points[2].X - points[0].X, points[1].X - points[0].X, points[0].X - p.X);
        Vector3 b = new Vector3(points[2].Y - points[0].Y, points[1].Y - points[0].Y, points[0].Y - p.Y);
        Vector3 u = Vector3.Cross(a, b);
        if (System.Math.Abs(u.Z) < 1.0f) // Degenrate
        {
            return new Vector3(-1, -1, -1);
        }

        return new Vector3(1.0f - (u.X + u.Y) / u.Z, u.Y / u.Z, u.X / u.Z);
    }
}
