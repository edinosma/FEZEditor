using FezEditor.Structure;
using FEZRepacker.Core.Definitions.Game.Level;
using Microsoft.Xna.Framework;

namespace FezEditor.Hosts;

public class LiquidHost : Host
{
    public sealed override Rid Rid { get; protected set; }

    public LiquidType Type { get; set; }
    
    public float Height { get; set; }
    
    public LiquidHost(Game game) : base(game)
    {
        Rid = RenderingService.InstanceCreate(Rid.Invalid);
    }

    ~LiquidHost()
    {
        Dispose();
    }
}