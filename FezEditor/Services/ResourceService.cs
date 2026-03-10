using System.Diagnostics;
using FezEditor.Structure;
using FezEditor.Tools;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace FezEditor.Services;

[UsedImplicitly]
public class ResourceService : IDisposable
{
    public event Action? ProviderChanged;

    public bool HasNoProvider => _provider == null;

    public bool IsReadonly => _provider?.IsReadonly ?? true;

    public string Root => _provider?.Root ?? string.Empty;

    public IEnumerable<string> Files => _provider?.Files ?? Enumerable.Empty<string>();

    private IResourceProvider? _provider;

    private readonly IContentManager _content;

    private readonly Game _game;

    public ResourceService(Game game)
    {
        _game = game;
        _game.Activated += OnGameActivated;
        _content = game.GetService<ContentService>().Global;
    }

    private void OnGameActivated(object? o, EventArgs eventArgs)
    {
#if (!DEBUG)
        if (_provider != null)
        {
            _provider.Refresh();
            ProviderChanged?.Invoke();
        }
#endif
    }

    public void OpenProvider(FileSystemInfo info)
    {
        IResourceProvider provider = info switch
        {
            FileInfo file => new PakResourceProvider(file),
            DirectoryInfo dir => new DirResourceProvider(dir),
            _ => throw new ArgumentException("Not supported: " + info)
        };

        CloseProvider();
        _provider = provider;
        ProviderChanged?.Invoke();
    }

    public void CloseProvider()
    {
        ProviderChanged?.Invoke();
        _provider?.Dispose();
        _provider = null;
    }

    public Stream OpenStream(string path, string extension)
    {
        return _provider!.OpenStream(path, extension);
    }

    public string GetExtension(string path)
    {
        return _provider?.GetExtension(path) ?? string.Empty;
    }

    public string GetFullPath(string path)
    {
        return _provider?.GetFullPath(path) ?? string.Empty;
    }

    public string GetRelativePath(string absolutePath)
    {
        var root = GetFullPath(string.Empty);
        return absolutePath.WithoutBaseDirectory(root).Replace('\\', '/');
    }

    public object Load(string path)
    {
        if (path.Contains("SaveSlot", StringComparison.OrdinalIgnoreCase))
        {
            using var stream = _provider!.OpenStream(path, string.Empty);
            return SaveData.Read(stream);
        }

        return _provider!.Load<object>(path);
    }

    public SaveData LoadSaveDataFromContent(string path)
    {
        using var stream = _content.LoadStream(path);
        return SaveData.Read(stream);
    }

    public Dictionary<string, RAnimatedTexture> LoadAnimations(string path)
    {
        var animations = new Dictionary<string, RAnimatedTexture>(StringComparer.OrdinalIgnoreCase);

        foreach (var file in _provider!.Files)
        {
            if (file.StartsWith(path, StringComparison.OrdinalIgnoreCase) &&
                !file.Contains("Metadata", StringComparison.OrdinalIgnoreCase))
            {
                var name = file[(path.Length + 1)..];
                var asset = _provider!.Load<RAnimatedTexture>(file);
                animations.Add(name, asset);
            }
        }

        return animations;
    }

    public void Save(string path, object asset)
    {
        if (asset is SaveData saveData)
        {
            using var stream = SaveData.Write(saveData);
            using var fileStream = new FileStream(path, FileMode.Create);
            stream.CopyTo(fileStream);
            return;
        }

        _provider!.Save(path, asset);
        _provider.Refresh();
        ProviderChanged?.Invoke();
    }

    public void Duplicate(string path)
    {
        _provider!.Duplicate(path);
        _provider.Refresh();
        ProviderChanged?.Invoke();
    }

    public void Move(string path, string newPath)
    {
        _provider!.Move(path, newPath);
        _provider.Refresh();
        ProviderChanged?.Invoke();
    }

    public void Delete(string path)
    {
        _provider!.Remove(path);
        _provider.Refresh();
        ProviderChanged?.Invoke();
    }

    public void OpenInFileManager(string path)
    {
        var absolutePath = GetFullPath(path);
        var target = File.Exists(absolutePath) ? absolutePath : Path.GetDirectoryName(absolutePath)!;

        if (OperatingSystem.IsWindows())
        {
            Process.Start("explorer.exe", $"/select,\"{target}\"");
        }
        else if (OperatingSystem.IsMacOS())
        {
            Process.Start("open", $"-R \"{target}\"");
        }
        else
        {
            Process.Start("xdg-open", $"\"{Path.GetDirectoryName(target)}\"");
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _provider?.Dispose();
        _game.Activated -= OnGameActivated;
    }
}