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

    public bool EPressed { get; private set; }

    public InputHandler(Camera camera)
    {
        _camera = camera;
    }

    public void ProcessKeyboard(KeyboardState input, float deltaTime)
    {
        EPressed = input.IsKeyDown(Keys.E);

        if (input.IsKeyDown(Keys.W))
        {
            if (!_wPressed)
            {
                _wPressed = true;
            }
            _camera.Position += _camera.Front * _cameraSpeed * deltaTime;
        }
        else if (_wPressed)
        {
            _wPressed = false;
        }

        if (input.IsKeyDown(Keys.S))
        {
            if (!_sPressed)
            {
                _sPressed = true;
            }
            _camera.Position -= _camera.Front * _cameraSpeed * deltaTime;
        }
        else if (_sPressed)
        {
            _sPressed = false;
        }

        if (input.IsKeyDown(Keys.A))
        {
            if (!_aPressed)
            {
                _aPressed = true;
            }
            _camera.Position -= _camera.Right * _cameraSpeed * deltaTime;
        }
        else if (_aPressed)
        {
            _aPressed = false;
        }

        if (input.IsKeyDown(Keys.D))
        {
            if (!_dPressed)
            {
                _dPressed = true;
            }
            _camera.Position += _camera.Right * _cameraSpeed * deltaTime;
        }
        else if (_dPressed)
        {
            _dPressed = false;
        }
    }

    public void ProcessMouse(float mouseX, float mouseY)
    {
        if (_lastMousePosition == null)
        {
            _lastMousePosition = new Vector2(mouseX, mouseY);
        }
        else
        {
            float deltaX = mouseX - _lastMousePosition.Value.X;
            float deltaY = mouseY - _lastMousePosition.Value.Y;
            _lastMousePosition = new Vector2(mouseX, mouseY);

            _camera.Yaw += deltaX * _mouseSensitivity;
            _camera.Pitch -= deltaY * _mouseSensitivity;
        }
    }
}
