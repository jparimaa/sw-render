using SDL2;
using System.Collections.Generic;
using System.Numerics;

class Rasterizer
{
    private readonly System.IntPtr c_renderer;
    private readonly int c_width;
    private readonly int c_height;
    private readonly Point c_boundingBoxMin;
    private readonly Point c_boundingBoxMax;

    private List<List<float>> m_depthBuffer;

    public Rasterizer(System.IntPtr renderer, int width, int height)
    {
        c_renderer = renderer;
        c_width = width;
        c_height = height;
        c_boundingBoxMin = new Point(0, 0);
        c_boundingBoxMax = new Point(c_width - 1, c_height - 1);

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

    public void RasterizeNoise(int amount = 10000)
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

    public void RasterizeTriangle(Triangle triangle)
    {
        var lightDir = new Vector3(0, 0, -1);

        var boundingBox = new BoundingBox(triangle.ScreenPositions, c_boundingBoxMin, c_boundingBoxMax);

        for (int x = boundingBox.Left; x <= boundingBox.Right; ++x)
        {
            for (int y = boundingBox.Top; y <= boundingBox.Bottom; ++y)
            {
                Vector3 bc = MyMath.GetBarycentric(triangle.ScreenPositions, new Vector2(x, y));
                if (bc.X < 0.0f || bc.Y < 0.0f || bc.Z < 0.0f)
                {
                    continue;
                }

                Vector3 normal = Vector3.Normalize(bc.X * triangle.Normals[0] + bc.Y * triangle.Normals[1] + bc.Z * triangle.Normals[2]);
                float lightCoefficient = Vector3.Dot(normal, lightDir);
                if (lightCoefficient < 0.0f)
                {
                    continue;
                }

                float currentDepth = bc.X * triangle.Depths[0] + bc.Y * triangle.Depths[1] + bc.Z * triangle.Depths[2];
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
