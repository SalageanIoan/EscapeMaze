using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using EscapeMaze.Rendering;
using OpenTK.Graphics.OpenGL4;

namespace EscapeMaze.Scenes;

public class GameOverScene : IScene
{
    private Button? _gameOverImage;
    private int _windowWidth;
    private int _windowHeight;
    private bool _shouldQuit;
    private double _timeElapsed;

    public void Initialize(int windowWidth, int windowHeight)
    {
        _windowWidth = windowWidth;
        _windowHeight = windowHeight;

        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
        GL.Disable(EnableCap.DepthTest);
        
        _gameOverImage = new Button("Data/UI/GameOver.png", "Data/UI/GameOver.png", 
            0, 0, windowWidth, windowHeight);
    }

    public void Update(FrameEventArgs args, KeyboardState keyboardState, bool isFocused)
    {
        _timeElapsed += args.Time;

        if (!isFocused) return;

        if (_timeElapsed > 5.0 && keyboardState.IsAnyKeyDown)
        {
             _shouldQuit = true;
        }
    }

    public void Render(FrameEventArgs args)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);
        _gameOverImage?.Render(_windowWidth, _windowHeight);
    }

    public void Resize(int width, int height)
    {
        _windowWidth = width;
        _windowHeight = height;
        
        if (_gameOverImage != null)
        {
             _gameOverImage.X = 0;
             _gameOverImage.Y = 0;
             _gameOverImage.Width = width;
             _gameOverImage.Height = height;
        }
    }

    public void MouseMove(float x, float y) { }

    public void Unload()
    {
        _gameOverImage?.Dispose();
    }

    public bool ShouldQuit => _shouldQuit;

    public bool ShouldChangeScene(out IScene? nextScene)
    {
        nextScene  = null;
        return false; 
    }
}
