using System.Text.Json.Serialization;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.DeviceMessage.Common;

namespace GXOVnT.Shared.DeviceMessage.Response;

/// <summary>
/// Basic echo model
/// </summary>
public class EchoResponse() : BaseMessageModel(JsonModelType.EchoResponse)
{

    #region Properties

    [JsonPropertyName(JsonFieldNames.JsonFieldEchoMessage)] 
    public string EchoMessage { get; set; } = string.Empty;

    #endregion
    

}