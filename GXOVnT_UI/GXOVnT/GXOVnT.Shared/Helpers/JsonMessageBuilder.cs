

using GXOVnT.Shared.JsonModels;

namespace GXOVnT.Shared.Helpers;

public static class JsonMessageBuilder
{

    public static byte[] BuildEchoMessage(string echoMessage)
    {

        var container = new GXOVnTMessagingContainer()
        {
            MessageTypeId = 10,
            MessageData = echoMessage,
            HasMessageData = true
        };

        var payload = System.Text.Json.JsonSerializer.Serialize(container);
        return System.Text.Encoding.UTF8.GetBytes(payload);
    }
    
    
}