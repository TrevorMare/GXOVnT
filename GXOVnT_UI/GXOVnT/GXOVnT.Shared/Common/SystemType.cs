namespace GXOVnT.Shared.Common;

public class SystemType : Enumeration
{
    
    public static readonly SystemType UnInitialized = new(0, nameof(UnInitialized));
    public static readonly SystemType Extension = new(1, nameof(Extension));
    public static readonly SystemType Primary = new(2, nameof(Primary));

    private SystemType(int id, string name)
        : base(id, name)
    {
    }
    
}