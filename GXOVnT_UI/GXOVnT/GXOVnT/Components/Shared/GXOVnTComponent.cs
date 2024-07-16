using System.ComponentModel;
using GXOVnT.Shared;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.Interfaces;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GXOVnT.Components.Shared;

public class GXOVnTComponent : ComponentBase
{

    #region Members

    protected StateObject? AttachedViewModelStateObject;

    #endregion
    
    #region Properties

        
    [Inject]
    protected IDialogService DialogService { get; set; } = default!;
    
    [Inject]
    protected ILogService LogService { get; set; } = default!;
    
    
    #endregion

    #region Override

    protected override void OnInitialized()
    {
        base.OnInitialized();
        InitializeViewModel();
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

    
    #region Event Callbacks

    private async void WizardStepModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    private async void AttachedViewModelStateObjectOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        await OnViewModelPropertyChanged(sender, e);
        await InvokeAsync(StateHasChanged);
    }

    protected virtual Task OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        return Task.CompletedTask;
    }

    #endregion
 
    
    
}