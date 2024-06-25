using GXOVnT.Shared.Common;
using GXOVnT.Shared.DeviceMessage.Common;

namespace GXOVnT.Shared.DeviceMessage.Request;

/// <summary>
/// Model to request the system configuration settings
/// </summary>
public class GetSystemSettingsRequest() 
    : BaseMessageModel(JsonModelType.GetSystemSettingsRequest)
{
}