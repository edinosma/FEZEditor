using FezEditor.Structure;
using Microsoft.Xna.Framework;

namespace FezEditor.Hosts;

public class GridHost : Host
{
    public sealed override Rid Rid { get; protected set; }

    public bool Enabled { get; set; } = true;

    public GridPlane Plane { get; set; } = GridPlane.Xz;

    public Color PrimaryColor { get; set; } = new(0.5f, 0.5f, 0.5f);

    public Color SecondaryColor { get; set; } = new(0.3f, 0.3f, 0.3f, 0.8f);

    public int Size { get; set; } = 100;

    public int PrimarySteps { get; set; } = 10;

    public float DivisionLevelBias { get; set; } = -0.2f;

    public int DivisionLevelMax { get; set; } = 2;

    public int DivisionLevelMin { get; set; }
    
    public GridHost(Game game) : base(game)
    {
        Rid = RenderingService.InstanceCreate(Rid.Invalid);
    }

    ~GridHost()
    {
        Dispose();
    }
}