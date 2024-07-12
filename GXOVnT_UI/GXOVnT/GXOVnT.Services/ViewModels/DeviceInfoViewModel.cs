using GXOVnT.Services.Interfaces;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.DeviceMessage.Response;

namespace GXOVnT.Services.ViewModels;

public class DeviceInfoViewModel : StateObject
{

    #region ctor

    private readonly IDeviceService _deviceService;
    private readonly IBluetoothService _bluetoothService;
    private Guid? _selectedSystemId;
    private Models.System? _selectedSystem;
    private GetSystemSettingsResponse? _deviceInfo;
    #endregion

    #region Properties

    public Guid? SelectedSystemId
    {
        get => _selectedSystemId;
        private set => SetField(ref _selectedSystemId, value);
    }

    public Models.System? SelectedSystem
    {
        get => _selectedSystem;
        private set => SetField(ref _selectedSystem, value);
    }

    public GetSystemSettingsResponse? DeviceInfo
    {
        get => _deviceInfo;
        private set => SetField(ref _deviceInfo, value);
    }

    public bool IsSystemInitialized =>
        _deviceInfo != null && _deviceInfo.SystemType != SystemType.UnInitialized.Id;
    #endregion
    
    #region ctor

    public DeviceInfoViewModel()
    {
        _deviceService = Services.GetRequiredService<IDeviceService>();
        _bluetoothService = Services.GetRequiredService<IBluetoothService>();
    }

    #endregion

    #region Methods
    public void SetSystemId(Guid? deviceId)
    {
        SelectedSystemId = deviceId;

        if (SelectedSystemId == null)
        {
            SelectedSystem = null;
            return;
        }
        SelectedSystem = _bluetoothService.FindDevice(SelectedSystemId.Value);

        if (SelectedSystem != null) 
            return;
        
        LogService.LogError("Could not locate the requested system.");
        _selectedSystemId = null;
    }
    

    public async Task GetDeviceInfo(Guid? systemId)
    {
        try
        {
            SetBusyState(true, "Loading system information");

            SetSystemId(systemId);

            if (SelectedSystem == null)
            {
                DeviceInfo = null;
                return;
            }

            DeviceInfo = await _deviceService.GetDeviceInfoAsync(SelectedSystem);
        }
        catch (Exception ex)
        {
            LogService.LogError($"An error occured while reading the device info. {ex.Message}");
        }
        finally
        {
            SetBusyState(false);
        }
    }

    #endregion
    
}