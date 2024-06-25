using System.Text.Json.Serialization;
using GXOVnT.Shared.Common;

namespace GXOVnT.Shared.DeviceMessage.Request;

/// <summary>
/// Basic echo model
/// </summary>
public class ResponseEchoMessageModel() : BaseMessageModel(JsonModelType.ResponseEcho)
{

    #region Properties

    [JsonPropertyName("echoMessage")] 
    public string EchoMessage { get; set; } = string.Empty;

    #endregion
    

}