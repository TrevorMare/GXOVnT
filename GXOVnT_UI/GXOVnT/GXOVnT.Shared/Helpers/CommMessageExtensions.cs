
using GXOVnT.Shared.DeviceMessage;

namespace GXOVnT.Shared.Helpers;

public static class CommMessageExtensions
{

    public static CommMessage ToCommMessage(this byte[] buffer, Int16 messageId, int packetChunkSize = 20)
    {
        return new CommMessage(messageId, buffer, packetChunkSize);
    }
    
}