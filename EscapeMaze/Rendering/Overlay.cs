using OpenTK.Graphics.OpenGL4;

namespace EscapeMaze.Rendering;

public class Overlay : IDisposable
{
    private int _vao;
    private int _vbo;
    private Shader? _shader;

    public Overlay()
    {
        Initialize();
    }

    private void Initialize()
    {
        string vertexShaderSource = ShaderLoader.LoadShaderSource("Shaders/overlay_vertex_shader.txt");
        string fragmentShaderSource = ShaderLoader.LoadShaderSource("Shaders/overlay_fragment_shader.txt");
        _shader = new Shader(vertexShaderSource, fragmentShaderSource);

        float[] vertices = {
            -1.0f,  1.0f,
            -1.0f, -1.0f,
             1.0f, -1.0f,
            -1.0f,  1.0f,
             1.0f, -1.0f,
             1.0f,  1.0f
        };

        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.BindVertexArray(0);
    }

    public void Render()
    {
        if (_shader == null) return;
        
        _shader.Use();
        GL.BindVertexArray(_vao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        GL.BindVertexArray(0);
    }

    public void Dispose()
    {
        GL.DeleteVertexArray(_vao);
        GL.DeleteBuffer(_vbo);
        _shader?.Dispose();
    }
}
