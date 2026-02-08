using FezEditor.Structure;
using Microsoft.Xna.Framework;

namespace FezEditor.Hosts;

public class CursorHost : Host
{
    public sealed override Rid Rid { get; protected set; }
    
    public Vector3 Position { get; set; }
    
    public Quaternion Rotation { get; set; }
    
    public CursorHost(Game game) : base(game)
    {
        Rid = RenderingService.InstanceCreate(Rid.Invalid);
    }

    ~CursorHost()
    {
        Dispose();
    }
}