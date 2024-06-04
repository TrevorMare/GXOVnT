using GXOVnT.Shared.Common;

namespace GXOVnT.Services.Common;

public class LogMessageType : Enumeration
{
    public static readonly LogMessageType Debug = new(0, nameof(Debug));
    public static readonly LogMessageType Information = new(1, nameof(Information));
    public static readonly LogMessageType Warning = new(2, nameof(Warning));
    public static readonly LogMessageType Error = new(2, nameof(Error));

    private LogMessageType(int id, string name)
        : base(id, name)
    {
    }
}