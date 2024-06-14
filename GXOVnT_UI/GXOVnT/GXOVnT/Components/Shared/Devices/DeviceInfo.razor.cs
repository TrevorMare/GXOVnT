using System.ComponentModel;
using GXOVnT.Services.Interfaces;
using GXOVnT.Services.Models;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.JsonModels;
using Microsoft.AspNetCore.Components;

namespace GXOVnT.Components.Shared.Devices;

public partial class DeviceInfo : GXOVnTComponent
{
    
    #region Properties

    [Inject]
    private IBluetoothService BluetoothService { get; set; } = default;
    
    [Inject]
    private IMessageOrchestrator MessageOrchestrator { get; set; } = default!;
   
    [Parameter]
    public GXOVnTDevice? GXOVnTDevice { get; set; } 
    
    private bool ComponentInitialized { get; set; }
    
    private bool ConnectedToDevice { get; set; }
    
    private bool FailedToGetInformation { get; set; }
    
    private bool FailedToConnect { get; set; }
    
    private bool DataLoaded { get; set; }
    
    private ResponseGetSystemSettingsModel? DeviceSettingsResponse { get; set; }

    private bool NeedConfirmation => StepRequiresConfirmation();
    
    private bool ConfirmedContinue { get; set; }
    #endregion

    #region Override

    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        SetWizardForwardEnabled(false);
        
        MessageOrchestrator.PropertyChanged -= MessageOrchestratorOnPropertyChanged;
        MessageOrchestrator.PropertyChanged += MessageOrchestratorOnPropertyChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender) 
            return;
        
        ComponentInitialized = true;
        ConnectedToDevice = false;
        FailedToGetInformation = false;
        DataLoaded = false;
        DeviceSettingsResponse = null;
        ConfirmedContinue = false;

        await InvokeAsync(StateHasChanged);
        
        await GetDeviceInfo();
    }

    #endregion

    #region Methods

    private bool StepRequiresConfirmation()
    {
        if (DeviceSettingsResponse == null)
            return false;
        
        if (DeviceSettingsResponse.SystemConfigured)
            return true;

        if ((DeviceSettingsResponse.GXOVnTSystemType?.Id ?? GXOVnTSystemType.UnInitialized.Id) !=
            GXOVnTSystemType.UnInitialized.Id)
            return true;

        return false;
    }

    private async Task GetDeviceInfo()
    {
        try
        {
            IsBusy = true;
            ConnectedToDevice = false;
            FailedToGetInformation = false;
            FailedToConnect = false;
            DataLoaded = false;
            
            SetWizardForwardEnabled(false);

            await InvokeAsync(StateHasChanged);

            if (GXOVnTDevice?.Device == null)
                return;

            ConnectedToDevice = await BluetoothService.ConnectToDevice(GXOVnTDevice.Device.Id, true);
            if (!ConnectedToDevice)
            {
                FailedToConnect = true;
                return;
            }

            var requestModel = new RequestGetSystemSettingsModel();
            var responseModel = await MessageOrchestrator.SendMessage<RequestGetSystemSettingsModel, ResponseGetSystemSettingsModel>(
                requestModel);

            if (responseModel == null)
            {
                FailedToGetInformation = true;
                return;
            }
                
            DataLoaded = true;
            DeviceSettingsResponse = responseModel;
            
            if (!StepRequiresConfirmation())
                SetWizardForwardEnabled(true);
        }
        catch (Exception)
        {
            LogService.LogError("There was an internal error retrieving the device information");
        }
        finally
        {
            IsBusy = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    #endregion

    #region Event Callbacks
    private void OnConfirmChanged(bool value)
    {
        ConfirmedContinue = value;
        SetWizardForwardEnabled(ConfirmedContinue);
    }
    
    private async void MessageOrchestratorOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    #endregion
    
}