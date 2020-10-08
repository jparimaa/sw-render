using ObjLoader.Loader.Data.Elements;
using ObjLoader.Loader.Data.VertexData;
using ObjLoader.Loader.Loaders;
using SDL2;
using System.Collections.Generic;

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

        List<Point> triangle1 = new List<Point>()
        {
            new Point(0,0),
            new Point(100,100),
            new Point(0,100),
        };

        List<Point> triangle2 = new List<Point>()
        {
            new Point(300,300),
            new Point(200,200),
            new Point(200,300),
        };

        List<List<Point>> points = new List<List<Point>>();
        {
            var objLoaderFactory = new ObjLoaderFactory();
            var objLoader = objLoaderFactory.Create();
            var fileStream = new System.IO.FileStream("../../../head.obj", System.IO.FileMode.Open);
            LoadResult model = objLoader.Load(fileStream);

            foreach (Group group in model.Groups)
            {
                foreach (Face face in group.Faces)
                {
                    List<Point> facePoints = new List<Point>();
                    for (int i = 0; i < face.Count; ++i)
                    {
                        Vertex v = model.Vertices[face[i].VertexIndex - 1];
                        Point p = new Point((v.X + 1.0f) * width / 2, height - ((v.Y + 1.0f) * height / 2));
                        facePoints.Add(p);
                    }
                    points.Add(facePoints);
                }
            }
        }

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
            //renderer.RenderTriangle(triangle1);
            //renderer.RenderTriangle(triangle2);
            foreach (var facePoints in points)
            {
                renderer.RenderTriangle(facePoints);
            }

            renderer.Finish();
        }

        SDL.SDL_DestroyWindow(window);
        SDL.SDL_Quit();
    }
}
