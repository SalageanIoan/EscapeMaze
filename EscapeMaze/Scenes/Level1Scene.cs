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
    private GameObject? _floor;
    private bool _shouldChangeScene;
    private IScene? _nextScene;

    public void Initialize(int windowWidth, int windowHeight)
    {
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.Enable(EnableCap.DepthTest);

        _camera = new Camera(Vector3.UnitZ * 3, windowWidth / (float)windowHeight);
        Console.WriteLine($"Camera initialized at position: {_camera.Position}");

        _inputHandler = new InputHandler(_camera);

        string vertexShaderSource = ShaderLoader.LoadShaderSource("Shaders/vertex_shader.txt");
        string fragmentShaderSource = ShaderLoader.LoadShaderSource("Shaders/fragment_shader.txt");
        _shader = new Shader(vertexShaderSource, fragmentShaderSource);

        _cube = new GameObject("Data/cube_vertices.txt", Vector3.Zero);
        _floor = new GameObject("Data/floor_vertices.txt", Vector3.Zero);
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
    }

    public void Render(FrameEventArgs args)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        if (_shader == null || _camera == null || _cube == null || _floor == null)
        {
            return;
        }

        _shader.Use();

        Matrix4 view = _camera.GetViewMatrix();
        Matrix4 projection = _camera.GetProjectionMatrix();

        _shader.SetMatrix4("view", view);
        _shader.SetMatrix4("projection", projection);

        _shader.SetMatrix4("model", _cube.GetModelMatrix());
        _cube.Draw();

        _shader.SetMatrix4("model", _floor.GetModelMatrix());
        _floor.Draw();
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
        _floor?.Dispose();
        _shader?.Dispose();
    }

    public bool ShouldChangeScene(out IScene? nextScene)
    {
        nextScene = _nextScene;
        return _shouldChangeScene;
    }
}