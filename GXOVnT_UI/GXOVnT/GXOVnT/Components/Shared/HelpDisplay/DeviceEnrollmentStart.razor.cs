using System.ComponentModel;
using Microsoft.AspNetCore.Components;

namespace GXOVnT.Components.Shared.HelpDisplay;

public partial class DeviceEnrollmentStart : ComponentBase
{
    
    [CascadingParameter]
    public Models.WizardStepModel? WizardStepModel { get; set; }


    protected override void OnInitialized()
    {
        base.OnInitialized();

        
        
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (!firstRender) return;

    }
}