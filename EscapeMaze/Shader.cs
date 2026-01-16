using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace EscapeMaze;

public class Shader : IDisposable
{
    private int _handle;
    private bool _disposed;

    public Shader(string vertexSource, string fragmentSource)
    {
        
        int vertexShader = CompileShader(ShaderType.VertexShader, vertexSource);
        int fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentSource);

        _handle = GL.CreateProgram();
        
        GL.AttachShader(_handle, vertexShader);
        GL.AttachShader(_handle, fragmentShader);
        
        GL.LinkProgram(_handle);
        
        GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out int status);
        if (status == 0)
        {
            string infoLog = GL.GetProgramInfoLog(_handle);
            throw new Exception($"Error linking shader program: {infoLog}");
        }
        
        GL.DetachShader(_handle, vertexShader);
        GL.DetachShader(_handle, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
        
    }

    private int CompileShader(ShaderType type, string source)
    {
        int shader = GL.CreateShader(type);
        GL.ShaderSource(shader, source);
        GL.CompileShader(shader);
        
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);
        if (status == 0)
        {
            string infoLog = GL.GetShaderInfoLog(shader);
            throw new Exception($"Error compiling {type} shader: {infoLog}");
        }
        
        return shader;
    }

    public void Use()
    {
        GL.UseProgram(_handle);
    }

    public void SetMatrix4(string name, Matrix4 matrix)
    {
        int location = GL.GetUniformLocation(_handle, name);
        GL.UniformMatrix4(location, false, ref matrix);
    }

    public void SetVector3(string name, Vector3 vector)
    {
        int location = GL.GetUniformLocation(_handle, name);
        GL.Uniform3(location, vector);
    }

    public void SetFloat(string name, float value)
    {
        int location = GL.GetUniformLocation(_handle, name);
        GL.Uniform1(location, value);
    }

    public void SetInt(string name, int value)
    {
        int location = GL.GetUniformLocation(_handle, name);
        GL.Uniform1(location, value);
    }

    public void SetBool(string name, bool value)
    {
        int location = GL.GetUniformLocation(_handle, name);
        GL.Uniform1(location, value ? 1 : 0);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            GL.DeleteProgram(_handle);
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
