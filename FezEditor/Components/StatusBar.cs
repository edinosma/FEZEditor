using FezEditor.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FezEditor.Components;

[UsedImplicitly]
public class StatusBar : DrawableGameComponent
{
    private readonly IEditorService _editorService;

    public StatusBar(Game game, IEditorService editorService) : base(game)
    {
        _editorService = editorService;
    }

    public void Draw()
    {
        // TODO: implement this
    }
}
