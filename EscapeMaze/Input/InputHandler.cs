using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using EscapeMaze.Objects;
using System.Collections.Generic;

namespace EscapeMaze.Input;

public class InputHandler
{
    private Camera _camera;
    private float _cameraSpeed = 1.5f;
    private float _mouseSensitivity = 0.1f;
    private Vector2? _lastMousePosition;

    private float _verticalVelocity;
    private const float Gravity = 15.0f;
    private const float JumpForce = 3.0f;
    private const float PlayerRadius = 0.2f;
    private const float GroundLevel = 0.0f;

    public bool EPressed { get; private set; }

    public InputHandler(Camera camera)
    {
        _camera = camera;
    }

    public void ProcessKeyboard(KeyboardState input, float deltaTime, List<GameObject> walls)
    {
        EPressed = input.IsKeyDown(Keys.E);

        Vector3 front = _camera.Front;
        front.Y = 0;
        front = Vector3.Normalize(front);

        Vector3 right = _camera.Right;
        right.Y = 0;
        right = Vector3.Normalize(right);

        Vector3 moveDir = Vector3.Zero;
        if (input.IsKeyDown(Keys.W)) moveDir += front;
        if (input.IsKeyDown(Keys.S)) moveDir -= front;
        if (input.IsKeyDown(Keys.A)) moveDir -= right;
        if (input.IsKeyDown(Keys.D)) moveDir += right;

        if (moveDir.LengthSquared > 0)
        {
            moveDir = Vector3.Normalize(moveDir);
        }

        Vector3 proposedMove = moveDir * _cameraSpeed * deltaTime;

        if (!CheckCollision(_camera.Position + new Vector3(proposedMove.X, 0, 0), walls))
        {
            _camera.Position += new Vector3(proposedMove.X, 0, 0);
        }

        if (!CheckCollision(_camera.Position + new Vector3(0, 0, proposedMove.Z), walls))
        {
            _camera.Position += new Vector3(0, 0, proposedMove.Z);
        }

        if (input.IsKeyDown(Keys.Space) && _camera.Position.Y <= GroundLevel + 0.05f)
        {
            _verticalVelocity = JumpForce;
        }

        _verticalVelocity -= Gravity * deltaTime;
        _camera.Position += new Vector3(0, _verticalVelocity * deltaTime, 0);

        if (_camera.Position.Y < GroundLevel)
        {
            _camera.Position = new Vector3(_camera.Position.X, GroundLevel, _camera.Position.Z);
            _verticalVelocity = 0;
        }
    }

    private bool CheckCollision(Vector3 targetPos, List<GameObject> walls)
    {
        float minX = targetPos.X - PlayerRadius;
        float maxX = targetPos.X + PlayerRadius;
        float minZ = targetPos.Z - PlayerRadius;
        float maxZ = targetPos.Z + PlayerRadius;

        
        float playerMinY = targetPos.Y - 0.5f;
        float playerMaxY = targetPos.Y + 0.5f;

        foreach (var wall in walls)
        {
            if (wall.IsOpening) continue;

            float wMinX = wall.Position.X - 0.5f;
            float wMaxX = wall.Position.X + 0.5f;
            float wMinZ = wall.Position.Z - 0.5f;
            float wMaxZ = wall.Position.Z + 0.5f;
            float wMinY = wall.Position.Y - 0.5f;
            float wMaxY = wall.Position.Y + 0.5f;

            bool overlapX = maxX > wMinX && minX < wMaxX;
            bool overlapZ = maxZ > wMinZ && minZ < wMaxZ;
            bool overlapY = playerMaxY > wMinY && playerMinY < wMaxY;

            if (overlapX && overlapZ && overlapY)
            {
                return true;
            }
        }
        return false;
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