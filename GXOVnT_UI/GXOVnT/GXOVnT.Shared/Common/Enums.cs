namespace GXOVnT.Shared.Common;

[Flags]
public enum CommMessageDetail : byte
{
    None = 0,
    IsStartPacket = 1,
    IsEndPacket = 2
}

public enum JsonModelType 
{
    
    EchoRequest = 3,
    
    Unknown = 999
}