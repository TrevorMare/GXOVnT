using System.Text.Json.Serialization;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.DeviceMessage;

namespace GXOVnT.Shared.JsonModels;

public class RequestTestWiFiSettingsMessageModel : BaseMessageModel
{
    [JsonPropertyName("wifiSSID")] 
    public string WiFiSSID { get; set; } = string.Empty;
    
    [JsonPropertyName("wifiPassword")] 
    public string WiFiPassword { get; set; } = string.Empty;
    
    public RequestTestWiFiSettingsMessageModel()
    {
        MessageTypeId = (int)JsonModelType.RequestTestWiFiSettings;
    }
}