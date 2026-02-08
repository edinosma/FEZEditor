using FezEditor.Structure;
using FEZRepacker.Core.Definitions.Game.Level;
using Microsoft.Xna.Framework;

namespace FezEditor.Hosts;

public class LevelHost : Host
{
    public sealed override Rid Rid { get; protected set; }
    
    public CameraHost Camera { get; }
    
    public PlayerHost Player { get; }
    
    public SkyHost Sky { get; }
    
    public LiquidHost Liquid { get; }
    
    public GridHost Grid { get; }
    
    public OriginHost Origin { get; }
    
    public CursorHost Cursor { get; }

    public TrilesHost Triles { get; }
    
    public IReadOnlyDictionary<int, ArtObjectHost> ArtObjects { get; }
    
    public IReadOnlyDictionary<int, BackgroundPlane> BackgroundPlans { get; }
    
    public IReadOnlyDictionary<int, NpcHost> Npcs { get; }
    
    public IReadOnlyDictionary<int, VolumeHost> Volumes { get; }
    
    private readonly Rid _renderTarget;

    private readonly Rid _root;
    
    public LevelHost(Game game) : base(game)
    {
        Rid = RenderingService.WorldCreate();
        _renderTarget = RenderingService.RenderTargetCreate();
        _root = RenderingService.WorldGetRoot(Rid);
    }

    ~LevelHost()
    {
        Dispose();
    }
}