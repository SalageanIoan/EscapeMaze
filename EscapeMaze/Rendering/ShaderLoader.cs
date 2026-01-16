namespace EscapeMaze.Rendering;

public static class ShaderLoader
{
    public static string LoadShaderSource(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Shader file not found: {filePath}");
        }

        string source = File.ReadAllText(filePath);
        Console.WriteLine($"Shader loaded from {Path.GetFileName(filePath)}");
        return source;
    }
}
