using ImGuiNET;
using Microsoft.Xna.Framework;

namespace FezEditor.Components;

public class NotSupportedComponent : EditorComponent
{
    private readonly Type _type;
    
    public NotSupportedComponent(Game game, string title, Type type) : base(game, title)
    {
        _type = type;
    }

    public override void Draw()
    {
        var text = $"(!) There's no editor for the asset of {_type.Name} type!";
        ImGuiX.SetTextCentered(text);
        ImGui.Text(text);
    }
}