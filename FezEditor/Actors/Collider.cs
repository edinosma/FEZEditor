using FezEditor.Tools;
using Microsoft.Xna.Framework;

namespace FezEditor.Actors;

public class Collider : ActorComponent
{
    public BoundingBox BoundingBox { get; private set; }
    
    public Vector3 Size { get; set; }

    private Transform _transform = null!;
    
    public override void Initialize()
    {
        _transform = Actor.GetComponent<Transform>();
    }

    public override void Update(GameTime gameTime)
    {
        BoundingBox = Mathz.ComputeBoundingBox(
            _transform.Position, _transform.Rotation,
            _transform.Scale, Size
        );
    }
}