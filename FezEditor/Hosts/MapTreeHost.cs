using FezEditor.Structure;
using FEZRepacker.Core.Definitions.Game.MapTree;
using Microsoft.Xna.Framework;

namespace FezEditor.Hosts;

public class MapTreeHost : Host
{
    public sealed override Rid Rid { get; protected set; }
    
    public CameraHost Camera { get; }
    
    public IReadOnlyDictionary<MapNode, MapNodeHost> Nodes { get; }
    
    private readonly Rid _renderTarget;

    private readonly Rid _root;
    
    public MapTreeHost(Game game) : base(game)
    {
        Rid = RenderingService.WorldCreate();
        _renderTarget = RenderingService.RenderTargetCreate();
        _root = RenderingService.WorldGetRoot(Rid);
    }

    ~MapTreeHost()
    {
        Dispose();
    }
    
    public override void Load(object asset)
    {
        throw new NotImplementedException();
    }
    
    public void Highlight(MapNode nodeToHightlight, bool highlight)
    {
        throw new NotImplementedException();
    }

    public void RefreshSubTree(MapNode node)
    {
        throw new NotImplementedException();
    }
}