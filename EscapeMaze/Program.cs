using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace EscapeMaze;

class Program
{
    static void Main()
    {
        var nativeWindowSettings = new NativeWindowSettings()
        {
            ClientSize = new Vector2i(1920, 1080),
            Title = "EscapeMaze - 3D OpenTK Game",
        };

        using (var game = new Game(GameWindowSettings.Default, nativeWindowSettings))
        {
            game.Run();
        }
    }
}
