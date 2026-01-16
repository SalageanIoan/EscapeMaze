using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace EscapeMaze.Rendering;

public class Button : IDisposable
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public bool IsHovered { get; private set; }

    private int _textureId;
    private int _hoverTextureId;
    private int _vao;
    private int _vbo;
    private Shader? _shader;

    public Button(string normalTexturePath, string hoverTexturePath, float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;

        _textureId = LoadTexture(normalTexturePath);
        _hoverTextureId = LoadTexture(hoverTexturePath);

        SetupBuffers();
    }

    private void SetupBuffers()
    {
        string vertexShader = @"
#version 330 core
layout (location = 0) in vec2 aPosition;
layout (location = 1) in vec2 aTexCoord;

out vec2 TexCoord;
uniform mat4 projection;

void main()
{
    gl_Position = projection * vec4(aPosition, 0.0, 1.0);
    TexCoord = aTexCoord;
}";

        string fragmentShader = @"
#version 330 core
in vec2 TexCoord;
out vec4 FragColor;

uniform sampler2D texture0;

void main()
{
    FragColor = texture(texture0, TexCoord);
}";

        _shader = new Shader(vertexShader, fragmentShader);

        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();

        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 6 * 4, IntPtr.Zero, BufferUsageHint.DynamicDraw);

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);

        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));

        GL.BindVertexArray(0);
    }

    private int LoadTexture(string path)
    {
        int texture = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, texture);

        using (var image = Image.Load<Rgba32>(path))
        {
            image.Mutate(x => x.Flip(FlipMode.Vertical));

            var pixels = new byte[4 * image.Width * image.Height];
            image.CopyPixelDataTo(pixels);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0,
                PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
        }

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

        return texture;
    }

    public void UpdateHoverState(float mouseX, float mouseY)
    {
        IsHovered = mouseX >= X && mouseX <= X + Width &&
                    mouseY >= Y && mouseY <= Y + Height;
    }

    public bool IsClicked(float mouseX, float mouseY)
    {
        return IsHovered;
    }

    public void Render(int screenWidth, int screenHeight)
    {
        if (_shader == null) return;

        Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0, screenWidth, screenHeight, 0, -1, 1);

        _shader.Use();
        _shader.SetMatrix4("projection", projection);

        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Disable(EnableCap.DepthTest);

        int textureToUse = IsHovered ? _hoverTextureId : _textureId;
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, textureToUse);

        float[] vertices = {
            X,         Y + Height, 0.0f, 0.0f,
            X,         Y,          0.0f, 1.0f,
            X + Width, Y,          1.0f, 1.0f,

            X,         Y + Height, 0.0f, 0.0f,
            X + Width, Y,          1.0f, 1.0f,
            X + Width, Y + Height, 1.0f, 0.0f
        };

        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, vertices.Length * sizeof(float), vertices);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

        GL.BindVertexArray(0);
        GL.Enable(EnableCap.DepthTest);
        GL.Disable(EnableCap.Blend);
    }

    public void Dispose()
    {
        GL.DeleteTexture(_textureId);
        GL.DeleteTexture(_hoverTextureId);
        GL.DeleteVertexArray(_vao);
        GL.DeleteBuffer(_vbo);
        _shader?.Dispose();
    }
}
