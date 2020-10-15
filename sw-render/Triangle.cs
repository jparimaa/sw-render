using System.Numerics;

class Triangle
{
    public Vector3[] Positions { get; set; }
    public Vector3[] Normals { get; set; }
    public Vector2[] UVs { get; set; }

    public Triangle()
    {
        Positions = new Vector3[3];
        Normals = new Vector3[3];
        UVs = new Vector2[3];
    }
}
