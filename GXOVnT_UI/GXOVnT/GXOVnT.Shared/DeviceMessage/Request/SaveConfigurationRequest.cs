using GXOVnT.Shared.Common;
using GXOVnT.Shared.DeviceMessage;

namespace GXOVnT.Shared.JsonModels;

public class RequestSaveConfigurationMessageModel : BaseMessageModel
{
    
    public RequestSaveConfigurationMessageModel()
    {
        MessageTypeId = (int)JsonModelType.RequestSaveConfiguration;
    }
}