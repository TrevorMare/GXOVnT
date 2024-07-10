using System.ComponentModel;
using GXOVnT.Services.Interfaces;
using GXOVnT.Shared.Interfaces;
using GXOVnT.ViewModels.Wizards;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GXOVnT.Components.Shared;

public class GXOVnTComponent : ComponentBase
{

    #region Members

    private bool _isBusy;
    private string? _busyText;

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
    
    protected string? BusyText
    {
        get => WizardStepModel?.BusyText ?? _busyText;
        set
        {
            if (WizardStepModel != null)
                WizardStepModel.BusyText = value;
            _busyText = value;
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

    protected void SetBusyValues(bool isBusy, string? busyText = default)
    {
        IsBusy = isBusy;
        BusyText = isBusy ? busyText : "";
    }

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