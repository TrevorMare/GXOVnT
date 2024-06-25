using System.Text.Json.Serialization;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.DeviceMessage.Common;

namespace GXOVnT.Shared.DeviceMessage.Response;

public class StatusResponse() : BaseMessageModel(JsonModelType.StatusResponse)
{

    #region Properties

    [JsonPropertyName(JsonFieldNames.JsonFieldStatusCode)]
    public int StatusCode { get; set; } = 0;
    
    [JsonPropertyName(JsonFieldNames.JsonFieldStatusMessage)]
    public string StatusMessage { get; set; } = string.Empty;

    #endregion

    
  
}