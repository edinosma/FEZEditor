using Microsoft.Xna.Framework;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;

namespace FezEditor.Structure;

/// <summary>
/// Vertex data container for a mesh surface.
/// Only Vertices and Indices are required. Other arrays are optional
/// and must match the Vertices length when provided.
/// </summary>
public readonly struct MeshSurface
{
    public Vector3[] Vertices { get; init; }
    
    public Vector3[] Normals { get; init; }
    
    public Color[] Colors { get; init; }
    
    public Vector2[] TexCoords { get; init; }
    
    public int[] Indices { get; init; }
}