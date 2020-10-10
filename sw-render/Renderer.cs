using SDL2;
using System.Collections.Generic;
using System.Numerics;

class Renderer
{
    private readonly System.IntPtr c_window;
    private readonly int c_width;
    private readonly int c_height;
    private readonly System.IntPtr c_surfacePtr;
    private readonly SDL.SDL_Surface c_surface;
    private readonly uint c_windowPixelFormat;
    private readonly byte c_bytesPerPixel;
    private readonly Point c_boundingBoxMin;
    private readonly Point c_boundingBoxMax;

    public Renderer(System.IntPtr window, int width, int height)
    {
        c_window = window;
        c_width = width;
        c_height = height;
        c_surfacePtr = SDL.SDL_GetWindowSurface(c_window);
        c_surface = System.Runtime.InteropServices.Marshal.PtrToStructure<SDL.SDL_Surface>(c_surfacePtr);
        c_windowPixelFormat = SDL.SDL_GetWindowPixelFormat(c_window);
        c_bytesPerPixel = SDL.SDL_BYTESPERPIXEL(c_windowPixelFormat);
        c_boundingBoxMin = new Point(0, 0);
        c_boundingBoxMax = new Point(c_width - 1, c_height - 1);
    }

    public void Clear()
    {
        SDL.SDL_Rect rect = new SDL.SDL_Rect() { x = 0, y = 0, w = c_width, h = c_height };
        SDL.SDL_FillRect(c_surfacePtr, ref rect, SDL.SDL_MapRGB(c_surface.format, 0, 0, 64));
    }

    public void RenderNoise(int amount = 10000)
    {
        var random = new System.Random();

        for (int i = 0; i < amount; i++)
        {
            int x = random.Next(c_width);
            int y = random.Next(c_height);

            var color = new byte[3];
            random.NextBytes(color);

            uint rgb = SDL.SDL_MapRGB(c_surface.format, color[0], color[1], color[2]);

            SetPixel(x, y, rgb);
        }
    }

    public void RenderTriangle(List<Point> points)
    {
        BoundingBox boundingBox = new BoundingBox(points, c_boundingBoxMin, c_boundingBoxMax);
        uint rgb = SDL.SDL_MapRGB(c_surface.format, 255, 255, 0);

        for (int x = boundingBox.Left; x <= boundingBox.Right; ++x)
        {
            for (int y = boundingBox.Top; y <= boundingBox.Bottom; ++y)
            {
                Vector3 bc = MyMath.GetBarycentric(points, new Vector2(x, y));
                if (bc.X < 0.0f || bc.Y < 0.0f || bc.Z < 0.0f)
                {
                    continue;
                }

                SetPixel(x, y, rgb);
            }
        }
    }

    public void Finish()
    {
        SDL.SDL_UpdateWindowSurface(c_window);
    }

    private void SetPixel(int x, int y, uint color)
    {
        int offset = y * c_surface.pitch + x * c_bytesPerPixel;
        System.Runtime.InteropServices.Marshal.WriteIntPtr(c_surface.pixels, offset, new System.IntPtr(color));
    }
}
