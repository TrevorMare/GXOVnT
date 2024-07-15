using System.ComponentModel;
using GXOVnT.Services.Interfaces;
using GXOVnT.Services.Models;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.DeviceMessage.Request;
using GXOVnT.Shared.DeviceMessage.Response;
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
    public Services.Models.System? Device { get; set; }
    
    private bool DeviceInformationGetExecuted { get; set; }
    
    private bool ComponentInitialized { get; set; }
    
    private bool ConnectedToDevice { get; set; }
    
    private bool FailedToGetInformation { get; set; }
    
    private bool DataLoaded { get; set; }

    private SetSystemSettingsRequest SetSettingsMessageModel { get; set; } = new();
    #endregion

    #region Override

    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        
        
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
    
    private async Task GetDeviceInfo()
    {
        try
        {
            DeviceInformationGetExecuted = true;
            DataLoaded = false;
            SetSettingsMessageModel = new SetSystemSettingsRequest();
            
           
            
            if (Device == null)
                return;
            
           
            ConnectedToDevice = await Device.ConnectToDeviceAsync();

            if (!ConnectedToDevice)
                return;

           
            var requestModel = new GetSystemSettingsRequest();
            var responseModel = await MessageOrchestrator.SendMessage<GetSystemSettingsRequest, GetSystemSettingsResponse>(
                requestModel, Device);

            if (responseModel == null)
            {
                FailedToGetInformation = true;
                return;
            }

            SetSettingsMessageModel.SystemConfigured = responseModel.SystemConfigured;
            SetSettingsMessageModel.SystemType = responseModel.SystemType;
            SetSettingsMessageModel.SystemName = responseModel.SystemName;
            SetSettingsMessageModel.WiFiPassword = responseModel.WiFiPassword;
            SetSettingsMessageModel.WiFiSsid = responseModel.WiFiSsid;
            DataLoaded = true;
  
        }
        catch (Exception)
        {
            LogService.LogError("There was an internal error retrieving the device information");
        }
        finally
        {
           
        }
    }

    private async Task RebootDeviceAsync()
    {
        try
        {
            
            if (Device == null)
                return;
            
           
            ConnectedToDevice = await Device.ConnectToDeviceAsync();

            if (!ConnectedToDevice)
                return;
            
          
            await Device.SendJsonModelToDevice(new RebootRequest());
            await Task.Delay(2000);

           

            using var reConnectCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(1));
            var isReconnected = await Device.ReconnectWhenAvailable(reConnectCancellationTokenSource.Token);

            if (!isReconnected)
            {
                await DialogService.ShowMessageBox("Error",
                    "Could not reconnect to the device within the allocated time");
                return;
            }
            
            await DialogService.ShowMessageBox("Success",
                "The device successfully connected to the WiFi with the specified settings");
            
        }
        catch (Exception)
        {
            LogService.LogError("There was an internal error testing the WiFi settings on the device");
        }
        finally
        {
          
        }
        
        
    }

    private async Task TestDeviceWiFiSettings()
    {

        try
        {
            var wifiSsid = SetSettingsMessageModel.WiFiSsid;
            var wifiPassword = SetSettingsMessageModel.WiFiPassword;
            
         
            if (Device == null)
                return;
            
         
            ConnectedToDevice = await Device.ConnectToDeviceAsync();

            if (!ConnectedToDevice)
                return;
            
         
            var requestTestWifiModel = new TestWiFiSettingsRequest()
            {
                WiFiPassword = wifiPassword,
                WiFiSsid = wifiSsid
            };
            var responseTestWifiModel =
                await MessageOrchestrator.SendMessage<TestWiFiSettingsRequest, StatusResponse>(
                    requestTestWifiModel, Device);

            if (responseTestWifiModel is not { StatusCode: 200 })
            {
                await DialogService.ShowMessageBox("Error",
                    "Could not communicate with device or get a valid response while setting the WiFi settings to test");
                return;
            }
            
      
            await Device.SendJsonModelToDevice(new RebootRequest());
            await Task.Delay(2000);

        

            using var reConnectCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(1));
            var isReconnected = await Device.ReconnectWhenAvailable(reConnectCancellationTokenSource.Token);

            if (!isReconnected)
            {
                await DialogService.ShowMessageBox("Error",
                    "Could not reconnect to the device within the allocated time");
                return;
            }
            
          
            var responseTestWifiModelResults =
                await MessageOrchestrator.SendMessage<GetTestWiFiSettingsRequest, GetTestWiFiSettingsResponse>(
                    new GetTestWiFiSettingsRequest() , Device);

            if (responseTestWifiModelResults == null)
            {
                await DialogService.ShowMessageBox("Error",
                    "Could not load the test results from the device. Unknown response");
                return;
            }

            if (!responseTestWifiModelResults.Success)
            {
                await DialogService.ShowMessageBox("Error",
                    "The device could not connect to the WiFi with the specified settings");
                return;
            }
            
            await DialogService.ShowMessageBox("Success",
                "The device successfully connected to the WiFi with the specified settings");
         
        }
        catch (Exception)
        {
            LogService.LogError("There was an internal error testing the WiFi settings on the device");
        }
        finally
        {
          
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