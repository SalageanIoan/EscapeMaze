using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace EscapeMaze.Scenes;

public interface IScene
{
    void Initialize(int windowWidth, int windowHeight);
    void Update(FrameEventArgs args, KeyboardState keyboardState, bool isFocused);
    void Render(FrameEventArgs args);
    void Resize(int width, int height);
    void MouseMove(float x, float y);
    void Unload();
    bool ShouldChangeScene(out IScene? nextScene);
}
