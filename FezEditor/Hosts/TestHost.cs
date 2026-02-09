using FezEditor.Structure;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FezEditor.Hosts;

public class TestHost : Host
{
    public sealed override Rid Rid { get; protected set; }

    public CameraHost Camera { get; }

    private readonly Rid _renderTarget;

    private readonly Rid _root;

    private readonly Rid _material;

    private readonly Rid _mesh;

    private float _elapsed;

    public TestHost(Game game) : base(game)
    {
        Rid = RenderingService.WorldCreate();
        _root = RenderingService.WorldGetRoot(Rid);

        _renderTarget = RenderingService.RenderTargetCreate();
        RenderingService.RenderTargetSetWorld(_renderTarget, Rid);
        RenderingService.RenderTargetSetClearColor(_renderTarget, Color.Black);

        Camera = new CameraHost(Game)
        {
            Projection = CameraHost.ProjectionType.Orthographic,
            Size = 4f,
            Position = new Vector3(0f, 0f, -4f),
            Rotation = Quaternion.Identity
        };
        RenderingService.WorldSetCamera(Rid, Camera.Rid);

        var effect = new BasicEffect(Game.GraphicsDevice) { VertexColorEnabled = true };
        _material = RenderingService.MaterialCreate(effect);
        RenderingService.MaterialSetCullMode(_material, CullMode.None);

        var meshSurface = new MeshSurface
        {
            Vertices = new[]
            {
                new Vector3(0.0f, 0.5f, 0f), // top
                new Vector3(0.5f, -0.5f, 0f), // bottom-right
                new Vector3(-0.5f, -0.5f, 0f) // bottom-left
            },
            Indices = new[] { 0, 1, 2 },
            Colors = new[]
            {
                Color.Red,
                Color.Green,
                Color.Blue
            }
        };

        _mesh = RenderingService.MeshCreate();
        RenderingService.MeshAddSurface(_mesh, PrimitiveType.TriangleList, meshSurface, _material);
        RenderingService.InstanceSetMesh(_root, _mesh);
    }

    public override void Update(GameTime gameTime)
    {
        _elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
        var rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, _elapsed);
        RenderingService.InstanceSetRotation(_root, rotation);
        Camera.Update(gameTime);
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
        RenderingService.FreeRid(_mesh);
        RenderingService.FreeRid(_material);
        Camera.Dispose();
        RenderingService.FreeRid(_root);
        RenderingService.FreeRid(_renderTarget);
        base.Dispose();
    }
}