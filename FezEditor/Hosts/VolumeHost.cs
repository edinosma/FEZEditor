using FezEditor.Structure;
using Microsoft.Xna.Framework;

namespace FezEditor.Hosts;

public class VolumeHost : Host
{
    public sealed override Rid Rid { get; protected set; }
    
    public Vector3 From { get; set; }
    
    public Vector3 To { get; set; }
    
    public BoundingBox Bounds { get; set; }
    
    public VolumeHost(Game game) : base(game)
    {
        Rid = RenderingService.InstanceCreate(Rid.Invalid);
    }

    ~VolumeHost()
    {
        Dispose();
    }
}