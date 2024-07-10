using GXOVnT.Services.Interfaces;
using GXOVnT.Services.Models;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.DeviceMessage.Request;
using GXOVnT.Shared.DeviceMessage.Response;

namespace GXOVnT.Services.Services;

public class DeviceService : NotifyChanged, IDeviceService
{

    #region Members

    private readonly ILogService _logService;
    private readonly IMessageOrchestrator _messageOrchestrator;
    
    private bool _isBusy;
    private string _busyText = "";
    
    #endregion

    #region Properties

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetField(ref _isBusy, value);
    }

    public string BusyText
    {
        get => _busyText;
        private set => SetField(ref _busyText, value);
    }
    #endregion

    #region ctor

    public DeviceService(ILogService logService, IMessageOrchestrator messageOrchestrator)
    {
        _logService = logService;
        _messageOrchestrator = messageOrchestrator;
    }

    #endregion

    #region Public Methods

    public async Task RequestRebootAsync(Models.System device, bool reconnect = true)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(device);

            SetBusyStatus(true, "Checking device connection");
            
            // Make sure that we are connected to the device
            var deviceConnected = await device.ConnectToDeviceAsync();
            if (!deviceConnected)
                throw new GXOVnTException("Unable to send the reboot request. The device is not connected");

            SetBusyStatus(true, "Sending device reboot request");

            await device.SendJsonModelToDevice(new RebootRequest());
            await Task.Delay(1000);

            if (!reconnect)
                return;
            
            using var reConnectCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(1));
            var deviceReconnected = await device.ReconnectWhenAvailable(reConnectCancellationTokenSource.Token);

            if (!deviceReconnected)
                throw new GXOVnTException("Could not reconnect to the device after the reboot");

        }
        catch (Exception ex)
        {
            _logService.LogError($"An error occured communicating with the device. {ex.Message}");
            throw;
        }
        finally
        {
            SetBusyStatus(false);
        }
    }

    public async Task<bool> TestWiFiSettingsOnDeviceAsync(Models.System device, string wifiSsid, string wifiPassword)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(device);

            SetBusyStatus(true, "Checking device connection");
            
            // Make sure that we are connected to the device
            var deviceConnected = await device.ConnectToDeviceAsync();
            if (!deviceConnected)
                throw new GXOVnTException("Unable to send the reboot request. The device is not connected");

            SetBusyStatus(true, "Sending test WiFi connection request");
            
            var responseTestWifiModel =
                await _messageOrchestrator.SendMessage<TestWiFiSettingsRequest, StatusResponse>(
                    new TestWiFiSettingsRequest()
                    {
                        WiFiPassword = wifiPassword,
                        WiFiSsid = wifiSsid
                    }, device);
            
            if (responseTestWifiModel == null)
                throw new GXOVnTException("Unable to send the test WiFi request. The device did not respond");
            
            if (responseTestWifiModel.StatusCode != 200)
                throw new GXOVnTException($"The device did not return a success response code ({responseTestWifiModel.StatusCode}). {responseTestWifiModel.StatusMessage}");
                
            // Request the reboot of the device
            await RequestRebootAsync(device, true);
            
            SetBusyStatus(true, "Requesting the WiFi test results from the device ");
            var responseTestWifiModelResults =
                await _messageOrchestrator.SendMessage<GetTestWiFiSettingsRequest, GetTestWiFiSettingsResponse>(
                    new GetTestWiFiSettingsRequest() , device);

            if (responseTestWifiModelResults == null)
                throw new GXOVnTException("Unable to query the test WiFi results. The device did not respond");

            if (!responseTestWifiModelResults.Tested)
                throw new GXOVnTException("The WiFi test did not occur on the device.");
            
            return responseTestWifiModelResults.Success;
        }
        catch (Exception ex)
        {
            _logService.LogError($"An error occured communicating with the device. {ex.Message}");
            throw;
        }
        finally
        {
            SetBusyStatus(false);
        }
    }
    
    public async Task<GetSystemSettingsResponse> GetDeviceInfoAsync(Models.System device)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(device);

            SetBusyStatus(true, "Checking device connection");
            
            // Make sure that we are connected to the device
            var deviceConnected = await device.ConnectToDeviceAsync();
            if (!deviceConnected)
                throw new GXOVnTException("Unable to send the get device info request. The device is not connected");

            SetBusyStatus(true, "Sending device info request");
            var requestModel = new GetSystemSettingsRequest();
            var responseModel = await _messageOrchestrator.SendMessage<GetSystemSettingsRequest, GetSystemSettingsResponse>(
                requestModel, device);
            
            if (responseModel == null)
                throw new GXOVnTException("Unable to get the device info. The device did not respond");
            
            return responseModel;
        }
        catch (Exception ex)
        {
            _logService.LogError($"An error occured communicating with the device. {ex.Message}");
            throw;
        }
        finally
        {
            SetBusyStatus(false);
        }
    }
   
    public async Task<EchoResponse> SendEchoMessageAsync(Models.System device, string echoMessage)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(device);

            SetBusyStatus(true, "Checking device connection");
            
            // Make sure that we are connected to the device
            var deviceConnected = await device.ConnectToDeviceAsync();
            if (!deviceConnected)
                throw new GXOVnTException("Unable to send the echo request. The device is not connected");

            SetBusyStatus(true, "Sending device echo request");
            var requestModel = new EchoRequest()
            {
                EchoMessage = echoMessage
            };
            var responseModel = await _messageOrchestrator.SendMessage<EchoRequest, EchoResponse>(
                requestModel, device);
            
            if (responseModel == null)
                throw new GXOVnTException("Unable to get the echo response. The device did not respond");
            
            return responseModel;
        }
        catch (Exception ex)
        {
            _logService.LogError($"An error occured communicating with the device. {ex.Message}");
            throw;
        }
        finally
        {
            SetBusyStatus(false);
        }
    }
    
    public async Task SetSystemSettingsAsync(Models.System device, SetSystemSettingsRequest request, bool sendSaveSettings = true)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(device);
            ArgumentNullException.ThrowIfNull(request);

            SetBusyStatus(true, "Checking device connection");
            
            // Make sure that we are connected to the device
            var deviceConnected = await device.ConnectToDeviceAsync();
            if (!deviceConnected)
                throw new GXOVnTException("Unable to send the system settings. The device is not connected");

            SetBusyStatus(true, "Sending device settings request");
            
            var responseModel = await _messageOrchestrator.SendMessage<SetSystemSettingsRequest, StatusResponse>(
                request, device);
            
            if (responseModel == null)
                throw new GXOVnTException("Unable to get the set settings response. The device did not respond");
            
            if (responseModel.StatusCode != 200)
                throw new GXOVnTException($"The device did not return a success response code ({responseModel.StatusCode}). {responseModel.StatusMessage}");

            if (!sendSaveSettings)
                return;
            
            await SaveSystemSettingsAsync(device);
        }
        catch (Exception ex)
        {
            _logService.LogError($"An error occured communicating with the device. {ex.Message}");
            throw;
        }
        finally
        {
            SetBusyStatus(false);
        }
    }
    
    public async Task DeleteSystemSettingsAsync(Models.System device, string systemPassword, bool rebootDevice = true)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(device);

            SetBusyStatus(true, "Checking device connection");
            
            // Make sure that we are connected to the device
            var deviceConnected = await device.ConnectToDeviceAsync();
            if (!deviceConnected)
                throw new GXOVnTException("Unable to send the delete system settings request. The device is not connected");

            SetBusyStatus(true, "Sending delete system settings request");
            
            var responseModel = await _messageOrchestrator.SendMessage<DeleteSystemSettingsRequest, StatusResponse>(
                new DeleteSystemSettingsRequest()
                {
                    SystemPassword = systemPassword
                }, device);
            
            if (responseModel == null)
                throw new GXOVnTException("Unable to get the delete system settings response. The device did not respond");
            
            if (responseModel.StatusCode != 200)
                throw new GXOVnTException($"The device did not return a success response code ({responseModel.StatusCode}). {responseModel.StatusMessage}");

            if (!rebootDevice)
                return;

            await RequestRebootAsync(device, true);
        }
        catch (Exception ex)
        {
            _logService.LogError($"An error occured communicating with the device. {ex.Message}");
            throw;
        }
        finally
        {
            SetBusyStatus(false);
        }
    }
    
    public async Task SendSystemBootMode(Models.System device, SystemBootMode systemBootMode, bool rebootDevice = true)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(device);

            SetBusyStatus(true, "Checking device connection");
            
            // Make sure that we are connected to the device
            var deviceConnected = await device.ConnectToDeviceAsync();
            if (!deviceConnected)
                throw new GXOVnTException("Unable to send the system boot mode settings request. The device is not connected");

            SetBusyStatus(true, "Sending system boot mode request");
            
            var responseModel = await _messageOrchestrator.SendMessage<SetSystemBootModeRequest, StatusResponse>(
                new SetSystemBootModeRequest()
                {
                    SystemBootMode = (int)systemBootMode
                }, device);
            
            if (responseModel == null)
                throw new GXOVnTException("Unable to get the system boot mode response. The device did not respond");
            
            if (responseModel.StatusCode != 200)
                throw new GXOVnTException($"The device did not return a success response code ({responseModel.StatusCode}). {responseModel.StatusMessage}");

            if (!rebootDevice)
                return;

            await RequestRebootAsync(device, true);
        }
        catch (Exception ex)
        {
            _logService.LogError($"An error occured communicating with the device. {ex.Message}");
            throw;
        }
        finally
        {
            SetBusyStatus(false);
        }
    }

    public async Task SaveSystemSettingsAsync(Models.System device)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(device);

            SetBusyStatus(true, "Checking device connection");
            
            // Make sure that we are connected to the device
            var deviceConnected = await device.ConnectToDeviceAsync();
            if (!deviceConnected)
                throw new GXOVnTException("Unable to send the system settings. The device is not connected");

            SetBusyStatus(true, "Sending save settings request");
            
            var responseModel = await _messageOrchestrator.SendMessage<SaveConfigurationRequest, StatusResponse>(
                new SaveConfigurationRequest(), device);
            
            if (responseModel == null)
                throw new GXOVnTException("Unable to get the save settings response. The device did not respond");
            
            if (responseModel.StatusCode != 200)
                throw new GXOVnTException($"The device did not return a success response code ({responseModel.StatusCode}). {responseModel.StatusMessage}");
        }
        catch (Exception ex)
        {
            _logService.LogError($"An error occured communicating with the device. {ex.Message}");
            throw;
        }
        finally
        {
            SetBusyStatus(false);
        }
    }
    
    public async Task SendKeepAliveRequestAsync(Models.System device)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(device);

            SetBusyStatus(true, "Checking device connection");
            
            // Make sure that we are connected to the device
            var deviceConnected = await device.ConnectToDeviceAsync();
            if (!deviceConnected)
                throw new GXOVnTException("Unable to send the keep alive request. The device is not connected");

            SetBusyStatus(true, "Sending keep alive request");
            
            var responseModel = await _messageOrchestrator.SendMessage<KeepAliveRequest, StatusResponse>(
                new KeepAliveRequest(), device);
            
            if (responseModel == null)
                throw new GXOVnTException("Unable to get the keep alive response. The device did not respond");
            
            if (responseModel.StatusCode != 200)
                throw new GXOVnTException($"The device did not return a success response code ({responseModel.StatusCode}). {responseModel.StatusMessage}");
        }
        catch (Exception ex)
        {
            _logService.LogError($"An error occured communicating with the device. {ex.Message}");
            throw;
        }
        finally
        {
            SetBusyStatus(false);
        }
    }
    
     public async Task<GetFirmwareUpdateResultResponse> CheckFirmwareUpdates(Models.System device, string wifiSsid = "", string wifiPassword = "")
    {
        try
        {
            ArgumentNullException.ThrowIfNull(device);

            SetBusyStatus(true, "Checking device connection");
            
            // Make sure that we are connected to the device
            var deviceConnected = await device.ConnectToDeviceAsync();
            if (!deviceConnected)
                throw new GXOVnTException("Unable to send the check firmware request. The device is not connected");

            SetBusyStatus(true, "Sending check firmware request");
            
            var responseTestWifiModel =
                await _messageOrchestrator.SendMessage<CheckFirmwareUpdateRequest, StatusResponse>(
                    new CheckFirmwareUpdateRequest()
                    {
                        WiFiPassword = wifiPassword,
                        WiFiSsid = wifiSsid
                    }, device);
            
            if (responseTestWifiModel == null)
                throw new GXOVnTException("Unable to send the check firmware update request. The device did not respond");
            
            if (responseTestWifiModel.StatusCode != 200)
                throw new GXOVnTException($"The device did not return a success response code ({responseTestWifiModel.StatusCode}). {responseTestWifiModel.StatusMessage}");
                
            // Request the reboot of the device
            await RequestRebootAsync(device, true);
            
            SetBusyStatus(true, "Requesting the firmware update results from the device ");
            var responseResults =
                await _messageOrchestrator.SendMessage<GetFirmwareUpdateResultRequest, GetFirmwareUpdateResultResponse>(
                    new GetFirmwareUpdateResultRequest() , device);

            if (responseResults == null)
                throw new GXOVnTException("Unable to query the firmware results. The device did not respond");

            return responseResults;
        }
        catch (Exception ex)
        {
            _logService.LogError($"An error occured communicating with the device. {ex.Message}");
            throw;
        }
        finally
        {
            SetBusyStatus(false);
        }
    }
    #endregion

    #region Private Methods

    private void SetBusyStatus(bool isBusy, string busyText = "")
    {
        IsBusy = isBusy;
        BusyText = isBusy ? busyText : "";
    }

    #endregion

}