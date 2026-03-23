using System.Text.Json;
using FezEditor.Structure;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Serilog;

namespace FezEditor.Services;

[UsedImplicitly]
public class AppStorageService : IDisposable
{
    public static readonly string BaseDir = Path.Combine(AppContext.BaseDirectory, "EditorData");

    private static readonly ILogger Logger = Logging.Create<AppStorageService>();

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private const int MaxRecentPaths = 10;

    public IReadOnlyList<Settings.RecentEntry> RecentPaths => _data.RecentPaths;

    private Settings _data = Settings.Default;

    private readonly Game _game;

    public AppStorageService(Game game)
    {
        _game = game;
        Load();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        SaveWindowState();
        Save();
    }

    public void AddRecentPath(string path, string kind)
    {
        _data.RecentPaths.RemoveAll(rp => string.Equals(rp.Path, path, StringComparison.OrdinalIgnoreCase));
        _data.RecentPaths.Insert(0, new Settings.RecentEntry(path, kind));

        if (_data.RecentPaths.Count > MaxRecentPaths)
        {
            _data.RecentPaths.RemoveRange(MaxRecentPaths, _data.RecentPaths.Count - MaxRecentPaths);
        }
    }

    public void ClearRecentPaths()
    {
        _data.RecentPaths.Clear();
    }

    public void SaveWindowState()
    {
        _data = _data with
        {
            Window = new Settings.WindowSize(_game.Window.ClientBounds.Width, _game.Window.ClientBounds.Height)
        };
    }

    public void LoadWindowState(GraphicsDeviceManager gdm)
    {
        gdm.PreferredBackBufferWidth = _data.Window.Width;
        gdm.PreferredBackBufferHeight = _data.Window.Height;
    }

    public void Save()
    {
        try
        {
            using var file = new FileStream(Settings.FilePath, FileMode.Create);
            JsonSerializer.Serialize(file, _data, JsonOptions);
        }
        catch (Exception e)
        {
            Logger.Error(e, "Unable to save application data");
        }
    }

    private void Load()
    {
        if (!File.Exists(Settings.FilePath))
        {
            Logger.Information("No settings file found, using defaults");
            return;
        }

        try
        {
            using var file = new FileStream(Settings.FilePath, FileMode.Open);
            _data = JsonSerializer.Deserialize<Settings>(file, JsonOptions)!;
        }
        catch (Exception e)
        {
            Logger.Error(e, "Unable to load application data, using defaults");
            _data = Settings.Default;
        }
    }
}
