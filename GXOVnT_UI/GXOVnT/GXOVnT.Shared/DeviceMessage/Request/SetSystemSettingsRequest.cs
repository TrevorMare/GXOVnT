using System.Text.Json.Serialization;
using GXOVnT.Shared.Common;

namespace GXOVnT.Shared.DeviceMessage.Request;

public class RequestSetSystemSettingsMessageModel : BaseMessageModel
{
    
    [JsonPropertyName("systemName")]
    public string SystemName { get; set; } = string.Empty;

    [JsonPropertyName("systemConfigured")]
    public bool SystemConfigured { get; set; }
    
    [JsonPropertyName("systemType")]
    public int SystemType { get; set; }
    
    [JsonPropertyName("wifiSSID")] 
    public string WiFiSSID { get; set; } = string.Empty;
    
    [JsonPropertyName("wifiPassword")] 
    public string WiFiPassword { get; set; } = string.Empty;
    
    public RequestSetSystemSettingsMessageModel()
    {
        MessageTypeId = (int)JsonModelType.RequestSetSystemSettings;
    }
}