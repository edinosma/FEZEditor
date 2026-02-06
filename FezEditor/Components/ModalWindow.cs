using FezEditor.Tools;
using ImGuiNET;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FezEditor.Components;

[UsedImplicitly]
public class ModalWindow : DrawableGameComponent
{
    private enum State
    {
        YesNo,
        Ok,
        Disposed
    }

    private string _title = "";

    private string _message = "";

    private Action? _onYes;

    private Action? _onNo;

    private State _state;

    public ModalWindow(Game game) : base(game)
    {
        Enabled = false;
    }

    public void Show(string message, string title = "Message")
    {
        _message = message;
        _title = title;
        _onYes = null;
        _onNo = null;
        _state = State.Ok;
        Enabled = true;
    }

    public void ShowConfirm(string message, Action onYes, Action? onNo = null, string title = "Confirm")
    {
        _message = message;
        _title = title;
        _onYes = onYes;
        _onNo = onNo;
        _state = State.YesNo;
        Enabled = true;
    }

    public override void Update(GameTime gameTime)
    {
        if (_state == State.Disposed)
        {
            Game.RemoveComponent(this);
        }
    }

    public override void Draw(GameTime gameTime)
    {
        ImGui.OpenPopup(_title);
        var center = ImGui.GetMainViewport().GetCenter().ToXna();
        ImGuiX.SetNextWindowPos(center, ImGuiCond.Appearing, Vector2.One / 2f);

        var open = true;
        if (ImGui.BeginPopupModal(_title, ref open,
                ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove))
        {
            ImGui.TextWrapped(_message);
            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();

            if (_state == State.YesNo)
            {
                // Yes/No buttons
                const int buttonWidth = 80;
                const int spacing = 10;
                const int totalWidth = buttonWidth * 2 + spacing;
                ImGui.SetCursorPosX((ImGui.GetWindowWidth() - totalWidth) * 0.5f);

                if (ImGui.Button("Yes", new System.Numerics.Vector2(buttonWidth, 0)))
                {
                    _onYes?.Invoke();
                    _state = State.Disposed;
                    ImGui.CloseCurrentPopup();
                }

                ImGui.SameLine();

                if (ImGui.Button("No", new System.Numerics.Vector2(buttonWidth, 0)) ||
                    ImGui.IsKeyPressed(ImGuiKey.Escape))
                {
                    _onNo?.Invoke();
                    _state = State.Disposed;
                    ImGui.CloseCurrentPopup();
                }
            }

            if (_state == State.Ok)
            {
                // Just OK button
                const float buttonWidth = 120;
                ImGui.SetCursorPosX((ImGui.GetWindowWidth() - buttonWidth) * 0.5f);

                if (ImGui.Button("OK", new System.Numerics.Vector2(buttonWidth, 0)) ||
                    ImGui.IsKeyPressed(ImGuiKey.Escape))
                {
                    _state = State.Disposed;
                    ImGui.CloseCurrentPopup();
                }
            }
        }

        ImGui.EndPopup();
    }
}