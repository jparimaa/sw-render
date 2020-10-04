using SDL2;

class Program
{
    static void SetPixel(SDL.SDL_Surface surface, uint format, int x, int y, uint pixel)
    {
        int offset = y * surface.pitch + x * SDL.SDL_BYTESPERPIXEL(format);
        System.Runtime.InteropServices.Marshal.WriteIntPtr(surface.pixels, offset, new System.IntPtr(pixel));
    }

    static void Render(System.IntPtr window)
    {
        System.IntPtr surfacePtr = SDL.SDL_GetWindowSurface(window);

        SDL.SDL_Surface surface = System.Runtime.InteropServices.Marshal.PtrToStructure<SDL.SDL_Surface>(surfacePtr);
        SDL.SDL_Rect rect = new SDL.SDL_Rect() { x = 0, y = 0, w = 800, h = 600 };
        SDL.SDL_FillRect(surfacePtr, ref rect, SDL.SDL_MapRGB(surface.format, 0, 0, 0));

        var random = new System.Random();

        uint windowPixelFormat = SDL.SDL_GetWindowPixelFormat(window);

        for (int i = 0; i < 10000; i++)
        {
            int x = random.Next(700);
            int y = random.Next(600);

            var color = new byte[3];
            random.NextBytes(color);

            uint rgb = SDL.SDL_MapRGB(surface.format, color[0], color[1], color[2]);

            SetPixel(surface, windowPixelFormat, x, y, rgb);
        }

        SDL.SDL_UpdateWindowSurface(window);
    }

    static void Main(string[] args)
    {
        SDL.SDL_Init(SDL.SDL_INIT_VIDEO);
        System.IntPtr window = SDL.SDL_CreateWindow("mycs", 50, 50, 800, 600, 0);

        uint windowPixelFormat = SDL.SDL_GetWindowPixelFormat(window);
        if (SDL.SDL_BYTESPERPIXEL(windowPixelFormat) != 4)
        {
            System.Console.WriteLine("ERROR: Pixel format not supported at the moment.");
            return;
        }

        SDL.SDL_Event e;
        bool quit = false;
        while (!quit)
        {
            while (SDL.SDL_PollEvent(out e) != 0)
            {
                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        quit = true;
                        break;
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_ESCAPE)
                        {
                            quit = true;
                        }
                        break;
                }
            }

            Render(window);
        }

        SDL.SDL_DestroyWindow(window);
        SDL.SDL_Quit();
    }
}
