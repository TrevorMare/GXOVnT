using GXOVnT.Services.Interfaces;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.DeviceMessage.Request;
using GXOVnT.Shared.DeviceMessage.Response;
using GXOVnT.Shared.Interfaces;

namespace GXOVnT.Services.ViewModels;

public class DeviceEnrollConfigurationViewModel : StateObject
{
    
    #region Members
    private readonly IDeviceService _deviceService;
    private readonly IBluetoothService _bluetoothService;
    private Guid? _selectedSystemId;
    private Models.System? _selectedSystem;
    private GetSystemSettingsResponse? _systemConfiguration;
    private string _systemName = string.Empty;
    private string _systemId = string.Empty;
    private bool _systemConfigured;
    private int _systemType;
    private SystemType? _gxovntSystemType;
    private string _firmwareVersion = string.Empty;
    private string _wifiSsid = string.Empty;
    private string _wifiPassword = string.Empty;
    private bool _isValid;
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

    public GetSystemSettingsResponse? SystemConfiguration
    {
        get => _systemConfiguration;
        private set => SetField(ref _systemConfiguration, value);
    }

    public bool IsValid
    {
        get => _isValid;
        private set => SetField(ref _isValid, value);
    }
    
    public string SystemName
    {
        get => _systemName;
        set
        {
            if (SetField(ref _systemName, value))
                ValidateData();
        }
    }

    public string SystemId 
    {
        get => _systemId;
        set
        {
            if (SetField(ref _systemId, value))
                ValidateData();
        }
    }

    public bool SystemConfigured
    {
        get => _systemConfigured;
        set => SetField(ref _systemConfigured, value);
    }

    public int SystemType
    {
        get => _systemType;
        set
        {
            if (SetField(ref _systemType, value))
                ValidateData();
        }
    }

    public SystemType? GXOVnTSystemType => Enumeration.FromValue<SystemType>(SystemType);

    public string FirmwareVersion
    {
        get => _firmwareVersion;
        private set => SetField(ref _firmwareVersion, value);
    }
    
    public string WiFiSsid
    {
        get => _wifiSsid;
        set
        {
            if (SetField(ref _wifiSsid, value))
                ValidateData();
        }
    }

    public string WiFiPassword
    {
        get => _wifiPassword;
        set
        {
            if (SetField(ref _wifiPassword, value))
                ValidateData();
        }
    }

    #endregion
    
    #region ctor

    public DeviceEnrollConfigurationViewModel()
    {
        _deviceService = Services.GetRequiredService<IDeviceService>();
        _bluetoothService = Services.GetRequiredService<IBluetoothService>();
        _deviceService.OnBusyStateChanged += DeviceServiceOnOnBusyStateChanged;
    }
    #endregion
    
    #region Public Methods
    public async Task LoadDeviceConfiguration(Guid? systemId)
    {
        await RunTaskAsync(async () =>
        {
            
            SetSystemId(systemId);

            if (SelectedSystem == null)
            {
                SystemConfiguration = null;
                SetLocalVariablesFromSystemConfiguration();
                return;
            }
            
            SystemConfiguration = await _deviceService.GetDeviceInfoAsync(SelectedSystem);
            SetLocalVariablesFromSystemConfiguration();
        }, "Loading the system configuration");
    }
    
    public void SetSystemId(Guid? deviceId)
    {
        // Check if the requested system is already selected
        if (IsDeviceIdAlreadySelected(deviceId))
            return;
        
        SelectedSystemId = deviceId;

        if (SelectedSystemId == null)
        {
            SystemConfiguration = null;
            SelectedSystem = null;
            return;
        }
        
        SelectedSystem = _bluetoothService.FindDevice(SelectedSystemId.Value);
        
        if (SelectedSystem != null) 
            return;
        
        LogService.LogError("Could not locate the requested system.");
        _selectedSystemId = null;
    }

    public void ValidateData()
    {
        var isValid = !string.IsNullOrEmpty(SystemName);
        isValid &= !string.IsNullOrEmpty(SystemId);
        isValid &= GXOVnTSystemType != null;
        isValid &= !string.IsNullOrEmpty(WiFiSsid);
        isValid &= !string.IsNullOrEmpty(WiFiPassword);

        IsValid = isValid;
    }

    public async Task<bool> SaveSettings()
    {
        return await RunTaskAsync(async () =>
        {
            if (SelectedSystem == null)
                return false;

            ValidateData();

            if (!_isValid)
                return false;

            await _deviceService.SetSystemSettingsAsync(SelectedSystem, new SetSystemSettingsRequest()
            {
                WiFiPassword = WiFiPassword,
                WiFiSsid = WiFiSsid,
                SystemConfigured = true,
                SystemName = SystemName,
                SystemType = SystemType
            });

            return true;

        }, "Saving the device configuration");
    }
    
    public async Task<bool> TestDeviceWiFiSettings()
    {
        return await RunTaskAsync(async () =>
        {
            var wifiSsid = WiFiSsid;
            var wifiPassword = WiFiPassword;
         
            if (SelectedSystem == null)
                return false;

            var testResponse = await _deviceService.TestWiFiSettingsOnDeviceAsync(SelectedSystem, wifiSsid, wifiPassword);
            return testResponse;

        }, "Testing WiFi details on system");
    }
    #endregion

    #region Private Methods

    private bool IsDeviceIdAlreadySelected(Guid? deviceId)
    {
        if (deviceId == null || _selectedSystemId == null)
            return false;

        return (deviceId.Value.Equals(_selectedSystemId.Value));
    }

    private void SetLocalVariablesFromSystemConfiguration()
    {
        if (SystemConfiguration == null)
        {
            SystemName = string.Empty;
            SystemId = string.Empty;
            SystemConfigured = false;
            SystemType = GXOVnT.Shared.Common.SystemType.UnInitialized.Id;
            FirmwareVersion = string.Empty;
            WiFiSsid = string.Empty;
            WiFiPassword = string.Empty;

            return;
        }
        
        SystemName = SystemConfiguration.SystemName;
        SystemId = SystemConfiguration.SystemId;
        SystemConfigured = SystemConfiguration.SystemConfigured;
        SystemType = SystemConfiguration.SystemType;
        FirmwareVersion = SystemConfiguration.FirmwareVersion;
        WiFiSsid = SystemConfiguration.WiFiSsid;
        WiFiPassword = SystemConfiguration.WiFiPassword;
    }
    #endregion

    #region Event Callbacks

    private void DeviceServiceOnOnBusyStateChanged(object sender, BusyStateChangedArgs args)
    {
        SetBusyState(args.IsBusy, args.BusyStatus);
    }
    #endregion

}