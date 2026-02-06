using FezEditor.Services;
using FezEditor.Tools;
using ImGuiNET;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FezEditor.Components;

[UsedImplicitly]
public class WelcomeScreen : DrawableGameComponent
{
    private Texture2D _logoTexture = null!;

    private bool _open = true;

    public WelcomeScreen(Game game) : base(game)
    {
    }

    protected override void LoadContent()
    {
        _logoTexture = Game.Content.Load<Texture2D>("Content/Icon");
    }

    public override void Draw(GameTime gameTime)
    {
        ImGui.SetNextWindowCollapsed(!_open, ImGuiCond.FirstUseEver);

        var center = ImGui.GetMainViewport().GetCenter().ToXna();
        ImGuiX.SetNextWindowPos(center, ImGuiCond.Appearing, Vector2.One / 2);

        if (ImGui.Begin("Start", ref _open,
                ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoMove))
        {
            ImGuiX.Image(_logoTexture, new Vector2(64, 64));
            ImGui.NewLine();
            ImGui.Text("Welcome to FEZEDITOR!");
            ImGui.NewLine();

            if (ImGui.Button("Open PAK..."))
            {
                FileDialog.Show(FileDialog.Type.OpenFile, CreatePakService, new FileDialog.Options
                {
                    Title = "Choose PAK file...",
                    Filters = new FileDialog.Filter[]
                    {
                        new("PAK files", "pak")
                    }
                });
            }

            if (ImGui.Button("Open assets directory..."))
            {
                FileDialog.Show(FileDialog.Type.OpenFolder, CreateDirService, new FileDialog.Options
                {
                    Title = "Choose assets directory..."
                });
            }

            if (ImGui.Button("Extract assets and open..."))
            {
                var selectOptions = new FileDialog.Options
                {
                    Title = "Select PAK files to extract...",
                    AllowMultiple = true,
                    Filters = new FileDialog.Filter[]
                    {
                        new("PAK files", "pak")
                    }
                };

                FileDialog.Show(FileDialog.Type.OpenFile, source =>
                    {
                        FileDialog.Show(FileDialog.Type.OpenFolder,
                            target => CreateContentExtractor(source, target), new FileDialog.Options
                            {
                                Title = "Choose a directory to save assets..."
                            });
                    },
                    selectOptions);
            }
        }

        ImGui.End();
    }

    private void CreateContentExtractor(FileDialog.Result source, FileDialog.Result target)
    {
    }

    private void CreatePakService(FileDialog.Result result)
    {
        if (result.Files.Length > 0)
        {
            try
            {
                var pakService = Game.CreateService<IResourceService, PakResourceService>();
                pakService.Initialize(new FileInfo(result.Files[0]));
            }
            catch (Exception ex)
            {
                Game.CreateComponent<ModalWindow>().Show(ex.Message, "Error occured!");
            }
        }
    }

    private void CreateDirService(FileDialog.Result result)
    {
        if (result.Files.Length > 0)
        {
            try
            {
                var dirService = Game.CreateService<IResourceService, DirResourceService>();
                dirService.Initialize(new DirectoryInfo(result.Files[0]));
            }
            catch (Exception ex)
            {
                Game.CreateComponent<ModalWindow>().Show(ex.Message, "Error occured!");
            }
        }
    }
}