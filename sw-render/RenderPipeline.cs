using SDL2;
using System.Collections.Generic;
using System.Numerics;

class RenderPipeline
{
    private readonly System.IntPtr c_renderer;
    private readonly int c_width;
    private readonly int c_height;
    private readonly Point c_boundingBoxMin;
    private readonly Point c_boundingBoxMax;

    private readonly Matrix4x4 c_worldMatrix;
    private readonly Matrix4x4 c_viewMatrix;
    private readonly Matrix4x4 c_projectionMatrix;

    private List<List<float>> m_depthBuffer;

    class VertexOut
    {
        public Vector4[] Positions { get; set; }
        public Vector3[] Normals { get; set; }
        public Vector2[] UVs { get; set; }

        public VertexOut()
        {
            Positions = new Vector4[3];
            Normals = new Vector3[3];
            UVs = new Vector2[3];
        }
    }

    class RasterOut
    {
        public Point[] ScreenPositions { get; set; }
        public Vector3[] Normals { get; set; }
        public Vector2[] UVs { get; set; }
        public float[] Depths { get; set; }

        public RasterOut()
        {
            ScreenPositions = new Point[3];
            Normals = new Vector3[3];
            UVs = new Vector2[3];
            Depths = new float[3];
        }
    }

    public RenderPipeline(System.IntPtr renderer, int width, int height)
    {
        c_renderer = renderer;
        c_width = width;
        c_height = height;
        c_boundingBoxMin = new Point(0, 0);
        c_boundingBoxMax = new Point(c_width - 1, c_height - 1);

        c_worldMatrix = Matrix4x4.Identity;
        c_viewMatrix = Matrix4x4.CreateLookAt(new Vector3(0.0f, 0.0f, 10.0f), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f));
        c_projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(0.785398f, width / height, 0.1f, 1000.0f);

        var defaults = new float[width];
        for (int i = 0; i < defaults.Length; ++i)
        {
            defaults[i] = -1.0f;
        }

        m_depthBuffer = new List<List<float>>(height);
        for (int i = 0; i < height; ++i)
        {
            m_depthBuffer.Add(new List<float>(defaults));
        }
    }

    public void DrawNoise(int amount = 10000)
    {
        var random = new System.Random();

        for (int i = 0; i < amount; i++)
        {
            int x = random.Next(c_width);
            int y = random.Next(c_height);

            var color = new byte[3];
            random.NextBytes(color);

            SetPixel(x, y, color[0], color[1], color[2]);
        }
    }

    public void DrawTriangle(Triangle triangle)
    {
        VertexOut vertexOut = VertexShader(triangle);
        RasterOut rasterOut = Rasterizer(vertexOut);
        PixelShader(rasterOut);
    }

    private VertexOut VertexShader(Triangle triangle)
    {
        var vertexOut = new VertexOut();
        Matrix4x4 wvp = c_worldMatrix * c_viewMatrix * c_projectionMatrix;
        for (int i = 0; i < 3; ++i)
        {
            var pos = new Vector4(triangle.Positions[i].X, triangle.Positions[i].Y, triangle.Positions[i].Z, 1.0f);
            vertexOut.Positions[i] = Vector4.Transform(pos, wvp);

            var n = new Vector4(triangle.Normals[i].X, triangle.Normals[i].Y, triangle.Normals[i].Z, 0.0f);
            n = Vector4.Transform(n, c_worldMatrix);

            vertexOut.Normals[i] = new Vector3(n.X, n.Y, n.Z);

            vertexOut.UVs[i] = triangle.UVs[i];
        }
        return vertexOut;
    }

    private RasterOut Rasterizer(VertexOut vertexOut)
    {
        var rasterOut = new RasterOut();
        for (int i = 0; i < 3; ++i)
        {
            Vector3 p = new Vector3(vertexOut.Positions[i].X, vertexOut.Positions[i].Y, vertexOut.Positions[i].Z) / vertexOut.Positions[i].W;
            rasterOut.ScreenPositions[i] = new Point(p.X * c_width, p.Y * c_height);
            rasterOut.Depths[i] = p.Z;
            rasterOut.Normals[i] = vertexOut.Normals[i];
            rasterOut.UVs[i] = vertexOut.UVs[i];

        }
        return rasterOut;
    }

    private void PixelShader(RasterOut rasterOut)
    {
        var lightDir = new Vector3(0, 0, -1);

        var boundingBox = new BoundingBox(rasterOut.ScreenPositions, c_boundingBoxMin, c_boundingBoxMax);

        for (int x = boundingBox.Left; x <= boundingBox.Right; ++x)
        {
            for (int y = boundingBox.Top; y <= boundingBox.Bottom; ++y)
            {
                Vector3 bc = MyMath.GetBarycentric(rasterOut.ScreenPositions, new Vector2(x, y));
                if (bc.X < 0.0f || bc.Y < 0.0f || bc.Z < 0.0f)
                {
                    continue;
                }

                Vector3 n = bc.X * rasterOut.Normals[0] + bc.Y * rasterOut.Normals[1] + bc.Z * rasterOut.Normals[2];
                Vector3 normal = Vector3.Normalize(n);
                float lightCoefficient = Vector3.Dot(normal, lightDir);
                if (lightCoefficient < 0.0f)
                {
                    continue;
                }

                float currentDepth = bc.X * rasterOut.Depths[0] + bc.Y * rasterOut.Depths[1] + bc.Z * rasterOut.Depths[2];
                float depthBufferDepth = m_depthBuffer[y][x];
                if (currentDepth < depthBufferDepth)
                {
                    continue;
                }
                m_depthBuffer[y][x] = currentDepth;

                byte c = System.Convert.ToByte(255.0f * lightCoefficient);

                SetPixel(x, y, c, c, c);
            }
        }
    }

    private void SetPixel(int x, int y, byte r, byte g, byte b)
    {
        SDL.SDL_SetRenderDrawColor(c_renderer, r, g, b, 255);
        SDL.SDL_RenderDrawPoint(c_renderer, x, y);
    }
}
