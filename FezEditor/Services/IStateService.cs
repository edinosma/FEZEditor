namespace FezEditor.Services;

public interface IStateService
{
    enum State
    {
        Loading,
        ResourcesExtracting,
        ResourcesLoaded
    }
    
    State CurrentState { get; }

    void Quit();
}