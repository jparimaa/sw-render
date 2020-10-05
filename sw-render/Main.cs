using SDL2;

class Program
{
    static void Main(string[] args)
    {
        SDL.SDL_Init(SDL.SDL_INIT_VIDEO);
        int width = 800;
        int height = 600;
        System.IntPtr window = SDL.SDL_CreateWindow("mycs", 50, 50, width, height, 0);

        uint windowPixelFormat = SDL.SDL_GetWindowPixelFormat(window);
        if (SDL.SDL_BYTESPERPIXEL(windowPixelFormat) != 4)
        {
            System.Console.WriteLine("ERROR: Pixel format not supported at the moment.");
            return;
        }

        Renderer renderer = new Renderer(window, width, height);

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

            renderer.Clear();
            renderer.RenderNoise();
            renderer.Finish();
        }

        SDL.SDL_DestroyWindow(window);
        SDL.SDL_Quit();
    }
}
