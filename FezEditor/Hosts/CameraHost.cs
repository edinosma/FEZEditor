using FezEditor.Structure;
using Microsoft.Xna.Framework;

namespace FezEditor.Hosts;

public class CameraHost : Host
{
    public enum ProjectionType
    {
        Perspective,
        Orthographic
    }

    public sealed override Rid Rid { get; protected set; }
    
    public Vector3 Position { get; set; } = Vector3.Zero;
    
    public Quaternion Rotation { get; set; } = Quaternion.Identity;
    
    public ProjectionType Projection { get; set; } = ProjectionType.Perspective;

    public float FieldOfView { get; set; } = 75.0f;

    public float Size { get; set; } = 1.0f;

    public float Near { get; set; } = 0.05f;
    
    public float Far { get; set; } = 1000.0f;

    public float AspectRatio { get; set; } = 1.0f;

    public CameraHost(Game game) : base(game)
    {
        Rid = RenderingService.CameraCreate();
    }
    
    ~CameraHost()
    {
        Dispose();
    }

    public override void Update(GameTime gameTime)
    {
        var rotationMatrix = Matrix.CreateFromQuaternion(Rotation);
        var target = Position - rotationMatrix.Forward;
        var viewMatrix = Matrix.CreateLookAt(Position, target, rotationMatrix.Up);
        RenderingService.CameraSetView(Rid, viewMatrix);

        var projectionMatrix = Matrix.Identity;
        switch (Projection)
        {
            case ProjectionType.Perspective:
                projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(FieldOfView),
                    AspectRatio,
                    Near,
                    Far
                );
                break;
                
            case ProjectionType.Orthographic:
                projectionMatrix = Matrix.CreateOrthographic(
                    AspectRatio * Size,
                    Size,
                    Near,
                    Far
                );
                break;
        }
        RenderingService.CameraSetProjection(Rid, projectionMatrix);
    }

    public Vector3 ProjectPosition(Vector2 screenPosition)
    {
        throw new NotImplementedException();
    }

    public Vector2 UnprojectPosition(Vector3 worldPosition)
    {
        throw new NotImplementedException();
    }
}