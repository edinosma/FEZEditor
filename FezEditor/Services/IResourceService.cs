namespace FezEditor.Services;

public interface IResourceService : IDisposable
{
    event Action? ProviderChanged;
    
    bool HasNoProvider { get; }
    
    bool IsReadonly { get; }
    
    string Root { get; }
    
    IEnumerable<string> Files { get; }

    void OpenProvider(FileSystemInfo info);

    void CloseProvider();
    
    object Load(string path);

    void Save(string path, object asset);
}