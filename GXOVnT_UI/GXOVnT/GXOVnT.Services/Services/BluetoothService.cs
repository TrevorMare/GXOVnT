using System.Collections.ObjectModel;
using GXOVnT.Services.Interfaces;
using GXOVnT.Services.Models;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.Interfaces;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Extensions;

namespace GXOVnT.Services.Services;

/// <summary>
/// Bluetooth service wrapper class. This class is responsible for performing the scans of devices
/// </summary>
public class BluetoothService : StateObject, IBluetoothService 
{


    #region Members
    private const string GXOVnTManufacturerValue = "GXOVnT";

    private readonly IRequestPermissionService _requestPermissionService;
    private IBluetoothLE? _bluetoothManager;
    private IAdapter? _bluetoothAdapter;
    private bool _isScanningDevices;
    private CancellationTokenSource _scanCancellationTokenSource = default!;
    private readonly ObservableCollection<Models.System> _discoveredDevices = new();
    #endregion
    
    #region Events
    public delegate void OnDeviceFoundHandler(object sender, Models.System args);
    public event OnDeviceFoundHandler? OnDeviceFound;
    #endregion
    
    #region Members
    private bool _applicationHasBluetoothPermissions;
    private bool _bluetoothIsReady;
    private BluetoothState _bluetoothState = BluetoothState.Unknown;
    #endregion

    #region Properties
    /// <summary>
    /// Gets a value indicating if the application has Bluetooth permissions 
    /// </summary>
    public bool ApplicationHasBluetoothPermissions
    {
        get => _applicationHasBluetoothPermissions;
        private set => SetField(ref _applicationHasBluetoothPermissions, value);
    }
    
    /// <summary>
    /// Gets a value indicating that the Bluetooth adapter is on and working
    /// </summary>
    public bool BluetoothIsReady
    {
        get => _bluetoothIsReady;
        private set => SetField(ref _bluetoothIsReady, value);
    }
   
    /// <summary>
    /// Gets the Bluetooth state
    /// </summary>
    public BluetoothState BluetoothState
    {
        get => _bluetoothState;
        private set => SetField(ref _bluetoothState, value);
    }

    /// <summary>
    /// Gets a value indicating if the service is actively scanning for devices
    /// </summary>
    public bool IsScanningDevices
    {
        get => _isScanningDevices;
        private set => SetField(ref _isScanningDevices, value);
    }
   
    /// <summary>
    /// Gets a list of scanned devices
    /// </summary>
    public IReadOnlyList<Models.System> DiscoveredDevices => _discoveredDevices.AsReadOnly();
    #endregion
    
    #region ctor

