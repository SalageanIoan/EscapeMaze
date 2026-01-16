using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Globalization;

namespace EscapeMaze.Rendering;

public class Mesh : IDisposable
{
    private int _vao;
    private int _vbo;
    private int _vertexCount;
    private bool _disposed;

    public Mesh(string verticesFilePath)
    {
        LoadVertices(verticesFilePath);
    }

    private void LoadVertices(string filePath)
    {
        float[] vertices;
        
        if (Path.GetExtension(filePath).ToLower() == ".obj")
        {
            vertices = ObjLoader.LoadObj(filePath, new Vector3(0.55f, 0.35f, 0.2f));
        }
        else
        {
            var lines = File.ReadAllLines(filePath);
            var verticesList = new List<float>();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                
                var values = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var value in values)
                {
                    verticesList.Add(float.Parse(value, CultureInfo.InvariantCulture));
                }
            }

            vertices = verticesList.ToArray();
        }
        _vertexCount = vertices.Length / 6;

        _vao = GL.GenVertexArray();
        GL.BindVertexArray(_vao);

        _vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.BindVertexArray(0);

        Console.WriteLine($"Mesh loaded from {Path.GetFileName(filePath)} with {_vertexCount} vertices");
    }

    public void Draw()
    {
        GL.BindVertexArray(_vao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, _vertexCount);
        GL.BindVertexArray(0);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            GL.DeleteBuffer(_vbo);
            GL.DeleteVertexArray(_vao);
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
