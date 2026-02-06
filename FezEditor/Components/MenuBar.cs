using FezEditor.Services;
using FezEditor.Tools;
using ImGuiNET;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FezEditor.Components;

[UsedImplicitly]
public class MenuBar : DrawableGameComponent
{
    private Texture2D _logoTexture = null!;

    private AboutWindow? _aboutWindow;
    
    private readonly IStateService _stateService;

    public MenuBar(Game game, IStateService stateService) : base(game)
    {
        _stateService = stateService;
    }

    protected override void LoadContent()
    {
        _logoTexture = Game.Content.Load<Texture2D>("Content/Icon");
    }

    public override void Draw(GameTime gameTime)
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("File", _stateService.CurrentState >= IStateService.State.ResourcesLoaded))
            {
                ImGui.Separator();

                if (ImGui.MenuItem("Save File"))
                {
                    // TODO: saving single modified file
                }
                
                if (ImGui.MenuItem("Save File As..."))
                {
                    // TODO: saving single modified file to different location
                }
                
                if (ImGui.MenuItem("Save All Files"))
                {
                    // TODO: saving all modified files
                }
                
                ImGui.Separator();
                
                if (ImGui.MenuItem("Undo"))
                {
                    // TODO: History undo
                }
                
                if (ImGui.MenuItem("Redo"))
                {
                    // TODO: History redo
                }
                
                ImGui.Separator();
                
                if (ImGui.MenuItem("Close File"))
                {
                    // TODO: Closing currently opened file
                }
                
                if (ImGui.MenuItem("Quit"))
                {
                    _stateService.Quit();
                }

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Help", _stateService.CurrentState != IStateService.State.ResourcesExtracting))
            {
                ImGuiX.Image(_logoTexture, new Vector2(16, 16));
                ImGui.SameLine();
                if (ImGui.MenuItem("About FEZEditor..."))
                {
                    ShowAboutWindow();
                }

                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }
    }

    private void ShowAboutWindow()
    {
        if (_aboutWindow == null)
        {
            _aboutWindow = Game.CreateComponent<AboutWindow>();
            _aboutWindow.Disposed += (_, _) => { _aboutWindow = null; };
        }
    }
}