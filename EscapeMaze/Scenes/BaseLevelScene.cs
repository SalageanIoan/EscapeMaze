using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using EscapeMaze.Input;
using EscapeMaze.Objects;
using EscapeMaze.Rendering;

namespace EscapeMaze.Scenes;

public abstract class BaseLevelScene : IScene
{
    protected Camera? Camera;
    protected InputHandler? InputHandler;
    protected Shader? Shader;
    protected GameObject? Key;
    protected List<GameObject> Walls = new ();
    protected GameObject? Floor;
    protected bool _shouldChangeScene;
    protected IScene? NextScene;
    protected bool HasKey;
    protected GameObject? Door;
    protected Vector3 DoorStartPosition;
    protected bool IsPaused;
    protected Button? ResumeButton;
    protected int WindowWidth;
    protected int WindowHeight;
    protected Overlay? Overlay;
    protected Button? InteractLabel;
    protected bool ShowInteractLabel;

    public abstract void Initialize(int windowWidth, int windowHeight);
    public abstract void Update(FrameEventArgs args, KeyboardState keyboardState, bool isFocused);

    public void Render(FrameEventArgs args)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        if (Shader == null || Camera == null)
        {
            return;
        }

        Shader.Use();
        Shader.SetInt("texture0", 0);

        Matrix4 view = Camera.GetViewMatrix();
        Matrix4 projection = Camera.GetProjectionMatrix();

        Shader.SetMatrix4("view", view);
        Shader.SetMatrix4("projection", projection);

        if (Floor != null)
        {
            Floor.Texture?.Use();
            Shader.SetBool("useTexture", true);
            Shader.SetMatrix4("model", Floor.GetModelMatrix());
            Shader.SetFloat("alpha", Floor.Alpha);
            Floor.Draw();
        }

        foreach (var wall in Walls)
        {
            wall.Texture?.Use();
            Shader.SetBool("useTexture", true);
            Shader.SetMatrix4("model", wall.GetModelMatrix());
            Shader.SetFloat("alpha", wall.Alpha);
            wall.Draw();
        }

        if (Key != null)
        {
            Key.Texture?.Use();
            Shader.SetBool("useTexture", true);
            Shader.SetMatrix4("model", Key.GetModelMatrix());
            Shader.SetFloat("alpha", Key.Alpha);
            Key.Draw();
        }

        if (ShowInteractLabel && !IsPaused)
        {
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            InteractLabel?.Render(WindowWidth, WindowHeight);
            GL.Enable(EnableCap.DepthTest);
        }

        if (IsPaused)
        {
            RenderPauseScreen();
        }
    }

    protected void RenderPauseScreen()
    {
        GL.Disable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend);
        
        Overlay?.Render();

        ResumeButton?.Render(WindowWidth, WindowHeight);

        GL.Enable(EnableCap.DepthTest);
    }

    public abstract void Resize(int width, int height);
    public abstract void MouseMove(float x, float y);
    public abstract void Unload();
    public abstract bool ShouldChangeScene(out IScene? nextScene);
}
