using Microsoft.Xna.Framework;

namespace FezEditor.Services;

public interface IImGuiService : IDisposable
{
    void BeforeLayout(GameTime gameTime);
    
    void AfterLayout();
}