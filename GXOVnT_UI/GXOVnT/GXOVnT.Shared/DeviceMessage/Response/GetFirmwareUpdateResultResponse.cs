using System.Text.Json.Serialization;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.DeviceMessage.Common;

namespace GXOVnT.Shared.DeviceMessage.Response;

public class GetFirmwareUpdateResultResponse() : BaseMessageModel(JsonModelType.GetFirmwareUpdateResultResponse)
{

    [JsonPropertyName(JsonFieldNames.JsonFieldFirmwareVersions)]
    public List<FirmwareVersionResult> FirmwareVersions { get; set; } = new();

}

public class FirmwareVersionResult
{
    [JsonPropertyName(JsonFieldNames.JsonFieldFirmwareVersion)]
    public string FirmwareVersion { get; set; } = string.Empty;
    
    [JsonPropertyName(JsonFieldNames.JsonFieldDownloadLocation)]
    public string DownloadLocation { get; set; } = string.Empty;
    
    [JsonPropertyName(JsonFieldNames.JsonFieldVersionNumber)]
    public string VersionNumber { get; set; } = string.Empty;
    
    [JsonPropertyName(JsonFieldNames.JsonFieldSystemType)]
    public int SystemType { get; set; }
    
    [JsonPropertyName(JsonFieldNames.JsonFieldFirmwareVersionInstalled)]
    public bool InstalledVersion { get; set; }
}
