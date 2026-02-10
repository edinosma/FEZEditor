using FezEditor.Structure;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FezEditor.Hosts;

public class GeometryHost : Host
{
    public sealed override Rid Rid { get; protected set; }
    
    public CameraHost Camera { get; }
    
    public ArtObjectHost ArtObject { get; }
    
    public GridHost Grid { get; }

    private readonly FirstPersonControl _firstPersonControl;
    
    private readonly Rid _renderTarget;

    private readonly Rid _root;
    
    public GeometryHost(Game game) : base(game)
    {
        Rid = RenderingService.WorldCreate();
        _root = RenderingService.WorldGetRoot(Rid);
        
        _renderTarget = RenderingService.RenderTargetCreate();
        RenderingService.RenderTargetSetWorld(_renderTarget, Rid);
        RenderingService.RenderTargetSetClearColor(_renderTarget, Color.Black);

        Camera = new CameraHost(Game)
        {
            Projection = CameraHost.ProjectionType.Perspective,
            FieldOfView = 90.0f
        };
        RenderingService.WorldSetCamera(Rid, Camera.Rid);

        // Enable fog for grid fade-out
        RenderingService.WorldSetFogType(Rid, FogType.Exponential2);
        RenderingService.WorldSetFogColor(Rid, Color.Black);
        RenderingService.WorldSetFogDensity(Rid, 0.015f);

        _firstPersonControl = new FirstPersonControl(Game)
        {
            Camera = Camera
        };

        ArtObject = new ArtObjectHost(Game);
        RenderingService.InstanceSetParent(ArtObject.Rid, _root);

        Grid = new GridHost(Game);
        RenderingService.InstanceSetParent(Grid.Rid, _root);
    }
    
    ~GeometryHost()
    {
        Dispose();
    }

    public override void Load(object asset)
    {
        ArtObject.Load(asset);
    }

    public override void Update(GameTime gameTime)
    {
        _firstPersonControl.Update(gameTime);
        ArtObject.Update(gameTime);
        Grid.Update(gameTime);
    }
    
    public void SetViewportSize(int width, int height)
    {
        RenderingService.RenderTargetSetSize(_renderTarget, width, height);
    }

    public Texture2D? GetViewportTexture()
    {
        return RenderingService.RenderTargetGetTexture(_renderTarget);
    }

    public override void Dispose()
    {
        RenderingService.FreeRid(_root);
        RenderingService.FreeRid(_renderTarget);
        base.Dispose();
    }
}