using System.ComponentModel;
using GXOVnT.Shared.Common;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GXOVnT.Components.Shared.WizardComponent;

public partial class WizardComponent : ComponentBase 
{

    #region Properties

    [Inject]
    private IDialogService DialogService { get; set; } = default!;

    [Parameter]
    public WizardSchema? WizardSchema { get; set; }
    
    [Parameter]
    public string? WizardTitle { get; set; } = string.Empty;
    
    [Parameter]
    public EventCallback WizardCancelled { get; set; }
    
    [Parameter]
    public EventCallback WizardCompleted { get; set; }

    [Parameter]
    public bool ConfirmCancelWizard { get; set; } = true;
    
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    #endregion

    #region Overrides

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (!firstRender || WizardSchema == null) return;
        WizardSchema.PropertyChanged -= WizardSchemaOnPropertyChanged;
        WizardSchema.PropertyChanged += WizardSchemaOnPropertyChanged;
        WizardSchema.OnWizardComplete += WizardSchemaOnOnWizardComplete;
    }
    #endregion
    
    #region Methods
    public void AddWizardStep(WizardStep wizardStep)
    {
        WizardSchema?.AddWizardStep(wizardStep.WizardSchemaStep);       
    }
    #endregion
    
    #region Event Callbacks
    private async void WizardSchemaOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }
    
    private async void WizardSchemaOnOnWizardComplete(object? sender, EventArgs e)
    {
        await WizardCompleted.InvokeAsync();
    }


    private async Task CancelButtonClicked()
    {
        if (ConfirmCancelWizard)
        {
            var result = await DialogService.ShowMessageBox(
                "Warning", 
                "Are you sure you want to quit the wizard? All progress will be lost.", 
                yesText:"Yes", cancelText:"No");
        
            if (WizardSchema != null)
                await WizardSchema.OnCancelWizard();
            
            if (result == true)
                await WizardCancelled.InvokeAsync();
            
            StateHasChanged();
        }
        else
        {
            if (WizardSchema != null)
                await WizardSchema.OnCancelWizard();
            
            await WizardCancelled.InvokeAsync();
        }
    }
    
    private async Task BackButtonClicked()
    {
        if (WizardSchema != null)
            await WizardSchema.GoToPreviousStep();
    }
    
    private async Task NextButtonClicked()
    {
        if (WizardSchema != null)
            await WizardSchema.GoToNextStep();
    }
    #endregion
    
}