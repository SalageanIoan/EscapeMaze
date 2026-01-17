using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using EscapeMaze.Input;
using EscapeMaze.Objects;
using EscapeMaze.Rendering;

namespace EscapeMaze.Scenes;

public class Level1Scene : IScene
{
    private Camera? _camera;
    private InputHandler? _inputHandler;
    private Shader? _shader;
    private GameObject? _key;
    private List<GameObject> _walls = new ();
    private GameObject? _floor;
    private bool _shouldChangeScene;
    private IScene? _nextScene;
    private bool _hasKey;
    private GameObject? _door;
    private Vector3 _doorStartPosition;

    public void Initialize(int windowWidth, int windowHeight)
    {
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        _camera = new Camera(Vector3.UnitZ * 3, windowWidth / (float)windowHeight);

        _inputHandler = new InputHandler(_camera);

        string vertexShaderSource = ShaderLoader.LoadShaderSource("Shaders/vertex_shader.txt");
        string fragmentShaderSource = ShaderLoader.LoadShaderSource("Shaders/fragment_shader.txt");
        _shader = new Shader(vertexShaderSource, fragmentShaderSource);

        _floor = new GameObject("Data/floor_vertices.txt", Vector3.Zero, "Data/UI/floor.png");
        
        int[,] mazeLayout = new [,]
        {
            {1, 1, 1, 1, 1, 2, 1, 1, 1, 1},
            {1, 0, 0, 0, 1, 0, 0, 0, 0, 1},
            {1, 0, 1, 0, 1, 0, 1, 1, 0, 1},
            {1, 0, 1, 0, 0, 0, 1, 0, 0, 1},
            {1, 0, 1, 1, 1, 1, 1, 0, 1, 1},
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 1, 1, 0, 1, 1, 1, 1, 0, 1},
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 0, 1, 0, 0, 0, 1, 1, 0, 1},
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
        };

        _walls = MazeGenerator.GenerateMaze(mazeLayout);
        foreach (var wall in _walls)
        {
            if (wall.Texture != null && wall.Texture.Path.Contains("door.png"))
            {
                _door = wall;
                _doorStartPosition = _door.Position;
                break;
            }
        }

        _key = new GameObject("Data/cube_vertices.txt", new Vector3(-0.5f, 0, 0.5f), "Data/UI/key.png");
        _key.Scale = new Vector3(0.5f);
    }

    public void Update(FrameEventArgs args, KeyboardState keyboardState, bool isFocused)
    {
        if (!isFocused || _camera == null || _inputHandler == null)
        {
            return;
        }

        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            _shouldChangeScene = true;
            _nextScene = new MenuScene();
        }

        _inputHandler.ProcessKeyboard(keyboardState, (float)args.Time, _walls);

        if (_inputHandler.EPressed)
        {
            if (_key != null)
            {
                var ray = new Ray(_camera.Position, _camera.Front);
                Vector3 extents = new Vector3(0.5f) * _key.Scale; 
                var boundingBox = new BoundingBox(_key.Position - extents, _key.Position + extents);
                if (Intersects(ray, boundingBox))
                {
                    _key.IsRotating = true;
                    _key.IsFadingOut = true;
                }
            }
            else if (_hasKey && _door != null && !_door.IsOpening)
            {
                var ray = new Ray(_camera.Position, _camera.Front);
                var boundingBox = new BoundingBox(_door.Position - new Vector3(0.5f), _door.Position + new Vector3(0.5f));
                if (Intersects(ray, boundingBox))
                {
                    _door.IsOpening = true;
                }
            }
        }

        if (_door != null && _door.IsOpening)
        {
            if (_door.Position.X > _doorStartPosition.X - 1.0f)
            {
                _door.Position -= new Vector3((float)args.Time, 0, 0);
            }
        }

        _key?.Update((float)args.Time);
        _floor?.Update((float)args.Time);
        foreach (var wall in _walls)
        {
            wall.Update((float)args.Time);
        }

        if (_key?.Alpha == 0)
        {
            _key.Dispose();
            _key = null;
            _hasKey = true;
        }
    }

    private bool Intersects(Ray ray, BoundingBox box)
    {
        float tmin = (box.Min.X - ray.Origin.X) / ray.Direction.X;
        float tmax = (box.Max.X - ray.Origin.X) / ray.Direction.X;

        if (tmin > tmax)
        {
            (tmin, tmax) = (tmax, tmin);
        }

        float tymin = (box.Min.Y - ray.Origin.Y) / ray.Direction.Y;
        float tymax = (box.Max.Y - ray.Origin.Y) / ray.Direction.Y;

        if (tymin > tymax)
        {
            (tymin, tymax) = (tymax, tymin);
        }

        if ((tmin > tymax) || (tymin > tmax))
        {
            return false;
        }

        if (tymin > tmin)
        {
            tmin = tymin;
        }

        if (tymax < tmax)
        {
            tmax = tymax;
        }

        float tzmin = (box.Min.Z - ray.Origin.Z) / ray.Direction.Z;
        float tzmax = (box.Max.Z - ray.Origin.Z) / ray.Direction.Z;

        if (tzmin > tzmax)
        {
            (tzmin, tzmax) = (tzmax, tzmin);
        }

        if ((tmin > tzmax) || (tzmin > tmax))
        {
            return false;
        }

        return true;
    }

    public void Render(FrameEventArgs args)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        if (_shader == null || _camera == null)
        {
            return;
        }

        _shader.Use();
        _shader.SetInt("texture0", 0);

        Matrix4 view = _camera.GetViewMatrix();
        Matrix4 projection = _camera.GetProjectionMatrix();

        _shader.SetMatrix4("view", view);
        _shader.SetMatrix4("projection", projection);

        if (_floor != null)
        {
            _floor.Texture?.Use();
            _shader.SetBool("useTexture", true);
            _shader.SetMatrix4("model", _floor.GetModelMatrix());
            _shader.SetFloat("alpha", _floor.Alpha);
            _floor.Draw();
        }

        foreach (var wall in _walls)
        {
            wall.Texture?.Use();
            _shader.SetBool("useTexture", true);
            _shader.SetMatrix4("model", wall.GetModelMatrix());
            _shader.SetFloat("alpha", wall.Alpha);
            wall.Draw();
        }

        if (_key != null)
        {
            _key.Texture?.Use();
            _shader.SetBool("useTexture", true);
            _shader.SetMatrix4("model", _key.GetModelMatrix());
            _shader.SetFloat("alpha", _key.Alpha);
            _key.Draw();
        }
    }

    public void Resize(int width, int height)
    {
        if (_camera != null)
        {
            _camera.AspectRatio = width / (float)height;
        }
    }

    public void MouseMove(float x, float y)
    {
        if (_inputHandler == null)
        {
            return;
        }

        _inputHandler.ProcessMouse(x, y);
    }

    public void Unload()
    {
        _key?.Dispose();
        _floor?.Dispose();
        foreach (var wall in _walls)
        {
            wall.Dispose();
        }
        _walls.Clear();
        _shader?.Dispose();
    }

    public bool ShouldChangeScene(out IScene? nextScene)
    {
        nextScene = _nextScene;
        return _shouldChangeScene;
    }
}