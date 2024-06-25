using System.Text.Json.Serialization;
using GXOVnT.Shared.Common;

namespace GXOVnT.Shared.DeviceMessage.Common;

/// <summary>
/// Base message model that contains the details on how to serialize/deserialize the content of the message
/// </summary>
public class BaseMessageModel
{

    #region Properties

    /// <summary>
    /// A unique Id shared between the device and this software indicating the type to serialize/deserialize into
    /// </summary>
    [JsonPropertyName(JsonFieldNames.JsonFieldMessageTypeId)]
    public short MessageTypeId { get; set; } 
    
    /// <summary>
    /// A value indicating if this message is in response to another message sent to the device.
    /// </summary>
    [JsonPropertyName(JsonFieldNames.JsonFieldReplyMessageId)]
    public short ReplyMessageId { get; set; }

    #endregion

    #region ctor
    
    public BaseMessageModel()
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