using FezEditor.Hosts;
using FezEditor.Services;
using FezEditor.Structure;
using FezEditor.Tools;
using ImGuiNET;
using Microsoft.Xna.Framework;

namespace FezEditor.Components;

public class TestComponent : EditorComponent
{
    private readonly TestHost _host;
    
    public TestComponent(Game game, string title) : base(game, title)
    {
        _host = new TestHost(game);
        _host.Load(new object());
    }

    public override void Update(GameTime gameTime)
    {
        _host.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        var size = ImGuiX.GetContentRegionAvail();
        var w = (int)size.X;
        var h = (int)size.Y;
            
        if (w > 0 && h > 0)
        {
            var texture = _host.GetViewportTexture();
            if (texture == null || texture.Width != w || texture.Height != h)
            {
                _host.SetViewportSize(w, h);
                _host.Camera.AspectRatio = (float)w / h;
            }

            if (texture is { IsDisposed: false })
            {
                ImGuiX.Image(texture, size);
            }
        }
    }

    public override void Dispose()
    {
        _host.Dispose();
    }
}