using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using EscapeMaze.Input;
using EscapeMaze.Objects;
using EscapeMaze.Rendering;

namespace EscapeMaze.Scenes;

public class Level2Scene : BaseLevelScene
{


    public override void Initialize(int windowWidth, int windowHeight)
    {
        WindowWidth = windowWidth;
        WindowHeight = windowHeight;

        GL.ClearColor(0.5f, 0.1f, 0.1f, 1.0f);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        Camera = new Camera(new Vector3(0, 0, 2.5f), windowWidth / (float)windowHeight);

        InputHandler = new InputHandler(Camera);

        string vertexShaderSource = ShaderLoader.LoadShaderSource("Shaders/vertex_shader.txt");
        string fragmentShaderSource = ShaderLoader.LoadShaderSource("Shaders/fragment_shader.txt");
        Shader = new Shader(vertexShaderSource, fragmentShaderSource);

        Floor = new GameObject("Data/floor_vertices.txt", Vector3.Zero, "Data/UI/floor.png");
        
        int[,] mazeLayout = new [,]
        {
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1}, 
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 1}, 
            {1, 0, 1, 1, 1, 1, 1, 1, 0, 1}, 
            {1, 0, 1, 0, 0, 0, 0, 1, 0, 1}, 
            {1, 0, 1, 0, 1, 1, 0, 1, 0, 1}, 
            {1, 0, 1, 0, 1, 1, 0, 1, 0, 1}, 
            {1, 0, 1, 0, 1, 1, 0, 1, 0, 1}, 
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 1, 1, 1, 0, 1, 1, 1, 0, 1},
            {1, 1, 1, 1, 2, 1, 1, 1, 1, 1}
        };

        Walls = MazeGenerator.GenerateMaze(mazeLayout);
        foreach (var wall in Walls)
        {
            if (wall.Texture != null && wall.Texture.Path.Contains("door.png"))
            {
                Door = wall;
                DoorStartPosition = Door.Position;
                break;
            }
        }
        
        Key = new GameObject("Data/cube_vertices.txt", new Vector3(3.5f, 0, -3.5f), "Data/UI/key.png");
        Key.Scale = new Vector3(0.5f);

        float buttonWidth = 200;
        float buttonHeight = 90;
        ResumeButton = new Button("Data/UI/resume_button.png", "Data/UI/resume_button.png",
            (windowWidth - buttonWidth) / 2, (windowHeight - buttonHeight) / 2, buttonWidth, buttonHeight);

        InteractLabel = new Button("Data/UI/press_interact.png", "Data/UI/press_interact.png", 
            (windowWidth - 300) / 2, windowHeight - 100, 300, 70);

        Overlay = new Overlay();
    }

    public override void Update(FrameEventArgs args, KeyboardState keyboardState, bool isFocused)
    {
        if (!isFocused) return;

        if (keyboardState.IsKeyPressed(Keys.P))
        {
            IsPaused = !IsPaused;
            return;
        }

        if (IsPaused)
        {
            if (keyboardState.IsAnyKeyDown && !keyboardState.IsKeyDown(Keys.P) && !keyboardState.IsKeyDown(Keys.Escape))
            {
                IsPaused = false;
            }
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                 _shouldChangeScene = true;
                 NextScene = new MenuScene();
            }
            return;
        }

        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            _shouldChangeScene = true;
            NextScene = new MenuScene();
        }

        if (Camera == null || InputHandler == null)
        {
            return;
        }

        ShowInteractLabel = false;
        var ray = new Ray(Camera.Position, Camera.Front);
        float targetDistance = float.MaxValue;
        bool targetFound = false;

        if (Key != null)
        {
            Vector3 extents = new Vector3(0.5f) * Key.Scale;
            var boundingBox = new BoundingBox(Key.Position - extents, Key.Position + extents);
            if (Input.Collision.CollisionHelper.Intersects(ray, boundingBox, out float dist))
            {
                if (dist < 2.5f)
                {
                    targetDistance = dist;
                    targetFound = true;
                }
            }
        }
        else if (HasKey && Door != null && !Door.IsOpening)
        {
            var boundingBox = new BoundingBox(Door.Position - new Vector3(0.5f), Door.Position + new Vector3(0.5f));
            if (Input.Collision.CollisionHelper.Intersects(ray, boundingBox, out float dist))
            {
                if (dist < 2.5f)
                {
                    targetDistance = dist;
                    targetFound = true;
                }
            }
        }

        if (targetFound)
        {
            bool obstructed = false;
            foreach (var wall in Walls)
            {
                if (Key == null && wall == Door) continue;

                var wallBox = new BoundingBox(wall.Position - new Vector3(0.5f), wall.Position + new Vector3(0.5f));
                if (Input.Collision.CollisionHelper.Intersects(ray, wallBox, out float wallDist))
                {
                    if (wallDist < targetDistance - 0.1f && wallDist > 0)
                    {
                        obstructed = true;
                        break;
                    }
                }
            }
            if (!obstructed)
            {
                ShowInteractLabel = true;
            }
        }

        InputHandler.ProcessKeyboard(keyboardState, (float)args.Time, Walls);

        if (InputHandler.EPressed && ShowInteractLabel)
        {
            if (Key != null)
            {
                Key.IsRotating = true;
                Key.IsFadingOut = true;
            }
            else if (HasKey && Door != null && !Door.IsOpening)
            {
                Door.IsOpening = true;
            }
        }

        if (Door != null && Door.IsOpening)
        {
            if (Door.Position.X > DoorStartPosition.X - 1.0f)
            {
                Door.Position -= new Vector3((float)args.Time, 0, 0);
            }
        }


        if (Camera.Position.Z > 5.0f)
        {
             _shouldChangeScene = true;
             NextScene = new GameOverScene();
        }


        Key?.Update((float)args.Time);
        Floor?.Update((float)args.Time);
        foreach (var wall in Walls)
        {
            wall.Update((float)args.Time);
        }

        if (Key?.Alpha == 0)
        {
            Key.Dispose();
            Key = null;
            HasKey = true;
        }
    }


    public override void Resize(int width, int height)
    {
        WindowWidth = width;
        WindowHeight = height;
        if (Camera != null)
        {
            Camera.AspectRatio = width / (float)height;
        }
        if (ResumeButton != null)
        {
             ResumeButton.X = (width - ResumeButton.Width) / 2;
             ResumeButton.Y = (height - ResumeButton.Height) / 2;
        }
        if (InteractLabel != null)
        {
            InteractLabel.X = (width - 300) / 2;
            InteractLabel.Y = height - 100;
        }
    }

    public override void MouseMove(float x, float y)
    {
        if (InputHandler == null || IsPaused)
        {
            return;
        }

        InputHandler.ProcessMouse(x, y);
    }

    public override void Unload()
    {
        Key?.Dispose();
        Floor?.Dispose();
        foreach (var wall in Walls)
        {
            wall.Dispose();
        }
        Walls.Clear();
        Shader?.Dispose();
        Overlay?.Dispose();
        InteractLabel?.Dispose();
    }

    public override bool ShouldChangeScene(out IScene? nextScene)
    {
        nextScene = NextScene;
        return _shouldChangeScene;
    }
}
