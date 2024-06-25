using System.Text.Json.Serialization;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.DeviceMessage.Common;

namespace GXOVnT.Shared.DeviceMessage.Request;

public class DeleteSystemSettingsRequest() : BaseMessageModel(JsonModelType.DeleteSystemSettingsRequest)
{

    #region Properties
    [JsonPropertyName(JsonFieldNames.JsonFieldSystemPassword)] 
    public string SystemPassword { get; set; } = string.Empty;

    #endregion

}