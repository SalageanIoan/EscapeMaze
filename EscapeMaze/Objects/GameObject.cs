using OpenTK.Mathematics;
using EscapeMaze.Rendering;

namespace EscapeMaze.Objects;

public class GameObject : IDisposable
{
    private Mesh _mesh;
    private bool _disposed;

    public Vector3 Position { get; set; }
    public Vector3 Rotation { get; set; }
    public Vector3 Scale { get; set; }

    public GameObject(string verticesFilePath, Vector3 position)
    {
        _mesh = new Mesh(verticesFilePath);
        Position = position;
        Rotation = Vector3.Zero;
        Scale = Vector3.One;
    }

    public Matrix4 GetModelMatrix()
    {
        var model = Matrix4.Identity;
        model *= Matrix4.CreateScale(Scale);
        model *= Matrix4.CreateRotationX(Rotation.X);
        model *= Matrix4.CreateRotationY(Rotation.Y);
        model *= Matrix4.CreateRotationZ(Rotation.Z);
        model *= Matrix4.CreateTranslation(Position);
        return model;
    }

    public void Draw()
    {
        _mesh.Draw();
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _mesh?.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
