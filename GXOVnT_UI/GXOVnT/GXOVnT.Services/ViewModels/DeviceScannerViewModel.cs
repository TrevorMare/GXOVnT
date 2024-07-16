using System.ComponentModel;
using GXOVnT.Services.Interfaces;
using GXOVnT.Shared.Common;

namespace GXOVnT.Services.ViewModels;

public class DeviceScannerViewModel : StateObject
{

    #region Members

    private readonly IBluetoothService _bluetoothService;
    private Guid? _selectedSystemId;
    private Models.System? _selectedSystem;
    
    #endregion

    #region ctor

    public DeviceScannerViewModel()
    {
        _bluetoothService = Services.GetRequiredService<IBluetoothService>();
        _bluetoothService.PropertyChanged += BluetoothServiceOnPropertyChanged;
    }
    #endregion

    #region Properties

    public bool IsScanningDevices => _bluetoothService.IsScanningDevices;
    
    public IReadOnlyList<Models.System> DiscoveredDevices => _bluetoothService.DiscoveredDevices;
    
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
    #endregion

    #region Public Methods

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

    public async Task StopScanDevicesAsync()
    {
        try
        {
            await _bluetoothService.StopScanForDevicesAsync();
        }
        catch (Exception ex)
        {
            LogService.LogError($"An error occured while stopping the scan of devices. {ex.Message}");
        }
        finally
        {
            SetBusyState(false);
        }
    }
    
    public async Task StartScanDevicesAsync()
    {
        try
        {
            SetBusyState(true, "Scanning Bluetooth devices");
            
            // Now perform the scan
            await _bluetoothService.StartScanForDevicesAsync();
        }
        catch (Exception ex)
        {
            LogService.LogError($"An error occured while scanning the devices. {ex.Message}");
        }
        finally
        {
            SetBusyState(false);
        }
    }
    #endregion

    #region Event Callbacks

    private void BluetoothServiceOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    { 
        OnStateChanged();
    }

    #endregion
}