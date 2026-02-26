using FezEditor.Structure;
using FezEditor.Tools;
using FEZRepacker.Core.Definitions.Game.Common;
using FEZRepacker.Core.Definitions.Game.TrileSet;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FezEditor.Components;

public class TrileSubject : ITrixelSubject
{
    private const int TextureWidth = 108;

    private const int TextureHeight = 18;
    
    public string TextureExportKey => $"{_set.Name}#{_id}";

    public int Id
    {
        get => _id;
        set
        {
            if (_id != value)
            {
                if (!_set.Triles.ContainsKey(_id))
                {
                    throw new KeyNotFoundException($"Trile with ID {_id} not found");
                }

                _id = value;
            }
        }
    }
    
    private Trile Trile => _set.Triles[_id];
    
    private readonly TrileSet _set;
    
    private int _id;
    
    private Action<Vector3>? _resized;

    public TrileSubject(TrileSet set)
    {
        _set = set;
        _id = set.Triles
            .OrderBy(kv => kv.Key)
            .First(kv => kv.Value.Geometry.Vertices.Length > 0)
            .Key;
    }
    
    public TrixelObject Materialize()
    {
        var obj = TrixelMaterializer.ReconstructGeometry(Trile.Size.ToXna(), Trile.Geometry.Vertices, Trile.Geometry.Indices);
        _resized = obj.Resize;
        return obj;
    }

    public object GetAsset(TrixelObject obj)
    {
        var trile = new Trile { Size = obj.Size.ToRepacker() };
        (trile.Geometry.Vertices, trile.Geometry.Indices) = TrixelMaterializer.Dematerialize(obj);
        trile.Offset = Trile.Offset;
        trile.Immaterial = Trile.Immaterial;
        trile.SeeThrough = Trile.SeeThrough;
        trile.Thin = Trile.Thin;
        trile.ForceHugging = Trile.ForceHugging;
        trile.Type = Trile.Type;
        trile.Face = Trile.Face;
        trile.SurfaceType = Trile.SurfaceType;
        trile.Faces = new Dictionary<FaceOrientation, CollisionType>(Trile.Faces);
        return trile;
    }

    public Texture2D LoadTexture(GraphicsDevice gd)
    {
        var atlas = _set.TextureAtlas;
        var px = (int)MathF.Round(Trile.AtlasOffset.X * atlas.Width);
        var py = (int)MathF.Round(Trile.AtlasOffset.Y * atlas.Height);
        
        var slice = new byte[TextureWidth * TextureHeight * 4];
        for (var row = 0; row < TextureHeight; row++)
        {
            var src = ((py + row) * atlas.Width + px) * 4;
            Buffer.BlockCopy(atlas.TextureData, src, slice, row * TextureWidth * 4, TextureWidth * 4);
        }
        
        var tex2D = new Texture2D(gd, TextureWidth, TextureHeight, false, SurfaceFormat.Color);
        tex2D.SetData(slice);
        RepackerExtensions.SetAlpha(tex2D, 1f);
        
        return tex2D;
    }

    public void UpdateTexture(Texture2D texture)
    {
        var atlas = _set.TextureAtlas;
        var pixels = new byte[texture.Width * texture.Height * 4];
        texture.GetData(pixels);
        
        var px = (int)MathF.Round(Trile.AtlasOffset.X * atlas.Width);
        var py = (int)MathF.Round(Trile.AtlasOffset.Y * atlas.Height);
        for (var row = 0; row < texture.Height; row++)
        {
            var dst = ((py + row) * atlas.Width + px) * 4;
            Buffer.BlockCopy(pixels, row * texture.Width * 4, atlas.TextureData, dst, texture.Width * 4);
        }
    }

