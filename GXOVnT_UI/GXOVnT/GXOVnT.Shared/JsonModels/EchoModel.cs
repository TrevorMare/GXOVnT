using System.Text.Json.Serialization;
using GXOVnT.Shared.Common;

namespace GXOVnT.Shared.JsonModels;

public class EchoModel : BaseModel
{

    public EchoModel()
    {
        MessageTypeId = (int)JsonModelType.EchoRequest;
    }

    [JsonPropertyName("echoMessage")] 
    public string EchoMessage { get; set; } = string.Empty;


}