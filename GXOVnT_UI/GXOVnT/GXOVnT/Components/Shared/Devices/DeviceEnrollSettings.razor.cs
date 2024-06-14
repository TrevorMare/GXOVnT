using System.ComponentModel;
using GXOVnT.Services.Interfaces;
using GXOVnT.Services.Models;
using GXOVnT.Shared.JsonModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GXOVnT.Components.Shared.Devices;

public partial class DeviceEnrollSettings : GXOVnTComponent
{

    #region Members
    private MudForm _form;
    private bool success;
    private string[] errors = { };
    #endregion
    
    #region Properties

    [Inject]
    private IBluetoothService BluetoothService { get; set; } = default!;
    
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

        await InvokeAsync(StateHasChanged);
        
        await GetDeviceInfo();
    }

    #endregion
    
    #region Methods

    private async Task TestDeviceWiFiSettings()
    {

        try
        {
            IsBusy = true;

            if (GXOVnTDevice == null)
                return;

            var requestTestWifiModel = new RequestTestWiFiSettingsModel()
            {
                WiFiPassword = "X@Kbi-Rh3$",
                WiFiSSID = "HouseMare"
            };
            var responseTestWifiModel =
                await MessageOrchestrator.SendMessage<RequestTestWiFiSettingsModel, StatusResponseModel>(
                    requestTestWifiModel);

            if (responseTestWifiModel.StatusCode != 200)
                return;

            // Send the reboot command

            await MessageOrchestrator.SendMessage(new RequestRebootModel());

            await BluetoothService.DisConnectFromDevice();

            using var reConnectCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(1));

            await BluetoothService.ReConnectToDeviceWhenAvailable(GXOVnTDevice.Id,
                reConnectCancellationTokenSource.Token);


        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);

        }
        finally
        {
            IsBusy = true;
        }
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

    private async void MessageOrchestratorOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    #endregion

    
    
    
}