using FezEditor.Structure;
using FEZRepacker.Core.Definitions.Game.Sky;
using Microsoft.Xna.Framework;

namespace FezEditor.Hosts;

public class SkyHost : Host
{
    public sealed override Rid Rid { get; protected set; }
    
    public SkyHost(Game game) : base(game)
    {
        Rid = RenderingService.InstanceCreate(Rid.Invalid);
    }

    ~SkyHost()
    {
        Dispose();
    }
    
    public void Load(Sky sky)
    { 
        throw new NotImplementedException();
    }
}