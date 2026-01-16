using OpenTK.Mathematics;

namespace EscapeMaze;

public class Camera
{
    private Vector3 _position;
    private Vector3 _front = -Vector3.UnitZ;
    private Vector3 _up = Vector3.UnitY;
    private Vector3 _right = Vector3.UnitX;

    private float _pitch;
    private float _yaw = -90.0f;
    private float _fov = MathHelper.PiOver4;

    public Camera(Vector3 position, float aspectRatio)
    {
        Position = position;
        AspectRatio = aspectRatio;
        UpdateCameraVectors();
    }

    public Vector3 Position
    {
        get => _position;
        set => _position = value;
    }

    public float AspectRatio { get; set; }

    public Vector3 Front => _front;
    public Vector3 Up => _up;
    public Vector3 Right => _right;

    public float Pitch
    {
        get => _pitch;
        set
        {
            _pitch = Math.Clamp(value, -89.0f, 89.0f);
            UpdateCameraVectors();
        }
    }

    public float Yaw
    {
        get => _yaw;
        set
        {
            _yaw = value;
            UpdateCameraVectors();
        }
    }

    public float Fov
    {
        get => MathHelper.RadiansToDegrees(_fov);
        set
        {
            var angle = Math.Clamp(value, 1f, 90f);
            _fov = MathHelper.DegreesToRadians(angle);
        }
    }

    public Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt(Position, Position + _front, _up);
    }

    public Matrix4 GetProjectionMatrix()
    {
        return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 100f);
    }

    private void UpdateCameraVectors()
    {
        _front.X = MathF.Cos(MathHelper.DegreesToRadians(_pitch)) * MathF.Cos(MathHelper.DegreesToRadians(_yaw));
        _front.Y = MathF.Sin(MathHelper.DegreesToRadians(_pitch));
        _front.Z = MathF.Cos(MathHelper.DegreesToRadians(_pitch)) * MathF.Sin(MathHelper.DegreesToRadians(_yaw));

        _front = Vector3.Normalize(_front);
        _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
        _up = Vector3.Normalize(Vector3.Cross(_right, _front));
    }
}
