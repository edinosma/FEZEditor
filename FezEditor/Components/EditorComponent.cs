using FezEditor.Services;
using FezEditor.Tools;
using Microsoft.Xna.Framework;

namespace FezEditor.Components;

public abstract class EditorComponent
{
    public string Title { get; }
    
    protected Game Game { get; }
    
    protected IRenderingService RenderingService { get; }

    protected EditorComponent(Game game, string title)
    {
        Game = game;
        Title = title;
        RenderingService = game.GetService<IRenderingService>();
    }
    
    public virtual void Initialize() { }
    
    public virtual void Update(GameTime gameTime) { }
    
    public virtual void Draw(GameTime gameTime) { }
    
    public virtual void Dispose() { }
}