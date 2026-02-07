using Microsoft.Xna.Framework;

namespace FezEditor.Structure;

public class MeshSurface
{
    public required Vector3[] Vertices;
    public required int[] Indices;
    public Vector3[]? Normals;
    public Color[]? Colors;
    public Vector2[]? TexCoords;
}