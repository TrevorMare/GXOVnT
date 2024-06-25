using System.Text.Json.Serialization;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.DeviceMessage.Common;

namespace GXOVnT.Shared.DeviceMessage.Response;

public class GetTestWiFiSettingsResponse() : BaseMessageModel(JsonModelType.GetTestWiFiSettingsResultResponse)
{

    #region Properties
    [JsonPropertyName(JsonFieldNames.JsonFieldStatusCode)]
    public int StatusCode { get; set; } = 0;
    
    [JsonPropertyName(JsonFieldNames.JsonFieldStatusMessage)]
    public string StatusMessage { get; set; } = string.Empty;

    [JsonPropertyName(JsonFieldNames.JsonFieldWiFiSsid)]
    public string WiFiSsid { get; set; } = string.Empty;

    [JsonPropertyName(JsonFieldNames.JsonFieldWiFiPassword)]
    public string WiFiPassword { get; set; } = string.Empty;

    [JsonPropertyName(JsonFieldNames.JsonFieldSuccess)]
    public bool Success { get; set; }

    [JsonPropertyName(JsonFieldNames.JsonFieldTested)]
    public bool Tested { get; set; }
    #endregion
 
}