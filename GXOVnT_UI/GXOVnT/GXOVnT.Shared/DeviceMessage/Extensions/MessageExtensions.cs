using GXOVnT.Shared.DeviceMessage.Common;

namespace GXOVnT.Shared.DeviceMessage.Extensions;

public static class MessageExtensions
{
    
    public static CommMessage ToCommMessage<T>(this T model, short messageId) where T : BaseMessageModel
    {
        var jsonPayload = System.Text.Json.JsonSerializer.Serialize(model);
        var bytes = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        return bytes.ToCommMessage(messageId);
    }
    
    public static CommMessage ToCommMessage(this byte[] buffer, short messageId, int packetChunkSize = 20)
    {
        return new CommMessage(messageId, buffer, packetChunkSize);
    }
    
}