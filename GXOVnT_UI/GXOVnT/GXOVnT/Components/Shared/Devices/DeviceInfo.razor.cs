using System.ComponentModel;
using GXOVnT.Services.Interfaces;
using GXOVnT.Services.Models;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.DeviceMessage.Request;
using GXOVnT.Shared.DeviceMessage.Response;
using Microsoft.AspNetCore.Components;

namespace GXOVnT.Components.Shared.Devices;

public partial class DeviceInfo : GXOVnTComponent
{
    
    #region Properties


    [Inject]
    private IDeviceService DeviceService { get; set; } = default!;
   
    [Parameter]
    public Services.Models.System? Device { get; set; } 
    
    private bool DeviceInformationGetExecuted { get; set; }
    
    private bool ComponentInitialized { get; set; }
    
    private bool ConnectedToDevice { get; set; }
    
    private bool FailedToGetInformation { get; set; }
    
    private bool DataLoaded { get; set; }
    
    private GetSystemSettingsResponse? DeviceSettingsResponse { get; set; }

    private bool NeedConfirmation => StepRequiresConfirmation();
    
    private bool ConfirmedContinue { get; set; }
    #endregion

    #region Override

    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        SetWizardForwardEnabled(false);
        
        DeviceService.PropertyChanged -= DeviceServiceOnPropertyChanged;
        DeviceService.PropertyChanged += DeviceServiceOnPropertyChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender) 
            return;

        ComponentInitialized = true;
        DeviceInformationGetExecuted = false;
        SetWizardForwardEnabled(false);
        await InvokeAsync(StateHasChanged);
        await GetDeviceInfo();
    }

    #endregion

    #region Methods
    private async Task GetDeviceInfo()
    {
        try
        {
            DeviceInformationGetExecuted = true;
            DataLoaded = false;
            ConfirmedContinue = false;
            SetWizardForwardEnabled(false);
            
            if (Device == null)
                return;

            var responseModel = await DeviceService.GetDeviceInfoAsync(Device);
                
            DataLoaded = true;
            DeviceSettingsResponse = responseModel;
            
            if (!StepRequiresConfirmation())
                SetWizardForwardEnabled(true);
        }
        catch (Exception)
        {
            FailedToGetInformation = true;
            LogService.LogError("There was an internal error retrieving the device information");
        }
        finally
        {
            SetBusyValues(false);
        }
    }
    
    
    private bool StepRequiresConfirmation()
    {
        if (DeviceSettingsResponse == null)
            return false;
        
        if (DeviceSettingsResponse.SystemConfigured)
            return true;

        return (DeviceSettingsResponse.GXOVnTSystemType?.Id ?? SystemType.UnInitialized.Id) !=
               SystemType.UnInitialized.Id;
    }

    #endregion

    #region Event Callbacks
    private void OnConfirmChanged(bool value)
    {
        ConfirmedContinue = value;
        SetWizardForwardEnabled(ConfirmedContinue);
    }
    
    private async void DeviceServiceOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        SetBusyValues(DeviceService.IsBusy, DeviceService.BusyText);
        
        await InvokeAsync(StateHasChanged);
    }

    #endregion
    
}