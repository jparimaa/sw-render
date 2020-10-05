using System.Collections.Generic;
using System.Numerics;

class MyMath
{
    public static Vector3 GetBarycentric(List<Vector2> points, Vector2 p)
    {
        System.Diagnostics.Debug.Assert(points.Count == 3);
        Vector3 a = new Vector3(points[2].X - points[0].X, points[1].X - points[0].X, points[0].X - p.X);
        Vector3 b = new Vector3(points[2].Y - points[0].Y, points[1].Y - points[0].Y, points[0].Y - p.Y);
        Vector3 u = Vector3.Cross(a, b);
        return new Vector3(1.0f - (u.X + u.Y) / u.Z, u.Y / u.Z, u.X / u.Z);
    }

    public static Vector4 GetBoundingBox(List<Vector2> points, Vector2 min, Vector2 max)
    {
        Vector4 boundingBox = new Vector4() { X = max.X, Y = max.Y, Z = min.X, W = min.Y };
        foreach (Vector2 p in points)
        {
            boundingBox.X = System.Math.Min(boundingBox.X, p.X);
            boundingBox.Y = System.Math.Min(boundingBox.Y, p.Y);
            boundingBox.Z = System.Math.Max(boundingBox.Z, p.X);
            boundingBox.W = System.Math.Max(boundingBox.W, p.X);
        }
        boundingBox.X = System.Math.Max(boundingBox.X, min.X);
        boundingBox.Y = System.Math.Max(boundingBox.Y, min.Y);
        boundingBox.Z = System.Math.Min(boundingBox.Z, max.X);
        boundingBox.W = System.Math.Min(boundingBox.W, max.Y);
        return boundingBox;
    }
}
