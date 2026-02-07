using Microsoft.Xna.Framework;

namespace FezEditor.Components;

public abstract class EditorComponent
{
    public abstract string Title { get; }
    
    protected Game Game { get; }

    protected EditorComponent(Game game)
    {
        Game = game;
    }
    
    public virtual void Initialize() { }
    
    public virtual void Update(GameTime gameTime) { }
    
    public virtual void Draw(GameTime gameTime) { }
    
    public virtual void Dispose() { }
}