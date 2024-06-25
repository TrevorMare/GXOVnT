using System.Text.Json.Serialization;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.DeviceMessage.Common;

namespace GXOVnT.Shared.DeviceMessage.Response;

public class GetSystemSettingsResponse() : BaseMessageModel(JsonModelType.GetSystemSettingsResponse)
{

    #region Properties

    [JsonPropertyName(JsonFieldNames.JsonFieldSystemName)]
    public string SystemName { get; set; } = string.Empty;

    [JsonPropertyName(JsonFieldNames.JsonFieldSystemId)] 
    public string SystemId { get; set; } = string.Empty;

    [JsonPropertyName(JsonFieldNames.JsonFieldSystemConfigured)]
    public bool SystemConfigured { get; set; }

    [JsonPropertyName(JsonFieldNames.JsonFieldSystemType)]
    public int SystemType { get; set; }

    [JsonIgnore]
    public GXOVnTSystemType? GXOVnTSystemType => Enumeration.FromValue<GXOVnTSystemType>(SystemType);
    
    [JsonPropertyName(JsonFieldNames.JsonFieldFirmwareVersion)] 
    public string FirmwareVersion { get; set; } = string.Empty;
    
    [JsonPropertyName(JsonFieldNames.JsonFieldWiFiSsid)] 
    public string WiFiSsid { get; set; } = string.Empty;
    
    [JsonPropertyName(JsonFieldNames.JsonFieldWiFiPassword)] 
    public string WiFiPassword { get; set; } = string.Empty;

    #endregion
   

 
}