using System.Text.Json.Serialization;
using GXOVnT.Shared.Common;

namespace GXOVnT.Shared.DeviceMessage.Response;

public class ResponseLastTestWiFiSettingsResult() : BaseMessageModel(JsonModelType.ResponseLastTestWiFiSettingsResult)
{

    #region Properties
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; } = 0;
    
    [JsonPropertyName("statusMessage")]
    public string StatusMessage { get; set; } = string.Empty;

    [JsonPropertyName("wifiSsid")]
    public string WiFiSSID { get; set; } = string.Empty;

    [JsonPropertyName("wifiPassword")]
    public string WiFiPassword { get; set; } = string.Empty;

    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("tested")]
    public bool Tested { get; set; }
    #endregion
 
}