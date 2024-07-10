using System.ComponentModel;
using GXOVnT.ViewModels.Wizards;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GXOVnT.Components.Shared.WizardComponent;

public partial class WizardComponent<TWizardModelType> : ComponentBase where TWizardModelType: WizardComponentModel
{

    #region Members
    
    private WizardStepModel? CurrentStepModel => WizardComponentModel.CurrentWizardStep;

    #endregion
    
    #region Properties

    [Inject]
    private IDialogService DialogService { get; set; } = default!;

    [Parameter]
    public TWizardModelType WizardComponentModel { get; set; }
    
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

        if (!firstRender) return;
        WizardComponentModel.PropertyChanged -= WizardComponentModelOnPropertyChanged;
        WizardComponentModel.PropertyChanged += WizardComponentModelOnPropertyChanged;
    }
    #endregion
    
    #region Methods
    public void AddWizardStep(WizardStepModel wizardStepModel)
    {
        WizardComponentModel.AddWizardStep(wizardStepModel);       
    }
    #endregion
    
    #region Event Callbacks
    private async void WizardComponentModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    private async Task CancelButtonClicked()
    {
        if (ConfirmCancelWizard)
        {
            var result = await DialogService.ShowMessageBox(
                "Warning", 
                "Are you sure you want to quit the wizard? All progress will be lost.", 
                yesText:"Yes", cancelText:"No");
        
            if (result == true)
                await WizardCancelled.InvokeAsync();
            
            StateHasChanged();
        }
        else
        {
            await WizardCancelled.InvokeAsync();
        }
    }
    
    private async Task BackButtonClicked()
    {
        if (WizardComponentModel.GotoPreviousStep())
            await InvokeAsync(StateHasChanged);
    }
    
    private async Task NextButtonClicked()
    {
        
        if (WizardComponentModel.GotoNextStep())
            await InvokeAsync(StateHasChanged);
    }
    #endregion
    
}