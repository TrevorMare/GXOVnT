using System.Text.Json.Serialization;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.DeviceMessage.Common;

namespace GXOVnT.Shared.DeviceMessage.Request;

public class SetSystemBootModeRequest() : BaseMessageModel(JsonModelType.SetSystemBootModeRequest)
{

    #region Properties
    [JsonPropertyName(JsonFieldNames.JsonFieldSystemBootMode)]
    public int SystemBootMode { get; set; }

    #endregion
    
}