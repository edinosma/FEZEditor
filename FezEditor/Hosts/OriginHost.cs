using FezEditor.Structure;
using Microsoft.Xna.Framework;

namespace FezEditor.Hosts;

public class OriginHost : Host
{
    public sealed override Rid Rid { get; protected set; }
    
    public bool Enabled { get; set; } = true;
    
    public Color AxisXColor { get; set; } = new(0.96f, 0.20f, 0.32f);

    public Color AxisYColor { get; set; } = new(0.53f, 0.84f, 0.01f);

    public Color AxisZColor { get; set; } = new(0.16f, 0.55f, 0.96f);
    
    public OriginHost(Game game) : base(game)
    {
        Rid = RenderingService.InstanceCreate(Rid.Invalid);
    }

    ~OriginHost()
    {
        Dispose();
    }
    
    public override void Load(object asset)
    {
        throw new NotImplementedException();
    }
}