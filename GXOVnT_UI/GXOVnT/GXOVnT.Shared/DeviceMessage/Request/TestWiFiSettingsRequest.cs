using System.Text.Json.Serialization;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.DeviceMessage.Common;

namespace GXOVnT.Shared.DeviceMessage.Request;

public class TestWiFiSettingsRequest() : BaseMessageModel(JsonModelType.TestWiFiSettingsRequest)
{
    
    #region Properties
    
    [JsonPropertyName(JsonFieldNames.JsonFieldWiFiSsid)] 
    public string WiFiSsid { get; set; } = string.Empty;
    
    [JsonPropertyName(JsonFieldNames.JsonFieldWiFiPassword)] 
    public string WiFiPassword { get; set; } = string.Empty;

    #endregion
    
}