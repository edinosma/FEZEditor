using FezEditor.Structure;
using FEZRepacker.Core.Definitions.Game.ArtObject;
using FEZRepacker.Core.Definitions.Game.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RVector2 = FEZRepacker.Core.Definitions.Game.XNA.Vector2;
using RVector3 = FEZRepacker.Core.Definitions.Game.XNA.Vector3;
using RVector4 = FEZRepacker.Core.Definitions.Game.XNA.Vector4;
using RQuaternion = FEZRepacker.Core.Definitions.Game.XNA.Quaternion;
using RColor = FEZRepacker.Core.Definitions.Game.XNA.Color;
using RTexture2D = FEZRepacker.Core.Definitions.Game.XNA.Texture2D;
using RAnimatedTexture = FEZRepacker.Core.Definitions.Game.Graphics.AnimatedTexture;
using RRectangle = FEZRepacker.Core.Definitions.Game.XNA.Rectangle;

namespace FezEditor.Tools;

public static class RepackerConversions
{
    public static MeshSurface ToXna<T>(this IndexedPrimitives<VertexInstance, T> geometry)
    {
        return new MeshSurface
        {
            Vertices = geometry.Vertices.Select(i => i.Position.ToXna()).ToArray(),
            Normals = geometry.Vertices.Select(i => i.Normal.ToXna()).ToArray(),
            TexCoords = geometry.Vertices.Select(i => i.TextureCoordinate.ToXna()).ToArray(),
            Indices = geometry.Indices.Select(i => (int)i).ToArray()
        };
    }

    public static Texture2D ToXna(this RTexture2D texture, GraphicsDevice gd)
    {
        var tex2D = new Texture2D(gd, texture.Width, texture.Height, false, SurfaceFormat.Color);
        var data = new byte[texture.TextureData.Length];
        Array.Copy(texture.TextureData, data, texture.TextureData.Length);
        tex2D.SetData(data);
        return tex2D;
    }

    public static Texture2D ToXna(this RAnimatedTexture texture, GraphicsDevice gd)
    {
        var tex2D = new Texture2D(gd, texture.AtlasWidth, texture.AtlasHeight, false, SurfaceFormat.Color);
        var data = new byte[texture.TextureData.Length];
        Array.Copy(texture.TextureData, data, texture.TextureData.Length);
        tex2D.SetData(data);
        return tex2D;
    }
    
    public static Vector2 ToXna(this RVector2 v) => new(v.X, v.Y);
    public static Vector3 ToXna(this RVector3 v) => new(v.X, v.Y, v.Z);
    public static Vector4 ToXna(this RVector4 v) => new(v.X, v.Y, v.Z, v.W);
    public static Quaternion ToXna(this RQuaternion q) => new(q.X, q.Y, q.Z, q.W);
    public static Color ToXna(this RColor c) => new(c.R, c.G, c.B, c.A);
    public static Rectangle ToXna(this RRectangle r) => new(r.X, r.Y, r.Width, r.Height);
    
    public static RVector2 ToRepacker(this Vector2 v) => new(v.X, v.Y);
    public static RVector3 ToRepacker(this Vector3 v) => new(v.X, v.Y, v.Z);
    public static RVector4 ToRepacker(this Vector4 v) => new(v.X, v.Y, v.Z, v.W);
    public static RQuaternion ToRepacker(this Quaternion q) => new(q.X, q.Y, q.Z, q.W);
    public static RColor ToRepacker(this Color c) => new(c.R, c.G, c.B, c.A);
    public static RRectangle ToRepacker(this Rectangle r) => new(r.X, r.Y, r.Width, r.Height);
}