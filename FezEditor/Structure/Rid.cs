namespace FezEditor.Structure;

/// <summary>
/// Opaque resource identifier. All server-managed resources are referenced by RID.
/// A default RID (value 0) is invalid/null.
/// </summary>
/// <param name="Id">Value of RID</param>
public readonly record struct Rid(uint Id)
{
    public static readonly Rid Invalid = new(0);
    
    public bool Valid => Id > 0;

    public override string ToString() => $"RID({Id})";
}