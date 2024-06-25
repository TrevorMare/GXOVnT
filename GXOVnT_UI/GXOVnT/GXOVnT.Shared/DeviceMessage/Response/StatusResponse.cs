using System.Text.Json.Serialization;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.DeviceMessage;

namespace GXOVnT.Shared.JsonModels;

public class ResponseStatusMessageModel() : BaseMessageModel(JsonModelType.ResponseStatus)
{

    #region Properties

    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; } = 0;
    
    [JsonPropertyName("statusMessage")]
    public string StatusMessage { get; set; } = string.Empty;

    #endregion

    
  
}