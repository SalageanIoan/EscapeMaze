using OpenTK.Mathematics;
using EscapeMaze.Objects;
using System.Collections.Generic;

namespace EscapeMaze.Scenes;

public static class MazeGenerator
{
    public static List<GameObject> GenerateMaze(int[,] mazeLayout)
    {
        List<GameObject> walls = new List<GameObject>();

        int rows = mazeLayout.GetLength(0);
        int cols = mazeLayout.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (mazeLayout[row, col] > 0)
                {
                    float x = -4.5f + col * 1.0f;
                    float z = -4.5f + row * 1.0f;
                    string texturePath = mazeLayout[row, col] == 2 ? "Data/UI/door.png" : "Data/UI/wall.png";
                    walls.Add(new GameObject("Data/cube_vertices.txt", new Vector3(x, 0, z), texturePath));
                }
            }
        }

        return walls;
    }
}
