using FezEditor.Components;
using FezEditor.Tools;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FezEditor.Services;

[UsedImplicitly]
public class StateService : IStateService
{
    public IStateService.State CurrentState => IStateService.State.Loading;

    private readonly Game _game;
    
    private ModalWindow? _modal;
    
    public StateService(Game game)
    {
        _game = game;
    }

    public void Quit()
    {
        if (_modal == null)
        {
            _modal = _game.CreateComponent<ModalWindow>();
            _modal.ShowConfirm(title: "Quitting FezEditor...",
                message: "Are you sure?",
                onYes: () => _game.Exit(),
                onNo: null);
            _modal.Disposed += (_, _) => { _modal = null; };
        }
    }
    
    
}