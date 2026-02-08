using FezEditor.Structure;
using Microsoft.Xna.Framework;

namespace FezEditor.Hosts;

public class NpcHost : Host
{
    public sealed override Rid Rid { get; protected set; }
    
    public string Name { get; set; } = "";
    
    public Vector3 Position { get; set; }
    
    public BoundingBox Bounds { get; set; } = new();
    
    public NpcHost(Game game) : base(game)
    {
        Rid = RenderingService.InstanceCreate(Rid.Invalid);
    }

    ~NpcHost()
    {
        Dispose();
    }

    public override void Load(object asset)
    {
        throw new NotImplementedException();
    }
}