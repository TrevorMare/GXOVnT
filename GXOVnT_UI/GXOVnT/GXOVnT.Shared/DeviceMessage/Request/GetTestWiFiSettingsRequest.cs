using GXOVnT.Shared.Common;
using GXOVnT.Shared.DeviceMessage;

namespace GXOVnT.Shared.JsonModels;

public class RequestLastWiFiTestResultMessageModel : BaseMessageModel
{
    public RequestLastWiFiTestResultMessageModel()
    {
        MessageTypeId = (int)JsonModelType.RequestLastTestWiFiSettingsResult;
    }
}