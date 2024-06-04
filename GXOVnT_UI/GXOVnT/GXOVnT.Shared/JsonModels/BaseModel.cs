using System.Text.Json.Serialization;

namespace GXOVnT.Shared.JsonModels;

public class BaseModel
{

    [JsonPropertyName("messageTypeId")]
    public short MessageTypeId { get; set; }
    
    [JsonPropertyName("replyMessageId")]
    public short ReplyMessageId { get; set; }

    
    
}