using System.ComponentModel;
using GXOVnT.Services.ViewModels;
using GXOVnT.Shared.Common;
using Microsoft.AspNetCore.Components;

namespace GXOVnT.Components.Shared.Devices;

public partial class DeviceInfo : GXOVnTComponent
{
    
    #region Properties

    /// <summary>
    /// The parameter is to be used in the context when a wizard model passes down a specific view model and should
    /// not be relied on that it will be set. The correct property to use is the standard view model object <see cref="ViewModel"/>
    /// </summary>
    [Parameter]
    public DeviceInfoViewModel? InitialViewModel { get; set; }
    
    /// <summary>
    /// This is a calculated view model that will either be a new view model from the service provider or
    /// the view model parameters
    /// </summary>
    private DeviceInfoViewModel ViewModel =>
        (DeviceInfoViewModel)AttachedViewModelStateObject!;
    
    private bool NeedConfirmation => StepRequiresConfirmation();
    
    private bool ConfirmedContinue { get; set; }
    #endregion

    #region Override
    protected override void InitializeViewModel()
    {
        // Set the internal view model object, this will either be from the wizard model or we should
        // initialize it from the service provider
        SetAttachedViewModelStateObject(InitialViewModel);
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // If it's the first render and this view model is not passed down from the 
        // wizard. The wizard will rather perform this step
        if (firstRender && InitialViewModel == null)
            await ViewModel.GetDeviceInfo(null);
    }

    #endregion

    #region Methods

    private async Task RetryGetDeviceInfo()
    {
        await ViewModel.GetDeviceInfo(null);
    }
    
    private bool StepRequiresConfirmation()
    {
        if (ViewModel.DeviceInfo == null)
            return false;
        
        if (ViewModel.DeviceInfo.SystemConfigured)
            return true;

        return (ViewModel.DeviceInfo.GXOVnTSystemType?.Id ?? SystemType.UnInitialized.Id) !=
               SystemType.UnInitialized.Id;
    }

    #endregion

    #region Event Callbacks
    private void OnConfirmChanged(bool value)
    {
        ConfirmedContinue = value;
    }
    #endregion
    
}