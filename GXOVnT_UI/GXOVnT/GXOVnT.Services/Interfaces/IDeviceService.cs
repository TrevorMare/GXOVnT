using System.ComponentModel;
using GXOVnT.Services.Models;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.DeviceMessage.Request;
using GXOVnT.Shared.DeviceMessage.Response;

namespace GXOVnT.Services.Interfaces;

public interface IDeviceService : INotifyPropertyChanged
{
    
    bool IsBusy { get; }
    
    string BusyText { get; }
    
    Task RequestRebootAsync(Models.System device, bool reconnect = true);

    Task<bool> TestWiFiSettingsOnDeviceAsync(Models.System device, string wifiSsid, string wifiPassword);

    Task<GetSystemSettingsResponse> GetDeviceInfoAsync(Models.System device);

    Task<EchoResponse> SendEchoMessageAsync(Models.System device, string echoMessage);

    Task SetSystemSettingsAsync(Models.System device, SetSystemSettingsRequest request, bool sendSaveSettings = true);

    Task SaveSystemSettingsAsync(Models.System device);

    Task SendKeepAliveRequestAsync(Models.System device);

    Task DeleteSystemSettingsAsync(Models.System device, string systemPassword, bool rebootDevice = true);

    Task SendSystemBootMode(Models.System device, SystemBootMode systemBootMode, bool rebootDevice = true);

    Task<GetFirmwareUpdateResultResponse> CheckFirmwareUpdates(Models.System device, string wifiSsid = "",
        string wifiPassword = "");
}