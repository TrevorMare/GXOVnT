using System.ComponentModel;
using GXOVnT.Services.Interfaces;
using GXOVnT.Shared;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.Interfaces;
using GXOVnT.ViewModels.Wizards;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GXOVnT.Components.Shared;

public class GXOVnTComponent : ComponentBase
{

    #region Members

    protected StateObject? AttachedViewModelStateObject;
    
    
    
    
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
        InitializeViewModel();
        

        if (WizardStepModel == null) return;
        
        WizardStepModel.PropertyChanged -= WizardStepModelOnPropertyChanged;
        WizardStepModel.PropertyChanged += WizardStepModelOnPropertyChanged;
    }
    
    
    protected virtual void InitializeViewModel() {}

    protected virtual void SetAttachedViewModelStateObject<T>(T? viewModelStateObject) where T : StateObject 
    {
        // Check if we did not previously have an attached model, if so remove the event handlers
        if (AttachedViewModelStateObject != null)
            AttachedViewModelStateObject.PropertyChanged -= AttachedViewModelStateObjectOnPropertyChanged;

        // Set the view model to the passed input
        AttachedViewModelStateObject = viewModelStateObject;

        // If the input state object is null, we need to initialize the value
        if (AttachedViewModelStateObject == null)
            AttachedViewModelStateObject = AppService.ServiceProvider.GetRequiredService<T>();
        
        // Now attach the event handler
        if (AttachedViewModelStateObject != null)
            AttachedViewModelStateObject.PropertyChanged += AttachedViewModelStateObjectOnPropertyChanged;
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

    private async void AttachedViewModelStateObjectOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    #endregion
 
    
    
}