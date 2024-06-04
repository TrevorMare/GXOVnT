using GXOVnT.Shared.Common;

namespace GXOVnT.Shared.JsonModels;

public class RequestRebootModel : BaseModel
{

    public RequestRebootModel()
    {
        MessageTypeId = (int)JsonModelType.RequestReboot;
    }
    
}