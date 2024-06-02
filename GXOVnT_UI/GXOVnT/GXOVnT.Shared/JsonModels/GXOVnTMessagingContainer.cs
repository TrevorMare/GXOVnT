using System.Text.Json.Serialization;

namespace GXOVnT.Shared.JsonModels;

public class GXOVnTMessagingContainer
{

    [JsonPropertyName("messageTypeId")]
    public int MessageTypeId { get; set; }

    [JsonPropertyName("messageData")]
    public string? MessageData { get; set; }

    [JsonPropertyName("hasMessageData")]
    public bool HasMessageData { get; set; }
    
}