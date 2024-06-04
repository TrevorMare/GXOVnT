using GXOVnT.Shared.DeviceMessage;
using GXOVnT.Shared.JsonModels;

namespace GXOVnT.Shared.Helpers;

public static class JsonModelExtensions
{
    public static CommMessage ToCommMessage<T>(this T model, short messageId) where T : BaseModel
    {
        var jsonPayload = System.Text.Json.JsonSerializer.Serialize(model);
        var bytes = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        return bytes.ToCommMessage(messageId);
    }
}