using System.Text.Json.Serialization;
using GXOVnT.Shared.Common;

namespace GXOVnT.Shared.JsonModels;

public class RequestSetSystemSettingsModel : BaseModel
{
    
    [JsonPropertyName("systemName")]
    public string SystemName { get; set; } = string.Empty;

    [JsonPropertyName("systemConfigured")]
    public bool SystemConfigured { get; set; }
    
    public RequestSetSystemSettingsModel()
    {
        MessageTypeId = (int)JsonModelType.RequestSetSystemSettings;
    }
}