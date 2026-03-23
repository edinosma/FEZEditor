using FezEditor.Services;

namespace FezEditor.Structure;

public record Settings
{
    public static readonly string FilePath = Path.Combine(AppStorageService.BaseDir, "Settings.json");

    public static readonly Settings Default = new()
    {
        RecentPaths = new List<RecentEntry>(),
        Window = new WindowSize(1280, 720)
    };

    public List<RecentEntry> RecentPaths { get; init; } = null!;

    public WindowSize Window { get; init; } = null!;

    public record RecentEntry(string Path, string Kind);

    public record WindowSize(int Width, int Height);
}