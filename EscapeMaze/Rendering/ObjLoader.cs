using OpenTK.Mathematics;
using System.Globalization;

namespace EscapeMaze.Rendering;

public static class ObjLoader
{
    public static float[] LoadObj(string filePath, Vector3 defaultColor)
    {
        var vertices = new List<Vector3>();
        var normals = new List<Vector3>();
        var faces = new List<(int v, int vn)>();

        var lines = File.ReadAllLines(filePath);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue;

            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                continue;

            if (parts[0] == "v" && parts.Length >= 4)
            {
                float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
                float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
                float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
                vertices.Add(new Vector3(x, y, z));
            }
            else if (parts[0] == "vn" && parts.Length >= 4)
            {
                float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
                float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
                float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
                normals.Add(new Vector3(x, y, z));
            }
            else if (parts[0] == "f" && parts.Length >= 4)
            {
                var faceVertices = new List<(int v, int vn)>();
                
                for (int i = 1; i < parts.Length; i++)
                {
                    var indices = parts[i].Split('/');
                    int vIndex = int.Parse(indices[0]) - 1;
                    int vnIndex = indices.Length > 2 && !string.IsNullOrEmpty(indices[2]) 
                        ? int.Parse(indices[2]) - 1 
                        : -1;
                    faceVertices.Add((vIndex, vnIndex));
                }

                for (int i = 1; i < faceVertices.Count - 1; i++)
                {
                    faces.Add(faceVertices[0]);
                    faces.Add(faceVertices[i]);
                    faces.Add(faceVertices[i + 1]);
                }
            }
        }

        var vertexData = new List<float>();
        
        foreach (var face in faces)
        {
            var vertex = vertices[face.v];
            
            vertex *= 0.01f;
            
            vertexData.Add(vertex.X);
            vertexData.Add(vertex.Y);
            vertexData.Add(vertex.Z);
            
            Vector3 color = defaultColor;
            if (face.vn >= 0 && face.vn < normals.Count)
            {
                var normal = normals[face.vn];
                color = new Vector3(
                    Math.Abs(normal.X),
                    Math.Abs(normal.Y),
                    Math.Abs(normal.Z)
                );
            }
            
            vertexData.Add(color.X);
            vertexData.Add(color.Y);
            vertexData.Add(color.Z);
        }

        Console.WriteLine($"OBJ loaded: {vertices.Count} vertices, {faces.Count} face vertices");
        
        return vertexData.ToArray();
    }

    public static void ConvertObjToVertexFile(string objFilePath, string outputFilePath, Vector3 defaultColor)
    {
        var vertexData = LoadObj(objFilePath, defaultColor);
        
        var lines = new List<string>();
        for (int i = 0; i < vertexData.Length; i += 6)
        {
            lines.Add($"{vertexData[i].ToString(CultureInfo.InvariantCulture)} " +
                     $"{vertexData[i + 1].ToString(CultureInfo.InvariantCulture)} " +
                     $"{vertexData[i + 2].ToString(CultureInfo.InvariantCulture)} " +
                     $"{vertexData[i + 3].ToString(CultureInfo.InvariantCulture)} " +
                     $"{vertexData[i + 4].ToString(CultureInfo.InvariantCulture)} " +
                     $"{vertexData[i + 5].ToString(CultureInfo.InvariantCulture)}");
        }
        
        File.WriteAllLines(outputFilePath, lines);
        Console.WriteLine($"Converted OBJ to vertex file: {outputFilePath}");
    }
}
