using System.Text.Json;
using FezEditor.Tools;
using FEZRepacker.Core.Conversion;
using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.XNB;
using ImGuiNET;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Serilog;

namespace FezEditor.Components;

[UsedImplicitly]
public class ContentExtractor : DrawableGameComponent
{
    private static readonly ILogger Logger = Logging.Create<ContentExtractor>();
    
    private const string ContentListingFile = "Content\\ContentListing.json";
    
    private const string AssetsDirectory = "Assets";
    
    private const string ContentDirectory = "Content";
    
    private readonly List<string> _pakFiles = new();
    
    private readonly Dictionary<string, string> _contentListing = new(StringComparer.OrdinalIgnoreCase);
    
    private float _progress;
    
    private string _currentFile = "";
    
    private int _filesProcessed;
    
    private int _totalFiles;
    
    private string _status = "";
    
    private State _state = State.Scan;
    
    private CancellationTokenSource? _cts;

    public ContentExtractor(Game game) : base(game)
    {
        Enabled = true;
    }

    protected override void LoadContent()
    {
        using var jsonStream = File.Open(ContentListingFile, FileMode.Open);
        var json = JsonSerializer.Deserialize<ContentListing>(jsonStream);

        foreach (var file in json?.PakFiles ?? Array.Empty<string>())
        {
            _pakFiles.Add(file);
        }

        foreach (var listing in json?.Listing ?? Array.Empty<string>())
        {
            _contentListing.Add(listing, listing);
        }
    }

    public override void Update(GameTime gameTime)
    {
        switch (_state)
        {
            case State.Scan:
            {
                var assetsDirectoryExist = Directory.Exists(AssetsDirectory);
                if (!assetsDirectoryExist)
                {
                    Directory.CreateDirectory(AssetsDirectory);
                }
                
                var assetsCountMatch = Directory.EnumerateFiles(AssetsDirectory, "*", SearchOption.AllDirectories).Count();
                if (assetsCountMatch < _contentListing.Count)
                {
                    _state = State.Disposed;
                    return;
                }
                
                _state = State.CanExtract;
                return;
            }

            case State.Disposed:
            {
                Game.RemoveComponent(this);
                return;
            }
        }
    }

    public override void Draw(GameTime gameTime)
    {
        switch (_state)
        {
            case State.CanExtract:
            {
                ImGui.OpenPopup("Confirm Extraction");
        
                var isOpen = true;
                if (ImGui.BeginPopupModal("Confirm Extraction", ref isOpen, ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoCollapse))
                {
                    ImGui.Text("This will extract all game assets to:");
                    ImGuiX.TextColored(new Color(0.5f, 0.8f, 1f, 1f), AssetsDirectory);
                    ImGui.Spacing();
                    ImGui.Text("Do you want to continue?");
                    ImGui.Spacing();
            
                    if (ImGuiX.Button("Yes", new Vector2(120, 0)))
                    {
                        _state = State.Extracting;
                        _ = ExtractAsync();
                        ImGui.CloseCurrentPopup();
                    }
            
                    ImGui.SameLine();
            
                    if (ImGuiX.Button("No", new Vector2(120, 0)))
                    {
                        _state = State.Disposed;
                        ImGui.CloseCurrentPopup();
                    }
            
                    ImGui.EndPopup();
                }

                if (!isOpen)
                {
                    _state = State.Disposed;
                }
                return;
            }

            case State.Extracting:
            case State.Complete:
            {
                ImGui.Begin("Extracting assets...", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.AlwaysAutoResize);
        
                if (_state == State.Extracting)
                {
                    ImGui.Text(_status);
                    ImGui.Text($"File: {_currentFile}");
                    ImGui.Text($"Progress: {_filesProcessed} / {_totalFiles}");
            
                    ImGuiX.ProgressBar(_progress, new Vector2(400, 0), $"{(_progress * 100):F1}%");
                    if (ImGui.Button("Cancel"))
                    {
                        _cts?.Cancel();
                    }
                }
                else
                {
                    ImGuiX.TextColored(
                        _status.Contains("Error") ? Color.Red : 
                        _status.Contains("complete") ? Color.Green : 
                        Color.White, 
                        _status);
                }
        
                ImGui.End();
                return;
            }
        }
    }
    
