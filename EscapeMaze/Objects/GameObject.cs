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
    public float Alpha { get; set; }
    public bool IsRotating { get; set; }
    public bool IsFadingOut { get; set; }
    public Texture? Texture { get; set; }

    public GameObject(string verticesFilePath, Vector3 position, string texturePath)
    {
        _mesh = new Mesh(verticesFilePath);
        Position = position;
        Rotation = Vector3.Zero;
        Scale = Vector3.One;
        Alpha = 1.0f;
        IsRotating = false;
        IsFadingOut = false;
        Texture = new Texture(texturePath);
    }

    public GameObject(string verticesFilePath, Vector3 position)
    {
        _mesh = new Mesh(verticesFilePath);
        Position = position;
        Rotation = Vector3.Zero;
        Scale = Vector3.One;
        Alpha = 1.0f;
        IsRotating = false;
        IsFadingOut = false;
        Texture = null;
    }

    public void Update(float deltaTime)
    {
        if (IsRotating)
        {
            Rotation = new Vector3(Rotation.X, Rotation.Y + deltaTime, Rotation.Z);
        }

        if (IsFadingOut)
        {
            Alpha -= deltaTime * 0.5f;
            if (Alpha < 0)
            {
                Alpha = 0;
            }
        }
    }

    public Matrix4 GetModelMatrix()
    {
        Matrix4 model = Matrix4.CreateScale(Scale);
        model = Matrix4.CreateRotationX(Rotation.X) * Matrix4.CreateRotationY(Rotation.Y) * Matrix4.CreateRotationZ(Rotation.Z) * model;
        model = Matrix4.CreateTranslation(Position) * model;
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
            Texture?.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
