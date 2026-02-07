using FezEditor.Components;

namespace FezEditor.Services;

public interface IEditorService
{
    IEnumerable<EditorComponent> Editors { get; }
    
    public EditorComponent? ActiveEditor { get; } 
    
    void OpenEditor(EditorComponent editor);

    void CloseEditor(EditorComponent editor);
    
    void MarkEditorActive(EditorComponent editor);
}