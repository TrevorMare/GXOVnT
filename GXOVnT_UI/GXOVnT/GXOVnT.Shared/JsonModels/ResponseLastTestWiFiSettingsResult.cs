using System.Text.Json.Serialization;
using GXOVnT.Shared.Common;

namespace GXOVnT.Shared.JsonModels;

public class ResponseLastTestWiFiSettingsResult : BaseModel
{

    #region Properties

    
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; } = 0;
    
    [JsonPropertyName("statusMessage")]
    public string StatusMessage { get; set; } = string.Empty;

    [JsonPropertyName("wifiSSID")]
    public string WiFiSSID { get; set; } = string.Empty;

    [JsonPropertyName("wifiPassword")]
    public string WiFiPassword { get; set; } = string.Empty;

    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("tested")]
    public bool Tested { get; set; }
    #endregion
    
    public ResponseLastTestWiFiSettingsResult()
    {
        MessageTypeId = (int)JsonModelType.ResponseLastTestWiFiSettingsResult;
    }
}