    private async Task ExtractAsync()
    {
        _cts = new CancellationTokenSource();
        _state = State.Extracting;
        _status = "Counting files...";
        _progress = 0f;
        
        try
        {
            await Task.Run(() => ExtractInternal(_cts.Token), _cts.Token);
            _status = "Extraction complete!";
            _progress = 1.0f;
        }
        catch (OperationCanceledException)
        {
            _status = "Extraction cancelled";
        }
        catch (Exception ex)
        {
            _status = $"Error: {ex.Message}";
        }
        finally
        {
            _state = State.Complete;
            await Task.Delay(2000);
            _state = State.Disposed;
        }
    }
    
    private void ExtractInternal(CancellationToken ct)
    {
        // First pass: count total files
        _totalFiles = 0;
        foreach (var file in _pakFiles)
        {
            var pakPath = file.EndsWith("Music.pak") 
                ? Path.Combine(ContentDirectory, "Music", file) 
                : Path.Combine(ContentDirectory, file);
            
            if (!File.Exists(pakPath))
            {
                continue;
            }
                
            using var pakStream = File.OpenRead(pakPath);
            using var pakReader = new PakReader(pakStream);

            foreach (var pakFile in pakReader.ReadFiles())
            {
                if (_contentListing.ContainsKey(pakFile.Path))
                {
                    _totalFiles += 1;
                }
                else
                {
                    Logger.Warning("Missing listed asset: {}", pakFile.Path);
                }
            }
        }
        
        _filesProcessed = 0;
        if (_totalFiles == 0)
        {
            _status = "No files to extract";
            return;
        }
        
        // Second pass: extract files
        foreach (var file in _pakFiles)
        {
            ct.ThrowIfCancellationRequested();
            
            var pakPath = Path.Combine(ContentDirectory, file);
            if (!File.Exists(pakPath))
            {
                continue;
            }
            
            var outputDir = AssetsDirectory;
            if (pakPath.EndsWith("Music.pak"))
            {
                outputDir = Path.Combine(AssetsDirectory, "Music");
            }

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }
            
            using var pakStream = File.OpenRead(pakPath);
            using var pakReader = new PakReader(pakStream);
            
            foreach (var pakFile in pakReader.ReadFiles())
            {
                ct.ThrowIfCancellationRequested();
                
                _currentFile = pakPath.EndsWith("Music.pak")
                    ? _contentListing[$"music\\{pakFile.Path}"]
                    : _contentListing[pakFile.Path];
                
                _status = $"Extracting: {file}";
                var extension = pakFile.FindExtension();
                using var fileStream = pakFile.Open();
                var initialStreamPosition = fileStream.Position;
                
                FileBundle bundle;
                try
                {
                    var outputData = XnbSerializer.Deserialize(fileStream)!;
                    bundle = FormatConversion.Convert(outputData);
                }
                catch (Exception)
                {
                    fileStream.Seek(initialStreamPosition, SeekOrigin.Begin);
                    bundle = FileBundle.Single(fileStream, extension);
                }
                
                var pakFilePathNormalized = Path.Combine(_currentFile.Split('/', '\\'));
                bundle.BundlePath = Path.Combine(outputDir, pakFilePathNormalized);
                Directory.CreateDirectory(Path.GetDirectoryName(bundle.BundlePath) ?? "");

                foreach (var outputFile in bundle.Files)
                {
                    var fileName = bundle.BundlePath + bundle.MainExtension + outputFile.Extension;
                    using var fileOutputStream = File.Open(fileName, FileMode.Create);
                    outputFile.Data.CopyTo(fileOutputStream);
                }
                
                bundle.Dispose();
                
                _filesProcessed++;
                _progress = (float)_filesProcessed / _totalFiles;
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        _cts?.Dispose();
        base.Dispose(disposing);
    }

    private enum State
    {
        Scan,
        CanExtract,
        Extracting,
        Complete,
        Disposed
    }
    
    private record ContentListing(
        string[] PakFiles,
        string[] Listing
    );
}