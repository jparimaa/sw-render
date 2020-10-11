using ObjLoader.Loader.Data.Elements;
using ObjLoader.Loader.Loaders;
using SDL2;
using System.Collections.Generic;
using System.Numerics;

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

        var renderer = new Renderer(window, width, height);

        SDL.SDL_Event e;
        bool quit = false;

        var points = new List<List<Point>>();
        var normals = new List<List<Vector3>>();
        {
            var objLoaderFactory = new ObjLoaderFactory();
            var objLoader = objLoaderFactory.Create();
            var fileStream = new System.IO.FileStream("../../../head.obj", System.IO.FileMode.Open);
            LoadResult model = objLoader.Load(fileStream);

            foreach (Group group in model.Groups)
            {
                foreach (Face face in group.Faces)
                {
                    var facePoints = new List<Point>();
                    var faceNormals = new List<Vector3>();
                    for (int i = 0; i < face.Count; ++i)
                    {

                        ObjLoader.Loader.Data.VertexData.Vertex v = model.Vertices[face[i].VertexIndex - 1];
                        var p = new Point((v.X + 1.0f) * width / 2, height - ((v.Y + 1.0f) * height / 2));
                        facePoints.Add(p);

                        ObjLoader.Loader.Data.VertexData.Normal n = model.Normals[face[i].NormalIndex - 1];
                        faceNormals.Add(new Vector3(n.X, n.Y, n.Z));
                    }
                    points.Add(facePoints);
                    normals.Add(faceNormals);
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
            for (int i = 0; i < points.Count; ++i)
            {
                renderer.RenderTriangle(points[i], normals[i]);
            }

            renderer.Finish();
        }

        SDL.SDL_DestroyWindow(window);
        SDL.SDL_Quit();
    }
}
