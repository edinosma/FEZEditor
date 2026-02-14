using FezEditor.Structure;
using ImGuiNET;
using Microsoft.Xna.Framework;

namespace FezEditor.Components;

public class SallyEditor : EditorComponent
{
    public override object Asset => _saveData;
    
    private readonly SaveData _saveData;

    public SallyEditor(Game game, string title, SaveData saveData) : base(game, title)
    {
        _saveData = saveData;
        History.Track(saveData);
    }
    
    public override void Draw()
    {
        ImGuiX.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(8, 8));
        
        var availSize = ImGui.GetContentRegionAvail();
        var width = availSize.X / 3f;

        if (ImGuiX.BeginChild("##Properties", new Vector2(width, 0), ImGuiChildFlags.Border))
        {
            DrawProperties();
            ImGui.EndChild();
        }
        
        ImGui.SameLine();
        
        if (ImGuiX.BeginChild("##LevelList", new Vector2(width, 0), ImGuiChildFlags.Border))
        {
            DrawLevelList();
            ImGui.EndChild();
        }
        
        ImGui.SameLine();

        if (ImGuiX.BeginChild("##LevelProperties", Vector2.Zero, ImGuiChildFlags.Border))
        {
            DrawLevelProperties();
            ImGui.EndChild();
        }

        ImGui.PopStyleVar();
    }
    
    private void DrawProperties()
    {
        //throw new NotImplementedException();
    }

    private void DrawLevelList()
    {
        //throw new NotImplementedException();
    }

    private void DrawLevelProperties()
    {
        //throw new NotImplementedException();
    }
}