using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using EscapeMaze.Scenes;

namespace EscapeMaze;

public class Game : GameWindow
{
    private IScene? _currentScene;

    public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        Console.WriteLine("OnLoad - Initializing game");

        _currentScene = new MenuScene();
        _currentScene.Initialize(Size.X, Size.Y);

        CursorState = CursorState.Normal;
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        _currentScene?.Render(args);

        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        if (_currentScene == null)
        {
            return;
        }

        _currentScene.Update(args, KeyboardState, IsFocused);

        if (_currentScene.ShouldChangeScene(out IScene? nextScene))
        {
            _currentScene.Unload();
            _currentScene = nextScene;
            _currentScene?.Initialize(Size.X, Size.Y);

            if (_currentScene is Level1Scene)
            {
                CursorState = CursorState.Grabbed;
            }
            else if (_currentScene is MenuScene)
            {
                CursorState = CursorState.Normal;
            }
        }

        if (_currentScene is MenuScene menuScene && menuScene.ShouldQuit)
        {
            Close();
        }
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, e.Width, e.Height);

        _currentScene?.Resize(e.Width, e.Height);
    }

    protected override void OnUnload()
    {
        base.OnUnload();

        _currentScene?.Unload();
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
        base.OnMouseMove(e);

        if (!IsFocused)
        {
            return;
        }

        _currentScene?.MouseMove(e.X, e.Y);
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);

        if (!IsFocused || e.Button != MouseButton.Left)
        {
            return;
        }

        if (_currentScene is MenuScene menuScene)
        {
            menuScene.MouseClick(MouseState.X, MouseState.Y);
        }
    }
}
