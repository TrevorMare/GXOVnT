using GXOVnT.Shared.Common;
using GXOVnT.Shared.DeviceMessage;
using GXOVnT.Shared.DeviceMessage.Common;

namespace GXOVnT.Services.Models;

/// <summary>
/// Wrapper class to hold the incoming message with a deserialized base model. This is used for the callbacks
/// on the message orchestrator responses
/// </summary>
public class MessageAggregate
{
    public CommMessage CommMessage { get; set; }
    
    public BaseMessageModel? BaseModel { get; set; }
    
    public short MessageId { get; set; } = 0;
    
    public short ReplyToMessageId { get; set; } = 0;
    
    public JsonModelType ModelType { get; set; } = JsonModelType.Unknown;
    
    public MessageAggregate(CommMessage commMessage)
    {
        CommMessage = commMessage;
        MessageId = commMessage.MessageId;
        BaseModel = global::System.Text.Json.JsonSerializer.Deserialize<BaseMessageModel>(commMessage.ToString());
        ReplyToMessageId = BaseModel?.ReplyMessageId ?? 0;
        if (BaseModel != null)
            ModelType = (JsonModelType)BaseModel.MessageTypeId;
    }
}