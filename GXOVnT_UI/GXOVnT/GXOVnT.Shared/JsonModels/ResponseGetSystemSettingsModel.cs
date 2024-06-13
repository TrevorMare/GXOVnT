using System.Text.Json.Serialization;
using GXOVnT.Shared.Common;

namespace GXOVnT.Shared.JsonModels;

public class ResponseGetSystemSettingsModel : BaseModel
{

    [JsonPropertyName("systemName")]
    public string SystemName { get; set; } = string.Empty;

    [JsonPropertyName("systemId")] 
    public string SystemId { get; set; } = string.Empty;

    [JsonPropertyName("systemConfigured")]
    public bool SystemConfigured { get; set; }

    [JsonPropertyName("systemType")]
    public int SystemType { get; set; }

    [JsonIgnore]
    public GXOVnTSystemType? GXOVnTSystemType => Enumeration.FromValue<GXOVnTSystemType>(SystemType);
    
    [JsonPropertyName("firmwareVersion")] 
    public string FirmwareVersion { get; set; } = string.Empty;
    
    [JsonPropertyName("wifiSSID")] 
    public string WiFiSSID { get; set; } = string.Empty;
    
    [JsonPropertyName("wifiPassword")] 
    public string WiFiPassword { get; set; } = string.Empty;

    public ResponseGetSystemSettingsModel()
    {
        MessageTypeId = (int)JsonModelType.ResponseSystemSettings;
    }
}