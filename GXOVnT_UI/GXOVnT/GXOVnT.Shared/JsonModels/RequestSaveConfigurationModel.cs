using GXOVnT.Shared.Common;

namespace GXOVnT.Shared.JsonModels;

public class RequestSaveConfigurationModel : BaseModel
{
    
    public RequestSaveConfigurationModel()
    {
        MessageTypeId = (int)JsonModelType.RequestSaveConfiguration;
    }
}