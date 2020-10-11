using System.Numerics;

class Triangle
{
    public float[] Depths { get; set; }
    public Point[] ScreenPositions { get; set; }
    public Vector3[] Normals { get; set; }

    public Triangle()
    {
        Depths = new float[3];
        ScreenPositions = new Point[3];
        Normals = new Vector3[3];
    }
}
