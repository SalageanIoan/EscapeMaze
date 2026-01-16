using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace EscapeMaze.Input;

public class InputHandler
{
    private Camera _camera;
    private float _cameraSpeed = 1.5f;
    private float _mouseSensitivity = 0.1f;
    private Vector2? _lastMousePosition;

    private bool _wPressed;
    private bool _aPressed;
    private bool _sPressed;
    private bool _dPressed;

    public InputHandler(Camera camera)
    {
        _camera = camera;
    }

    public void ProcessKeyboard(KeyboardState input, float deltaTime)
    {
        if (input.IsKeyDown(Keys.W))
        {
            if (!_wPressed)
            {
                Console.WriteLine("W key pressed - Moving forward");
                _wPressed = true;
            }
            _camera.Position += _camera.Front * _cameraSpeed * deltaTime;
        }
        else if (_wPressed)
        {
            Console.WriteLine("W key released");
            _wPressed = false;
        }

        if (input.IsKeyDown(Keys.S))
        {
            if (!_sPressed)
            {
                Console.WriteLine("S key pressed - Moving backward");
                _sPressed = true;
            }
            _camera.Position -= _camera.Front * _cameraSpeed * deltaTime;
        }
        else if (_sPressed)
        {
            Console.WriteLine("S key released");
            _sPressed = false;
        }

        if (input.IsKeyDown(Keys.A))
        {
            if (!_aPressed)
            {
                Console.WriteLine("A key pressed - Moving left");
                _aPressed = true;
            }
            _camera.Position -= _camera.Right * _cameraSpeed * deltaTime;
        }
        else if (_aPressed)
        {
            Console.WriteLine("A key released");
            _aPressed = false;
        }

        if (input.IsKeyDown(Keys.D))
        {
            if (!_dPressed)
            {
                Console.WriteLine("D key pressed - Moving right");
                _dPressed = true;
            }
            _camera.Position += _camera.Right * _cameraSpeed * deltaTime;
        }
        else if (_dPressed)
        {
            Console.WriteLine("D key released");
            _dPressed = false;
        }
    }

    public void ProcessMouse(float mouseX, float mouseY)
    {
        if (_lastMousePosition == null)
        {
            _lastMousePosition = new Vector2(mouseX, mouseY);
            Console.WriteLine($"Mouse control initialized at position: ({mouseX:F2}, {mouseY:F2})");
        }
        else
        {
            float deltaX = mouseX - _lastMousePosition.Value.X;
            float deltaY = mouseY - _lastMousePosition.Value.Y;
            _lastMousePosition = new Vector2(mouseX, mouseY);

            _camera.Yaw += deltaX * _mouseSensitivity;
            _camera.Pitch -= deltaY * _mouseSensitivity;

            Console.WriteLine($"Mouse moved - Delta: ({deltaX:F2}, {deltaY:F2}), Yaw: {_camera.Yaw:F2}, Pitch: {_camera.Pitch:F2}");
        }
    }
}
