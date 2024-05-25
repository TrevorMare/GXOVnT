namespace GXOVnT.Shared.Common;

[Flags]
public enum CommMessageDetail : byte
{
    None = 0,
    IsStartPacket = 1,
    IsEndPacket = 2
}