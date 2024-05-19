using GXOVnT.Services.Common;
using GXOVnT.Services.Interfaces;
using GXOVnT.Services.ViewModels;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;

namespace GXOVnT.Services;

public class BluetoothService : NotifyChanged, IBluetoothService
{

    #region Members

    private readonly IAlertService _alertService;
    private readonly IRequestPermissionService _requestPermissionService;
    private readonly LogViewModel _logViewModel;
    private IBluetoothLE? _bluetoothManager;
    private IAdapter? _bluetoothAdapter;
    private bool _isInitialized;
    private BluetoothState _bluetoothState;
    #endregion

    #region Properties

    public BluetoothState BluetoothState => _bluetoothState;

    #endregion
    
    #region ctor

    public BluetoothService(IAlertService alertService, 
        IRequestPermissionService requestPermissionService, LogViewModel logViewModel)
    {
        _alertService = alertService;
        _requestPermissionService = requestPermissionService;
        _logViewModel = logViewModel;
    }

    #endregion

    #region Methods

    public async Task<bool> InitializeService()
    {
        if (_isInitialized)
            return true;
        
        var hasPermissions = await _requestPermissionService.CheckBLEPermissionRequirement();

        // Cannot continue if we don't have permissions
        if (!hasPermissions)
            return false;
        
        _bluetoothManager = CrossBluetoothLE.Current;
            

        if (_bluetoothManager == null)
        {
            _logViewModel.LogWarning("Could not get an instance of the bluetooth manager");
            return false;
        }
        
        _bluetoothAdapter = _bluetoothManager.Adapter;

        if (_bluetoothAdapter == null)
        {
            _logViewModel.LogWarning("Could not get an instance of the bluetooth adapter");
            return false;
        }

        AttachBluetoothEvents();

        _isInitialized = true;

        return true;
    }

    public bool IsBluetoothReady()
    {
        if (!_isInitialized) 
            return false;

        var result = (_bluetoothManager?.IsOn ?? false);

        if (!result)
            _alertService.ShowAlertAsync("Bluetooth", "Please switch on the Bluetooth before trying again");

        return result;
    }
    
    
    #endregion

    #region Private Methods

    private void AttachBluetoothEvents()
    {

        if (_bluetoothManager == null || _bluetoothAdapter == null)
            return;
        
        _bluetoothManager.StateChanged -= BluetoothManagerOnStateChanged;
        _bluetoothManager.StateChanged += BluetoothManagerOnStateChanged;

        _bluetoothAdapter.ScanTimeoutElapsed -= BluetoothAdapterOnScanTimeoutElapsed;
        _bluetoothAdapter.ScanTimeoutElapsed += BluetoothAdapterOnScanTimeoutElapsed;
        _bluetoothAdapter.DeviceAdvertised -= BluetoothAdapterOnDeviceAdvertised;
        _bluetoothAdapter.DeviceAdvertised += BluetoothAdapterOnDeviceAdvertised;
        _bluetoothAdapter.DeviceDiscovered -= BluetoothAdapterOnDeviceDiscovered;
        _bluetoothAdapter.DeviceDiscovered += BluetoothAdapterOnDeviceDiscovered;
        
        // Set up scanner
        _bluetoothAdapter.ScanMode = ScanMode.LowLatency;
        _bluetoothAdapter.ScanTimeout = 30000; // ms
    }

    #endregion

    #region Event Callbacks

    private void BluetoothAdapterOnDeviceDiscovered(object? sender, DeviceEventArgs e)
    {
        
    }

    private void BluetoothAdapterOnDeviceAdvertised(object? sender, DeviceEventArgs e)
    {
        
    }

    private void BluetoothAdapterOnScanTimeoutElapsed(object? sender, EventArgs e)
    {
        
    }

    private void BluetoothManagerOnStateChanged(object? sender, BluetoothStateChangedArgs e)
    {
        _logViewModel.LogInformation($"Current Bluetooth state changed to {e.NewState.ToString()}");
        _bluetoothState = e.NewState;
    }

    #endregion
    
}