using GXOVnT.Shared.Common;

namespace GXOVnT.Shared.JsonModels;

public class RequestLastWiFiTestResultModel : BaseModel
{
    public RequestLastWiFiTestResultModel()
    {
        MessageTypeId = (int)JsonModelType.RequestLastTestWiFiSettingsResult;
    }
}