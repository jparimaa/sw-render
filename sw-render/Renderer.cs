using SDL2;

class Renderer
{
    private readonly System.IntPtr m_window;
    private readonly int m_width;
    private readonly int m_height;
    private readonly System.IntPtr m_surfacePtr;
    private readonly SDL.SDL_Surface m_surface;
    private readonly uint m_windowPixelFormat;
    private readonly byte m_bytesPerPixel;

    public Renderer(System.IntPtr window, int width, int height)
    {
        m_window = window;
        m_width = width;
        m_height = height;
        m_surfacePtr = SDL.SDL_GetWindowSurface(m_window);
        m_surface = System.Runtime.InteropServices.Marshal.PtrToStructure<SDL.SDL_Surface>(m_surfacePtr);
        m_windowPixelFormat = SDL.SDL_GetWindowPixelFormat(m_window);
        m_bytesPerPixel = SDL.SDL_BYTESPERPIXEL(m_windowPixelFormat);
    }

    public void Clear()
    {
        SDL.SDL_Rect rect = new SDL.SDL_Rect() { x = 0, y = 0, w = m_width, h = m_height };
        SDL.SDL_FillRect(m_surfacePtr, ref rect, SDL.SDL_MapRGB(m_surface.format, 0, 0, 0));
    }

    public void RenderNoise(int amount = 10000)
    {
        var random = new System.Random();

        for (int i = 0; i < amount; i++)
        {
            int x = random.Next(m_width);
            int y = random.Next(m_height);

            var color = new byte[3];
            random.NextBytes(color);

            uint rgb = SDL.SDL_MapRGB(m_surface.format, color[0], color[1], color[2]);

            SetPixel(x, y, rgb);
        }
    }

    public void Finish()
    {
        SDL.SDL_UpdateWindowSurface(m_window);
    }

    private void SetPixel(int x, int y, uint color)
    {
        int offset = y * m_surface.pitch + x * m_bytesPerPixel;
        System.Runtime.InteropServices.Marshal.WriteIntPtr(m_surface.pixels, offset, new System.IntPtr(color));
    }
}
