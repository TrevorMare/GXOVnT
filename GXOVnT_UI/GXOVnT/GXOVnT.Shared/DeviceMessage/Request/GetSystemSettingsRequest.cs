using GXOVnT.Shared.Common;
using GXOVnT.Shared.DeviceMessage;

namespace GXOVnT.Shared.JsonModels;

/// <summary>
/// Model to request the system configuration settings
/// </summary>
public class SystemSettingsRequest() 
    : BaseMessageModel(JsonModelType.RequestGetSystemSettings)
{
}