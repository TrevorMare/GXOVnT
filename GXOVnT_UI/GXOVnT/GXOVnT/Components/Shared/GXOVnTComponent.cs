using System.ComponentModel;
using GXOVnT.Models;
using GXOVnT.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GXOVnT.Components.Shared;

public class GXOVnTComponent : ComponentBase
{

    #region Members

    private bool _isBusy;

    #endregion
    
    #region Properties

        
    [Inject]
    protected IDialogService DialogService { get; set; } = default!;
    
    [Inject]
    protected ILogService LogService { get; set; } = default!;
    
    [CascadingParameter]
    protected WizardStepModel? WizardStepModel { get; set; }

    protected bool IsBusy
    {
        get => WizardStepModel?.IsBusy ?? _isBusy;
        set
        {
            if (WizardStepModel != null)
                WizardStepModel.IsBusy = value;
            _isBusy = value;
        } 
    }
    #endregion

    #region Override

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (WizardStepModel == null) return;
        
        WizardStepModel.PropertyChanged -= WizardStepModelOnPropertyChanged;
        WizardStepModel.PropertyChanged += WizardStepModelOnPropertyChanged;
    }

    #endregion

    #region Protected Methods

    protected void SetWizardForwardEnabled(bool value)
    {
        if (WizardStepModel == null) return;
        WizardStepModel.ForwardEnabled = value;
    }

    protected void SetWizardBackEnabled(bool value)
    {
        if (WizardStepModel == null) return;
        WizardStepModel.BackEnabled = value;
    }
    #endregion
    
    #region Event Callbacks

    private async void WizardStepModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    #endregion
    
    
}