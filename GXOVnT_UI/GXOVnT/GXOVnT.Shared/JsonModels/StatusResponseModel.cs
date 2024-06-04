using System.Text.Json.Serialization;
using GXOVnT.Shared.Common;

namespace GXOVnT.Shared.JsonModels;

public class StatusResponseModel : BaseModel
{

    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; } = 0;
    
    [JsonPropertyName("statusMessage")]
    public string StatusMessage { get; set; } = string.Empty;
    
    public StatusResponseModel()
    {
        MessageTypeId = (int)JsonModelType.StatusResponse;
    }
}