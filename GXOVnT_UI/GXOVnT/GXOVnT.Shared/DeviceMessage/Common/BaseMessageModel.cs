using System.Text.Json.Serialization;
using GXOVnT.Shared.Common;

namespace GXOVnT.Shared.DeviceMessage;

/// <summary>
/// Base message model that contains the details on how to serialize/deserialize the content of the message
/// </summary>
public class BaseMessageModel
{

    #region Properties

    /// <summary>
    /// A unique Id shared between the device and this software indicating the type to serialize/deserialize into
    /// </summary>
    [JsonPropertyName("messageTypeId")]
    public short MessageTypeId { get; set; } 
    
    /// <summary>
    /// A value indicating if this message is in response to another message sent to the device.
    /// </summary>
    [JsonPropertyName("replyMessageId")]
    public short ReplyMessageId { get; set; }

    #endregion

    #region ctor

    internal BaseMessageModel()
    {
        
    }
    
    internal BaseMessageModel(short messageTypeId)
    {
        MessageTypeId = messageTypeId;
    }

    internal BaseMessageModel(JsonModelType modelType)
    {
        MessageTypeId = (short)modelType;
    }

    #endregion
    
}