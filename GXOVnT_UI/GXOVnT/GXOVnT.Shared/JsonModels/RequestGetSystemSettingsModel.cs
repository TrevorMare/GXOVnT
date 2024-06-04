using GXOVnT.Shared.Common;

namespace GXOVnT.Shared.JsonModels;

public class RequestGetSystemSettingsModel : BaseModel
{
    public RequestGetSystemSettingsModel()
    {
        MessageTypeId = (int)JsonModelType.RequestGetSystemSettings;
    }
}