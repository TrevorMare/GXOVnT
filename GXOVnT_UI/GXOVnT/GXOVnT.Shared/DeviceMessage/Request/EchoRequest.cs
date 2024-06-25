using System.Text.Json.Serialization;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.DeviceMessage.Common;

namespace GXOVnT.Shared.DeviceMessage.Request;

/// <summary>
/// Basic echo model
/// </summary>
public class EchoRequest() : BaseMessageModel(JsonModelType.EchoRequest)
{

    #region Properties

    [JsonPropertyName(JsonFieldNames.JsonFieldEchoMessage)] 
    public string EchoMessage { get; set; } = string.Empty;

    #endregion
    

}