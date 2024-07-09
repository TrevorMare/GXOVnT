using System.ComponentModel;
using GXOVnT.Services.Models;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.DeviceMessage.Request;
using GXOVnT.Shared.DeviceMessage.Response;

namespace GXOVnT.Services.Interfaces;

public interface IGXOVnTBleDeviceService : INotifyPropertyChanged
{
    
    bool IsBusy { get; }
    
    string BusyText { get; }
    
    Task RequestRebootAsync(GXOVnTBleDevice device, bool reconnect = true);

    Task<bool> TestWiFiSettingsOnDeviceAsync(GXOVnTBleDevice device, string wifiSsid, string wifiPassword);

    Task<GetSystemSettingsResponse> GetDeviceInfoAsync(GXOVnTBleDevice device);

    Task<EchoResponse> SendEchoMessageAsync(GXOVnTBleDevice device, string echoMessage);

    Task SetSystemSettingsAsync(GXOVnTBleDevice device, SetSystemSettingsRequest request, bool sendSaveSettings = true);

    Task SaveSystemSettingsAsync(GXOVnTBleDevice device);

    Task SendKeepAliveRequestAsync(GXOVnTBleDevice device);

    Task DeleteSystemSettingsAsync(GXOVnTBleDevice device, string systemPassword, bool rebootDevice = true);

    Task SendSystemBootMode(GXOVnTBleDevice device, SystemBootMode systemBootMode, bool rebootDevice = true);

    Task<GetFirmwareUpdateResultResponse> CheckFirmwareUpdates(GXOVnTBleDevice device, string wifiSsid = "",
        string wifiPassword = "");
}