    public void DrawProperties(History history)
    {
        var name = Trile.Name;
        if (ImGui.InputText("Name", ref name, 255))
        {
            using (history.BeginScope("Edit Name")) 
            {
                Trile.Name = name;
            }
        }
        
        var size = Trile.Size.ToXna();
        if (ImGuiX.DragFloat3("Size", ref size))
        {
            using (history.BeginScope("Edit Size")) 
            {
                Trile.Size = size.ToRepacker();
                _resized?.Invoke(size);
            }
        }
        
        var offset = Trile.Offset.ToXna();
        if (ImGuiX.DragFloat3("Offset", ref offset))
        {
            using (history.BeginScope("Edit Offset")) 
            {
                Trile.Offset = offset.ToRepacker();
            }
        }
        
        var immaterial = Trile.Immaterial;
        if (ImGui.Checkbox("Immaterial", ref immaterial))
        {
            using (history.BeginScope("Edit Immaterial")) 
            {
                Trile.Immaterial = immaterial;
            }
        }
        
        var seeThrough = Trile.SeeThrough;
        if (ImGui.Checkbox("See Through", ref seeThrough))
        {
            using (history.BeginScope("Edit See Through")) 
            {
                Trile.SeeThrough = seeThrough;
            }
        }
        
        var thin = Trile.Thin;
        if (ImGui.Checkbox("Thin", ref thin))
        {
            using (history.BeginScope("Edit Thin")) 
            {
                Trile.Thin = thin;
            }
        }
        
        var forceHugging = Trile.ForceHugging;
        if (ImGui.Checkbox("Force Hugging", ref forceHugging))
        {
            using (history.BeginScope("Edit Force Hugging")) 
            {
                Trile.ForceHugging = forceHugging;
            }
        }
        
        var actorType = (int)Trile.Type;
        var actorTypes = Enum.GetNames<ActorType>();
        if (ImGui.Combo("Actor Type", ref actorType, actorTypes, actorTypes.Length))
        {
            using (history.BeginScope("Edit Actor Type"))
            {
                Trile.Type = (ActorType)actorType;
            }
        }
        
        var actorFace = (int)Trile.Face;
        var actorFaces = Enum.GetNames<ActorType>();
        if (ImGui.Combo("Actor Face", ref actorFace, actorFaces, actorFaces.Length))
        {
            using (history.BeginScope("Edit Actor Face")) 
            {
                Trile.Face = (FaceOrientation)actorFace;
            }
        }
        
        var surfaceType = (int)Trile.SurfaceType;
        var surfaceTypes = Enum.GetNames<SurfaceType>();
        if (ImGui.Combo("Surface Type", ref surfaceType, surfaceTypes, surfaceTypes.Length))
        {
            using (history.BeginScope("Edit Surface Type")) 
            {
                Trile.SurfaceType = (SurfaceType)surfaceType;
            }
        }
        
        // FaceOrientation is not IEquatable, so string key is being used
        var collisionFaces = Trile.Faces.ToDictionary(kv => kv.Key.ToString(), kv => kv.Value);
        if (ImGuiX.EditableDict("Collision Faces", ref collisionFaces, RenderFace, AddCollisionType, () => CollisionType.None))
        {
            using (history.BeginScope("Edit Collision Faces")) 
            {
                Trile.Faces = collisionFaces.ToDictionary(kv => Enum.Parse<FaceOrientation>(kv.Key), kv => kv.Value);
            }
        }
    }

    private static bool RenderFace(string key, ref CollisionType value)
    {
        ImGui.Text(key + ":");
        ImGui.SameLine();
        var collisionType = (int)value;
        var collisionTypes = Enum.GetNames<CollisionType>();
        return ImGui.Combo($"##{key}_value", ref collisionType, collisionTypes, collisionTypes.Length);
    }
    
    private static bool AddCollisionType(ref string key)
    {
        if (string.IsNullOrEmpty(key)) key = nameof(FaceOrientation.Left);
        var face = (int)Enum.Parse<FaceOrientation>(key);
        var faces = Enum.GetNames<FaceOrientation>();
        return ImGui.Combo("##item", ref face, faces, faces.Length);
    }
}