using FezEditor.Structure;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FezEditor.Hosts;

public class GridHost : Host
{
    public sealed override Rid Rid { get; protected set; }

    public bool Enabled { get; set; } = true;

    public GridPlane Plane { get; set; } = GridPlane.Z;

    public Color PrimaryColor { get; set; } = new(0.5f, 0.5f, 0.5f);

    public Color SecondaryColor { get; set; } = new(0.3f, 0.3f, 0.3f, 0.8f);

    public float CellSize { get; set; } = 1f;
    
    public int NumberOfCells { get; set; } = 1000;

    public int PrimarySteps { get; set; } = 10;
    
    public int SecondaryStep { get; set; } = 1;

    private readonly GridPlaneData[] _planes = new GridPlaneData[3];

    private readonly Rid _primaryMaterial;
    
    private readonly Rid _secondaryMaterial;

    public GridHost(Game game) : base(game)
    {
        Rid = RenderingService.InstanceCreate(Rid.Invalid);
        var effect = Game.Content.Load<Effect>("Effects/Grid");

        _primaryMaterial = RenderingService.MaterialCreate(effect.Clone());
        RenderingService.MaterialSetAlbedo(_primaryMaterial, PrimaryColor);

        _secondaryMaterial = RenderingService.MaterialCreate(effect.Clone());
        RenderingService.MaterialSetAlbedo(_secondaryMaterial, SecondaryColor);
        
        for (var i = 0; i < 3; i++)
        {
            var mesh = RenderingService.MeshCreate();
            var instance = RenderingService.InstanceCreate(Rid);
            RenderingService.InstanceSetMesh(instance, mesh);
            _planes[i] = new GridPlaneData(instance, mesh);
        }
        
        RenderingService.InstanceSetRotation(_planes[1].Instance, Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.PiOver2));
        RenderingService.InstanceSetRotation(_planes[2].Instance, Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.PiOver2));
        
        GenerateGridMesh(_planes[0].Mesh, GridPlane.X);
        GenerateGridMesh(_planes[1].Mesh, GridPlane.Y);
        GenerateGridMesh(_planes[2].Mesh, GridPlane.Z);
    }

    ~GridHost()
    {
        Dispose();
    }

    public override void Update(GameTime gameTime)
    {
        for (var i = 0; i < 3; i++)
        {
            RenderingService.InstanceSetVisibility(_planes[i].Instance, Enabled && i == (int)Plane);
        }
    }

    private void GenerateGridMesh(Rid meshRid, GridPlane plane)
    {
        RenderingService.MeshClear(meshRid);

        // Generate two surfaces: primary (bold) and secondary (faint) grid lines
        var primarySurface = CreateGridSurface(plane, PrimarySteps);
        RenderingService.MeshAddSurface(meshRid, PrimitiveType.LineList, primarySurface, _primaryMaterial);

        var secondarySurface = CreateGridSurface(plane, SecondaryStep);
        RenderingService.MeshAddSurface(meshRid, PrimitiveType.LineList, secondarySurface, _secondaryMaterial);
    }

    private MeshSurface CreateGridSurface(GridPlane plane, int stepInCells)
    {
        var vertices = new List<Vector3>();
        var indices = new List<int>();
        var colors = new List<Color>();
        
        var worldExtent = NumberOfCells * CellSize;
        var worldStep = stepInCells * CellSize;
        var vertexIndex = 0;

        switch (plane)
        {
            case GridPlane.X:
                for (var z = -worldExtent; z <= worldExtent; z += worldStep)
                {
                    vertices.Add(new Vector3(-worldExtent, 0, z));
                    vertices.Add(new Vector3(worldExtent, 0, z));
                    colors.Add(Color.White);
                    colors.Add(Color.White);
                    indices.Add(vertexIndex++);
                    indices.Add(vertexIndex++);
                }
                
                for (var x = -worldExtent; x <= worldExtent; x += worldStep)
                {
                    vertices.Add(new Vector3(x, 0, -worldExtent));
                    vertices.Add(new Vector3(x, 0, worldExtent));
                    colors.Add(Color.White);
                    colors.Add(Color.White);
                    indices.Add(vertexIndex++);
                    indices.Add(vertexIndex++);
                }
                break;

            case GridPlane.Y:
                for (var y = -worldExtent; y <= worldExtent; y += worldStep)
                {
                    vertices.Add(new Vector3(-worldExtent, y, 0));
                    vertices.Add(new Vector3(worldExtent, y, 0));
                    colors.Add(Color.White);
                    colors.Add(Color.White);
                    indices.Add(vertexIndex++);
                    indices.Add(vertexIndex++);
                }
                
                for (var x = -worldExtent; x <= worldExtent; x += worldStep)
                {
                    vertices.Add(new Vector3(x, -worldExtent, 0));
                    vertices.Add(new Vector3(x, worldExtent, 0));
                    colors.Add(Color.White);
                    colors.Add(Color.White);
                    indices.Add(vertexIndex++);
                    indices.Add(vertexIndex++);
                }
                break;

            case GridPlane.Z:
                for (var z = -worldExtent; z <= worldExtent; z += worldStep)
                {
                    vertices.Add(new Vector3(0, -worldExtent, z));
                    vertices.Add(new Vector3(0, worldExtent, z));
                    colors.Add(Color.White);
                    colors.Add(Color.White);
                    indices.Add(vertexIndex++);
                    indices.Add(vertexIndex++);
                }
                
                for (var y = -worldExtent; y <= worldExtent; y += worldStep)
                {
                    vertices.Add(new Vector3(0, y, -worldExtent));
                    vertices.Add(new Vector3(0, y, worldExtent));
                    colors.Add(Color.White);
                    colors.Add(Color.White);
                    indices.Add(vertexIndex++);
                    indices.Add(vertexIndex++);
                }
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(plane));
        }

        return new MeshSurface
        {
            Vertices = vertices.ToArray(),
            Indices = indices.ToArray(),
            Colors = colors.ToArray()
        };
    }

    public override void Dispose()
    {
        foreach (var plane in _planes)
        {
            RenderingService.FreeRid(plane.Instance);
            RenderingService.FreeRid(plane.Mesh);
        }
        RenderingService.FreeRid(_primaryMaterial);
        RenderingService.FreeRid(_secondaryMaterial);
        base.Dispose();
    }
    
    private readonly record struct GridPlaneData(Rid Instance, Rid Mesh);
}