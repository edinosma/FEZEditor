using FezEditor.Structure;
using FEZRepacker.Core.Definitions.Game.MapTree;
using Microsoft.Xna.Framework;

namespace FezEditor.Hosts;

public class MapNodeHost : Host
{
    public sealed override Rid Rid { get; protected set; }
    
    public Vector3 Position { get; set; }
    
    public BoundingBox Bounds { get; set; } = new();
    
    public bool Hightlighted { get; set; }
    
    public MapNodeHost(Game game) : base(game)
    {
        Rid = RenderingService.InstanceCreate(Rid.Invalid);
    }

    ~MapNodeHost()
    {
        Dispose();
    }
    
    public override void Load(object asset)
    {
        throw new NotImplementedException();
    }
    
    public void AddChild(MapNodeHost child, Vector3 position)
    {
        throw new NotImplementedException();
    }

    public void CreateLink(MapNodeConnection connection)
    {
        throw new NotImplementedException();
    }

    public void AddLinkBranch(Vector3 position, Vector3 scale)
    {
        throw new NotImplementedException();
    }

    public void UpdateMapIcons(MapNode node)
    {
        throw new NotImplementedException();
    }
}