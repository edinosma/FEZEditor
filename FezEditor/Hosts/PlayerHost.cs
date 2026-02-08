using FezEditor.Structure;
using Microsoft.Xna.Framework;

namespace FezEditor.Hosts;

public class PlayerHost : Host
{
    public sealed override Rid Rid { get; protected set; }
    
    public Vector3 Position { get; set; } = Vector3.Zero;

    public Quaternion Rotation { get; set; } = Quaternion.Identity;
    
    public BoundingBox Bounds { get; set; }
    
    public PlayerHost(Game game) : base(game)
    {
        Rid = RenderingService.InstanceCreate(Rid.Invalid);
    }

    ~PlayerHost()
    {
        Dispose();
    }

    public override void Load(object asset)
    {
        throw new NotImplementedException();
    }
}