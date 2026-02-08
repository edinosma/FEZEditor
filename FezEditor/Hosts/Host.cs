using FezEditor.Services;
using FezEditor.Structure;
using FezEditor.Tools;
using Microsoft.Xna.Framework;

namespace FezEditor.Hosts;

public abstract class Host : IDisposable
{
    public abstract Rid Rid { get; protected set; }

    protected Game Game { get; }
    
    protected IRenderingService RenderingService { get; }

    protected Host(Game game)
    {
        Game = game;
        RenderingService = game.GetService<IRenderingService>();
    }

    public virtual void Load(object asset)
    {
    }

    public virtual void Update(GameTime gameTime)
    {
    }

    public virtual void Dispose()
    {
        RenderingService.FreeRid(Rid);
        Rid = Rid.Invalid;
    }
}