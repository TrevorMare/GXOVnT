using System.Text.Json.Serialization;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.DeviceMessage.Common;

namespace GXOVnT.Shared.DeviceMessage.Request;

public class SetSystemSettingsRequest() : BaseMessageModel(JsonModelType.SetSystemSettingsRequest)
{

    #region Properties

    [JsonPropertyName(JsonFieldNames.JsonFieldSystemName)]
    public string SystemName { get; set; } = string.Empty;

    [JsonPropertyName(JsonFieldNames.JsonFieldSystemConfigured)]
    public bool SystemConfigured { get; set; }
    
    [JsonPropertyName(JsonFieldNames.JsonFieldSystemType)]
    public int SystemType { get; set; }
    
    [JsonPropertyName(JsonFieldNames.JsonFieldWiFiSsid)] 
    public string WiFiSsid { get; set; } = string.Empty;
    
    [JsonPropertyName(JsonFieldNames.JsonFieldWiFiPassword)] 
    public string WiFiPassword { get; set; } = string.Empty;

    #endregion

}