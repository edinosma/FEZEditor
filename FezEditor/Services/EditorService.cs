using FezEditor.Components;
using FezEditor.Tools;
using JetBrains.Annotations;

namespace FezEditor.Services;

[UsedImplicitly]
public class EditorService : IEditorService
{
    public IEnumerable<EditorComponent> Editors => _editors;

    public EditorComponent? ActiveEditor { get; private set; }

    private readonly List<EditorComponent> _editors = new();

    public void OpenEditor(EditorComponent editor)
    {
        _editors.Add(editor);
        {
            editor.Initialize();
        }
        
        if (ActiveEditor != editor)
        {
            ActiveEditor = editor;
        }
    }

    public void CloseEditor(EditorComponent editor)
    {
        if (_editors.Remove(editor))
        {
            editor.Dispose();
        }
        
        if (editor == ActiveEditor)
        {
            ActiveEditor = Editors.FirstOrDefault();
        }
    }

    public void MarkEditorActive(EditorComponent editor)
    {
        if (Editors.Contains(editor) && ActiveEditor != editor) 
        {
            ActiveEditor = editor;
        }
    }
}