using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using EscapeMaze.Rendering;

namespace EscapeMaze.Scenes;

public class MenuScene : IScene
{
    private Button? _startButton;
    private Button? _quitButton;
    private bool _shouldChangeScene;
    private IScene? _nextScene;
    private bool _shouldQuit;
    private int _windowWidth;
    private int _windowHeight;
    private float _lastMouseX;
    private float _lastMouseY;
    private bool _mouseButtonPressed;

    public bool ShouldQuit => _shouldQuit;

    public void Initialize(int windowWidth, int windowHeight)
    {
        _windowWidth = windowWidth;
        _windowHeight = windowHeight;

        GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
        GL.Disable(EnableCap.DepthTest);


        float buttonWidth = 200;
        float buttonHeight = 90;
        float centerX = (windowWidth - buttonWidth) / 2;
        float startY = windowHeight / 2 - 80;

        try
        {
            _startButton = new Button("Data/UI/start_button.png", "Data/UI/start_button.png",
                centerX, startY, buttonWidth, buttonHeight);
            _quitButton=new Button("Data/UI/quit_button.png", "Data/UI/quit_button.png",
                centerX, startY + 50, buttonWidth, buttonHeight);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load button textures: {ex.Message}");
        }
    }

    public void Update(FrameEventArgs args, KeyboardState keyboardState, bool isFocused)
    {
        if (!isFocused)
        {
            return;
        }

        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            _shouldQuit = true;
        }

        _startButton?.UpdateHoverState(_lastMouseX, _lastMouseY);
        _quitButton?.UpdateHoverState(_lastMouseX, _lastMouseY);

        if (_mouseButtonPressed)
        {
            if (_startButton != null && _startButton.IsClicked(_lastMouseX, _lastMouseY))
            {
                _shouldChangeScene = true;
                _nextScene = new Level1Scene();
            }
            else if (_quitButton != null && _quitButton.IsClicked(_lastMouseX, _lastMouseY))
            {
                _shouldQuit = true;
            }
            _mouseButtonPressed = false;
        }
    }

    public void Render(FrameEventArgs args)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);

        _startButton?.Render(_windowWidth, _windowHeight);
        _quitButton?.Render(_windowWidth, _windowHeight);
    }

    public void Resize(int width, int height)
    {
        _windowWidth = width;
        _windowHeight = height;

        if (_startButton != null && _quitButton != null)
        {
            float buttonWidth = 200;
            float centerX = (width - buttonWidth) / 2;
            float startY = height / 2 - 80;

            _startButton.X = centerX;
            _startButton.Y = startY;
            _quitButton.X = centerX;
            _quitButton.Y = startY + 100;
        }
    }

    public void MouseMove(float x, float y)
    {
        _lastMouseX = x;
        _lastMouseY = y;
    }

    public void MouseClick(float x, float y)
    {
        _mouseButtonPressed = true;
        _lastMouseX = x;
        _lastMouseY = y;
    }

    public void Unload()
    {
        _startButton?.Dispose();
        _quitButton?.Dispose();
    }

    public bool ShouldChangeScene(out IScene? nextScene)
    {
        nextScene = _nextScene;
        return _shouldChangeScene;
    }
}