    public BluetoothService(IRequestPermissionService requestPermissionService)
    {
        
        _requestPermissionService = requestPermissionService;
        _discoveredDevices.CollectionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(DiscoveredDevices));
        };
    }

    #endregion

    #region Methods
    public async Task<bool> StartScanForDevicesAsync(CancellationTokenSource? cancellationTokenSource = default)
    {
        try
        {
            SetBusyState(true, "Scanning devices");
            IsScanningDevices = true;

            if (!await InitializeBluetooth())
                return false;
            
            _discoveredDevices.Clear();
            _scanCancellationTokenSource = cancellationTokenSource ?? new CancellationTokenSource();
            
            await _bluetoothAdapter.StartScanningForDevicesAsync(_scanCancellationTokenSource.Token);

            IsScanningDevices = false;

            return true;

        }
        catch (Exception ex)
        {
            LogService.LogError($"StartScanForDevicesAsync: An error occured scanning for devices. {ex.Message}");
            return false;
        }
        finally
        {
            SetBusyState(false);
            IsScanningDevices = false;    
        }
    }

    public async Task StopScanForDevicesAsync()
    {
        await _scanCancellationTokenSource.CancelAsync();
    }

    public Models.System? FindDevice(Guid deviceId)
    {
        return _discoveredDevices.ToList().Find(d => d.Id == deviceId);
    }

    #endregion

    #region Private Methods
    
    
    private async Task<bool> InitializeBluetooth()
    {
        try
        {
            // First we need to detach the existing event handlers
            DetachBluetoothEvents();
            
            // Next, check if we still have the correct permissions
            BluetoothIsReady = false;
            BluetoothState = BluetoothState.Unknown;
            ApplicationHasBluetoothPermissions = await _requestPermissionService.RequestBluetoothPermission();

            if (!ApplicationHasBluetoothPermissions)
                return false;
            
            // Get the references to the current manager and adapter
            _bluetoothManager = CrossBluetoothLE.Current;
            _bluetoothAdapter = _bluetoothManager?.Adapter;
            
            if (_bluetoothManager == null || _bluetoothAdapter == null)
            {
                LogService.LogWarning("InitializeBluetooth: Could not get a reference to the Bluetooth adapter");
                return false;
            }
            
            // Set up scanner
            _bluetoothAdapter.ScanMode = ScanMode.Balanced;
            _bluetoothAdapter.ScanTimeout = 30000; // ms
            
            // Re-Attach the events if we have the adapter and manager objects
            AttachBluetoothEvents();
            
            BluetoothState = _bluetoothManager.State;
            BluetoothIsReady = (BluetoothState == BluetoothState.On);

       
        }
        catch (Exception ex)
        {
            LogService.LogError($"InitializeBluetooth: An error occured initializing the Bluetooth adapter. {ex.Message}");
            BluetoothIsReady = false;
            return false;
        }

        return BluetoothIsReady;
    }
    
    private void DetachBluetoothEvents()
    {
        if (_bluetoothManager != null)
            _bluetoothManager.StateChanged -= BluetoothManagerOnStateChanged;

        if (_bluetoothAdapter == null) 
            return;
        
        _bluetoothAdapter.DeviceAdvertised -= BluetoothAdapterOnDeviceAdvertised;
        _bluetoothAdapter.DeviceDiscovered -= BluetoothAdapterOnDeviceDiscovered;
        _bluetoothAdapter.DeviceConnected -= BluetoothAdapterOnDeviceConnected;
        _bluetoothAdapter.DeviceDisconnected -= BluetoothAdapterOnDeviceDisconnected;
    }

    private void AttachBluetoothEvents()
    {

        if (_bluetoothManager == null || _bluetoothAdapter == null)
            return;
        
        _bluetoothManager.StateChanged += BluetoothManagerOnStateChanged;
        _bluetoothAdapter.DeviceAdvertised += BluetoothAdapterOnDeviceAdvertised;
        _bluetoothAdapter.DeviceDiscovered += BluetoothAdapterOnDeviceDiscovered;
        _bluetoothAdapter.DeviceConnected += BluetoothAdapterOnDeviceConnected;
        _bluetoothAdapter.DeviceDisconnected += BluetoothAdapterOnDeviceDisconnected;
        
    }

    private async Task CheckDeviceMatchAndAddToDiscovered(DeviceEventArgs e)
    {
        var allAdvertisementRecords = e.Device.AdvertisementRecords ?? new List<AdvertisementRecord>();
        if (!allAdvertisementRecords.Any())
            return;

        var manufacturerData = allAdvertisementRecords.ToList()
            .Find(r => r.Type == AdvertisementRecordType.ManufacturerSpecificData);

        if (manufacturerData == null)
            return;

        var manufacturerDataValue = System.Text.Encoding.UTF8.GetString(manufacturerData.Data);

        if (!manufacturerDataValue.StartsWith(GXOVnTManufacturerValue, StringComparison.CurrentCultureIgnoreCase))
            return;

        var deviceId = e.Device.Id;
        if (FindDevice(deviceId) != null)
            return;

        var bleDevice = new Models.System(_bluetoothAdapter!, e.Device, LogService);
        
        _discoveredDevices.Add(bleDevice);
        
        OnDeviceFound?.Invoke(this, bleDevice);
        
        await e.Device.UpdateRssiAsync();
        
    }
    
    #endregion

    #region Event Callbacks
  
    
    private async void BluetoothAdapterOnDeviceDiscovered(object? sender, DeviceEventArgs e)
    {
        await CheckDeviceMatchAndAddToDiscovered(e);
    }

    private async void BluetoothAdapterOnDeviceAdvertised(object? sender, DeviceEventArgs e)
    {
        await CheckDeviceMatchAndAddToDiscovered(e);
    }

    private void BluetoothAdapterOnDeviceDisconnected(object? sender, DeviceEventArgs e)
    {
        var bleDevice = FindDevice(e.Device.Id);
        bleDevice?.ConnectionStateChanged();
    }

    private void BluetoothAdapterOnDeviceConnected(object? sender, DeviceEventArgs e)
    {
        var bleDevice = FindDevice(e.Device.Id);
        bleDevice?.ConnectionStateChanged();
    }

    private void BluetoothManagerOnStateChanged(object? sender, BluetoothStateChangedArgs e)
    {
        LogService.LogInformation($"BluetoothManagerOnStateChanged: Current Bluetooth state changed to {e.NewState.ToString()}");
        BluetoothState = e.NewState;
    }

    #endregion

    #region Dispose

    public ValueTask DisposeAsync()
    {
        if (_bluetoothAdapter == null)
            return ValueTask.CompletedTask;
        _discoveredDevices.Clear();
        return ValueTask.CompletedTask;
    }

    #endregion
  
}