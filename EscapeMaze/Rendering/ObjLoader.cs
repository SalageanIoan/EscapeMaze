using OpenTK.Mathematics;
using System.Globalization;

namespace EscapeMaze.Rendering;

public static class ObjLoader
{
    public static float[] LoadObj(string filePath)
    {
        var vertices = new List<Vector3>();
        var texCoords = new List<Vector2>();
        var normals = new List<Vector3>();
        var faces = new List<(int v, int vt, int vn)>();

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
            else if (parts[0] == "vt" && parts.Length >= 3)
            {
                float u = float.Parse(parts[1], CultureInfo.InvariantCulture);
                float v = float.Parse(parts[2], CultureInfo.InvariantCulture);
                texCoords.Add(new Vector2(u, v));
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
                var faceVertices = new List<(int v, int vt, int vn)>();
                
                for (int i = 1; i < parts.Length; i++)
                {
                    var indices = parts[i].Split('/');
                    int vIndex = int.Parse(indices[0]) - 1;
                    int vtIndex = indices.Length > 1 && !string.IsNullOrEmpty(indices[1])
                        ? int.Parse(indices[1]) - 1
                        : -1;
                    int vnIndex = indices.Length > 2 && !string.IsNullOrEmpty(indices[2]) 
                        ? int.Parse(indices[2]) - 1 
                        : -1;
                    faceVertices.Add((vIndex, vtIndex, vnIndex));
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
            
            vertexData.Add(vertex.X);
            vertexData.Add(vertex.Y);
            vertexData.Add(vertex.Z);
            
            // Add default color
            vertexData.Add(1.0f);
            vertexData.Add(1.0f);
            vertexData.Add(1.0f);

            if (face.vt >= 0 && face.vt < texCoords.Count)
            {
                var texCoord = texCoords[face.vt];
                vertexData.Add(texCoord.X);
                vertexData.Add(texCoord.Y);
            }
            else
            {
                vertexData.Add(0.0f);
                vertexData.Add(0.0f);
            }
        }

        return vertexData.ToArray();
    }

    public static void ConvertObjToVertexFile(string objFilePath, string outputFilePath)
    {
        var vertexData = LoadObj(objFilePath);
        
        var lines = new List<string>();
        for (int i = 0; i < vertexData.Length; i += 8)
        {
            lines.Add($"{vertexData[i].ToString(CultureInfo.InvariantCulture)} " +
                     $"{vertexData[i + 1].ToString(CultureInfo.InvariantCulture)} " +
                     $"{vertexData[i + 2].ToString(CultureInfo.InvariantCulture)} " +
                     $"{vertexData[i + 3].ToString(CultureInfo.InvariantCulture)} " +
                     $"{vertexData[i + 4].ToString(CultureInfo.InvariantCulture)} " +
                     $"{vertexData[i + 5].ToString(CultureInfo.InvariantCulture)} " +
                     $"{vertexData[i + 6].ToString(CultureInfo.InvariantCulture)} " +
                     $"{vertexData[i + 7].ToString(CultureInfo.InvariantCulture)}");
        }
        
        File.WriteAllLines(outputFilePath, lines);
    }
}

