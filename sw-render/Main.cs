using ObjLoader.Loader.Data.Elements;
using ObjLoader.Loader.Loaders;
using SDL2;
using System.Collections.Generic;
using System.Numerics;

class Program
{
    static int s_currentMouseX = -1;
    static int s_currentMouseY = -1;

    static void Main(string[] args)
    {
        SDL.SDL_Init(0);
        int width = 800;
        int height = 600;
        System.IntPtr window = SDL.SDL_CreateWindow("mycs", 50, 50, width, height, 0);
        SDL.SDL_SetWindowTitle(window, "FPS:");
        System.IntPtr renderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

        var renderPipeline = new RenderPipeline(renderer, width, height);
        List<Triangle> triangles = GenerateTriangles();

        SDL.SDL_Event e;
        bool quit = false;

        int frameCounter = 0;
        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        var camera = new Camera();
        var matrices = new RenderPipeline.Matrices();
        matrices.ProjectionMatrix = camera.GetProjectionMatrix();

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
                MoveCamera(e, camera);
            }

            SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 32, 255);
            SDL.SDL_RenderClear(renderer);

            matrices.ViewMatrix = camera.GetViewMatrix();
            renderPipeline.ClearDepth();
            renderPipeline.DrawTriangles(triangles, matrices);

            SDL.SDL_RenderPresent(renderer);

            ++frameCounter;
            long elapsedMs = stopwatch.ElapsedMilliseconds;
            if (elapsedMs > 1000)
            {
                float FPS = frameCounter / (elapsedMs / 1000.0f);
                SDL.SDL_SetWindowTitle(window, "FPS:" + FPS.ToString());
                frameCounter = 0;
                stopwatch.Restart();
            }
        }

        SDL.SDL_DestroyRenderer(renderer);
        SDL.SDL_DestroyWindow(window);
        SDL.SDL_Quit();
    }

    private static List<Triangle> GenerateTriangles()
    {
        var objLoaderFactory = new ObjLoaderFactory();
        var objLoader = objLoaderFactory.Create();
        var fileStream = new System.IO.FileStream("../../../head.obj", System.IO.FileMode.Open);
        LoadResult model = objLoader.Load(fileStream);

        var triangles = new List<Triangle>();
        foreach (Group group in model.Groups)
        {
            foreach (Face face in group.Faces)
            {
                var triangle = new Triangle();
                for (int i = 0; i < face.Count; ++i)
                {
                    ObjLoader.Loader.Data.VertexData.Vertex v = model.Vertices[face[i].VertexIndex - 1];
                    triangle.Positions[i] = new Vector3(v.X, v.Y, v.Z);
                    ObjLoader.Loader.Data.VertexData.Normal n = model.Normals[face[i].NormalIndex - 1];
                    triangle.Normals[i] = new Vector3(n.X, n.Y, -n.Z);
                    ObjLoader.Loader.Data.VertexData.Texture t = model.Textures[face[i].NormalIndex - 1];
                    triangle.UVs[i] = new Vector2(t.X, t.Y);
                }
                triangles.Add(triangle);
            }
        }
        return triangles;
    }

    private static void MoveCamera(SDL.SDL_Event e, Camera camera)
    {
        switch (e.type)
        {
            case SDL.SDL_EventType.SDL_KEYDOWN:
                if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_w)
                {
                    camera.position += camera.GetForward() * 0.1f;
                }
                if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_s)
                {
                    camera.position -= camera.GetForward() * 0.1f;
                }
                if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_a)
                {
                    camera.position -= camera.GetRight() * 0.1f;
                }
                if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_d)
                {
                    camera.position += camera.GetRight() * 0.1f;
                }
                if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_r)
                {
                    camera.position = new Vector3(0, 0, 10);
                    camera.rotation = new Vector3(0);
                }
                //System.Console.WriteLine("position: " + camera.position.ToString());
                break;
            case SDL.SDL_EventType.SDL_MOUSEMOTION:
                if (s_currentMouseX == -1 || s_currentMouseY == -1)
                {
                    s_currentMouseX = e.motion.x;
                    s_currentMouseY = e.motion.y;
                }
                float rotateY = -0.001f * (e.motion.x - s_currentMouseX);
                float rotateX = 0.001f * (e.motion.y - s_currentMouseY);
                camera.rotation.Y += rotateY;
                camera.rotation.X += rotateX;
                s_currentMouseX = e.motion.x;
                s_currentMouseY = e.motion.y;
                //System.Console.WriteLine("rotation: " + camera.rotation.ToString());
                break;
        }
    }
}
