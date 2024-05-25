using Google.Protobuf;
using GXOVnT.Shared.DeviceMessage;

namespace GXOVnT.Shared.Helpers;

public static class ProtoCommMessageExtensions
{

    public static CommMessage ToCommMessage<T>(this T protoMessage, Int16 messageId, int packetChunkSize = 20) where T : IMessage
    {
        using var stream = new MemoryStream();
        protoMessage.WriteTo(stream);
        var buffer = stream.ToArray();
        return new CommMessage(messageId, buffer, packetChunkSize);
    }
    
}