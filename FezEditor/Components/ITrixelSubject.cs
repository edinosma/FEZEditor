using FezEditor.Structure;
using Microsoft.Xna.Framework.Graphics;

namespace FezEditor.Components;

public interface ITrixelSubject
{
    string TextureExportKey { get; }
    
    TrixelObject Materialize();

    object GetAsset(TrixelObject obj);
    
    Texture2D LoadTexture(GraphicsDevice gd);
    
    void UpdateTexture(Texture2D texture);

    void DrawProperties(History history);
}