using System.ComponentModel;
using GXOVnT.Shared.Common;
using Microsoft.AspNetCore.Components;

namespace GXOVnT.Components.Shared.WizardComponent;

public partial class WizardStep : ComponentBase  
{
    
    #region Properties

    private bool IsCurrentStep => WizardSchemaStep.IsCurrentStep;
    
    public WizardSchemaStep WizardSchemaStep { get; set; } = new();
    
    [Parameter]
    public string? StepName 
    {
        get => WizardSchemaStep.StepName;
        set => WizardSchemaStep.StepName = value;
    }
    
    [Parameter]
    public string? StepTitle 
    {
        get => WizardSchemaStep.StepTitle;
        set => WizardSchemaStep.StepTitle = value;
    } 
    
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    
    [CascadingParameter] 
    private WizardComponent? WizardComponent { get; set; } 
    
    #endregion

    #region Overrides

    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        WizardSchemaStep.PropertyChanged -= WizardSchemaStepOnPropertyChanged;
        WizardSchemaStep.PropertyChanged += WizardSchemaStepOnPropertyChanged;
        
        WizardComponent?.AddWizardStep(this);
    }
    #endregion

    #region Event Callbacks

    private async void WizardSchemaStepOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    #endregion
    
}