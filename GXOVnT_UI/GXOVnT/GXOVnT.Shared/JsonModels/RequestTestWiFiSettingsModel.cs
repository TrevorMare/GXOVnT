using System.Text.Json.Serialization;
using GXOVnT.Shared.Common;

namespace GXOVnT.Shared.JsonModels;

public class RequestTestWiFiSettingsModel : BaseModel
{
    [JsonPropertyName("wifiSSID")] 
    public string WiFiSSID { get; set; } = string.Empty;
    
    [JsonPropertyName("wifiPassword")] 
    public string WiFiPassword { get; set; } = string.Empty;
    
    public RequestTestWiFiSettingsModel()
    {
        MessageTypeId = (int)JsonModelType.RequestTestWiFiSettings;
    }
}