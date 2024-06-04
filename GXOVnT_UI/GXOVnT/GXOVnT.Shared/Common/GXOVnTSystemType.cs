namespace GXOVnT.Shared.Common;

public class GXOVnTSystemType : Enumeration
{
    
    public static readonly GXOVnTSystemType UnInitialized = new(0, nameof(UnInitialized));
    public static readonly GXOVnTSystemType Extension = new(1, nameof(Extension));
    public static readonly GXOVnTSystemType Primary = new(2, nameof(Primary));

    private GXOVnTSystemType(int id, string name)
        : base(id, name)
    {
    }
    
}