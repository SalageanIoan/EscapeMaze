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
    private GameObject? _cube;
    private bool _shouldChangeScene;
    private IScene? _nextScene;

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

        _cube = new GameObject("Data/cube_vertices.txt", Vector3.Zero, "Data/UI/key.png");
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

        _inputHandler.ProcessKeyboard(keyboardState, (float)args.Time);

        if (_inputHandler.EPressed && _cube != null)
        {
            var ray = new Ray(_camera.Position, _camera.Front);
            var boundingBox = new BoundingBox(_cube.Position - new Vector3(0.5f), _cube.Position + new Vector3(0.5f));
            if (Intersects(ray, boundingBox))
            {
                _cube.IsRotating = true;
                _cube.IsFadingOut = true;
            }
        }

        _cube?.Update((float)args.Time);

        if (_cube?.Alpha == 0)
        {
            _cube.Dispose();
            _cube = null;
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

        if (_cube != null)
        {
            _cube.Texture?.Use();
            _shader.SetBool("useTexture", true);
            _shader.SetMatrix4("model", _cube.GetModelMatrix());
            _shader.SetFloat("alpha", _cube.Alpha);
            _cube.Draw();
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
        _cube?.Dispose();
        _shader?.Dispose();
    }

    public bool ShouldChangeScene(out IScene? nextScene)
    {
        nextScene = _nextScene;
        return _shouldChangeScene;
    }
}