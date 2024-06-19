using GXOVnT.Shared.Common;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;

namespace GXOVnT.Services.Models;

public class GXOVnTDevice : NotifyChanged
{

    #region Members
    private string _uuiid = Guid.NewGuid().ToString();
    private bool _systemConfigured;
    private GXOVnTSystemType _systemType = GXOVnTSystemType.UnInitialized;
    #endregion
    
    #region Properties

    public string UUID => $"{_uuiid}";
    
    public IDevice? Device { get; private set; }

    public string Id => Device?.Id.ToString() ?? string.Empty;

    public string DeviceName => Device?.Name ?? string.Empty;

    public int Rssi => Device?.Rssi ?? 0;

    public bool IsConnectable => Device?.IsConnectable ?? false;

    public DeviceState DeviceState => Device?.State ?? DeviceState.Disconnected;

    public bool SystemConfigured
    {
        get => _systemConfigured;
        set => SetField(ref _systemConfigured, value);
    }
    
    public GXOVnTSystemType SystemType
    {
        get => _systemType;
        set => SetField(ref _systemType, value);
    }
    #endregion

    #region ctor

    public GXOVnTDevice(IDevice bleDevice)
    {
        Device = bleDevice;
    }
    
    #endregion

    
}