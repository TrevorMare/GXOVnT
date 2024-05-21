using Microsoft.AspNetCore.Components;

namespace GXOVnT.Components.Shared;

public partial class DeviceConfigurator : ComponentBase
{
    
    [Parameter]public EventCallback DeviceConfigurationCancelled { get; set; }
    
}