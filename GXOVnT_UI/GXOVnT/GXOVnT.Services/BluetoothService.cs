using System.Collections.ObjectModel;
using GXOVnT.Services.Common;
using GXOVnT.Services.Interfaces;
using GXOVnT.Services.Models;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.Helpers;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Extensions;

namespace GXOVnT.Services;

public class BluetoothService : NotifyChanged, IBluetoothService 
{

    #region Helpers

    public record GXOVnTDeviceFoundArgs(
        DeviceEventArgs DeviceArgs,
        bool SystemConfigured,
        GXOVnTSystemType SystemType);

    #endregion

    #region Members

    private const string GXOVnTManufacturerValue = "GXOVnT";
    
    /// <summary>
    /// Unique id that the GXOVnT devices broadcast
    /// </summary>
    private static readonly Guid GXOVnTBluetoothServiceId = new ("05c1fba8-cc8b-4534-8787-0e6a0775c3de");
    
    /// <summary>
    /// This id is relative to the GXOVnT device and not the incoming for this service
    /// </summary>
    private static readonly Guid GXOVnTBluetoothIncomingCharacteristic = new ("4687b690-cd36-4a7c-9134-49ffe62d9e4f");
    
    /// <summary>
    /// This id is relative to the GXOVnT device and not the outgoing for this service
    /// </summary>
    private static readonly Guid GXOVnTBluetoothOutgoingCharacteristic = new ("4687b690-cd36-4a7c-9134-49ffe62d954f");
    
    /// <summary>
    /// Unique id for outgoing messages
    /// </summary>
    private short _outgoingMessageId = 1;
    #endregion
    
    #region Events

    public delegate void OnDeviceFoundHandler(object sender, GXOVnTDeviceFoundArgs e);
    
    public delegate void OnMessagePacketReceivedHandler(object sender, byte[]? buffer);

    public event OnDeviceFoundHandler? OnDeviceFound;
    
    public event OnMessagePacketReceivedHandler? OnMessagePacketReceived; 

    #endregion
    
    #region Members

    private readonly IAlertService _alertService;
    private readonly IRequestPermissionService _requestPermissionService;
    private readonly ILogService _logService;
    private readonly ObservableCollection<GXOVnTDevice> _scannedDevices = new();
    
    private bool _applicationHasBluetoothPermissions;
    private bool _bluetoothIsReady;
    private BluetoothState _bluetoothState = BluetoothState.Unknown;
    private string _bluetoothStateText = "";
    private bool _isScanningDevices;
    private CancellationTokenSource _scanCancellationTokenSource = default!;
    private bool _isConnectedToDevice;

    private IBluetoothLE? _bluetoothManager;
    private IAdapter? _bluetoothAdapter;
    private IDevice? _connectedDevice;
    private IService? _connectedDeviceService;
    private ICharacteristic? _incomingMessageCharacteristic;
    private ICharacteristic? _outgoingMessageCharacteristic;

   
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
    /// Gets the Bluetooth state text
    /// </summary>
    public string BluetoothStateText
    {
        get => _bluetoothStateText;
        private set => SetField(ref _bluetoothStateText, value);
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
    /// Gets a value indicating if this service is connected to a device
    /// </summary>
    public bool IsConnectedToDevice
    {
        get => _isConnectedToDevice;
        private set => SetField(ref _isConnectedToDevice, value);
    }

    /// <summary>
    /// Gets the current connected device
    /// </summary>
    public IDevice? ConnectedDevice => _connectedDevice;

    /// <summary>
    /// Gets a list of scanned devices
    /// </summary>
    public IReadOnlyList<GXOVnTDevice> ScannedDevices => _scannedDevices.AsReadOnly();
    #endregion
    
    #region ctor

    public BluetoothService(IAlertService alertService,
        IRequestPermissionService requestPermissionService, 
        ILogService logService)
    {
        _alertService = alertService;
        _requestPermissionService = requestPermissionService;
        _logService = logService;

        _scannedDevices.CollectionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(ScannedDevices));
        };
    }

    #endregion

    #region Methods
    public async Task<bool> StartScanForDevicesAsync(CancellationTokenSource? cancellationTokenSource = default)
    {
        try
        {
            IsScanningDevices = true;

            if (!await InitializeBluetooth())
                return false;
            
            _scannedDevices.Clear();
            _scanCancellationTokenSource = cancellationTokenSource ?? new CancellationTokenSource();
            
            await _bluetoothAdapter.StartScanningForDevicesAsync(_scanCancellationTokenSource.Token);

            IsScanningDevices = false;

            return true;

        }
        catch (Exception ex)
        {
            _logService.LogError($"StartScanForDevicesAsync: An error occured scanning for devices. {ex.Message}");
            return false;
        }
        finally
        {
            IsScanningDevices = false;    
        }
    }

    public async Task StopScanForDevicesAsync()
    {
        await _scanCancellationTokenSource.CancelAsync();
    }

    public async Task<bool> DisConnectFromDevice()
    {
        try
        {
            if (_bluetoothAdapter == null)
                return false;
            
            if (_connectedDevice != null)
            {
                if (_incomingMessageCharacteristic != null)
                    _incomingMessageCharacteristic.ValueUpdated -= IncomingMessageCharacteristicOnValueUpdated;

                _outgoingMessageCharacteristic = null;
                _incomingMessageCharacteristic = null;
                
                _connectedDeviceService = null;

                await _bluetoothAdapter.DisconnectDeviceAsync(_connectedDevice);
                _connectedDevice = null;
            }
            
            IsConnectedToDevice = false;
            return true;
        }
        catch (Exception ex)
        {
            _logService.LogError($"DisConnectFromDevice: An error occured disconnecting from the device. {ex.Message}");
            return false;
        }
    }
    
    public async Task<bool> ConnectToDevice(Guid deviceId, bool keepConnectionAlive)
    {
        try
        {
            // If we are already connected to a device
            if (IsConnectedToDevice && _connectedDevice != null)
            {
                // Same id, no need to re-connect
                if (_connectedDevice.Id == deviceId)
                    return true;
                // Disconnect from the current device
                await DisConnectFromDevice();
            }
            
            IsConnectedToDevice = false;

            if (_bluetoothAdapter == null)
                return false;

            _connectedDevice = _bluetoothAdapter.DiscoveredDevices.FirstOrDefault(d => d.Id == deviceId) ??
                         throw new ArgumentException(
                             "Could not locate the requested device for the specified device Id", nameof(deviceId));

            if (!_connectedDevice.IsConnectable)
                throw new ArgumentException(
                    "The device is not connectable for the specified device Id", nameof(deviceId));

            await _bluetoothAdapter.ConnectToDeviceAsync(_connectedDevice, new ConnectParameters(true));
            
            _connectedDeviceService = await _connectedDevice.GetServiceAsync(GXOVnTBluetoothServiceId);
            if (_connectedDeviceService == null)
                throw new ArgumentException(
                    "The device did not broadcast the expected service", nameof(deviceId));
            
            // Map the incoming message characteristic to the outgoing of the device
            _incomingMessageCharacteristic = await _connectedDeviceService.GetCharacteristicAsync(GXOVnTBluetoothOutgoingCharacteristic);
            if (_incomingMessageCharacteristic == null)
                throw new ArgumentException(
                    "The device service did not broadcast the expected characteristic", nameof(deviceId));
            _incomingMessageCharacteristic.ValueUpdated += IncomingMessageCharacteristicOnValueUpdated;
            
            // Map the outgoing message characteristic to the incoming of the device
            _outgoingMessageCharacteristic = await _connectedDeviceService.GetCharacteristicAsync(GXOVnTBluetoothIncomingCharacteristic);
            if (_incomingMessageCharacteristic == null)
                throw new ArgumentException(
                    "The device service did not broadcast the expected characteristic", nameof(deviceId));
            
            await _incomingMessageCharacteristic.StartUpdatesAsync();
            
            IsConnectedToDevice = true;
            return true;
        }
        catch (Exception ex)
        {
            await DisConnectFromDevice();
            _logService.LogError($"StartScanForDevicesAsync: An error occured scanning for devices. {ex.Message}");
            return false;
        }
        
    }

    public Task<bool> ReConnectToDeviceWhenAvailable(string id, CancellationToken token)
    {
        return Task.FromResult(true);
    }

    public async Task<short> SendJsonModelToDevice<T>(T jsonModel, Action<int>? progressChangedCallback = default) where T : Shared.JsonModels.BaseModel
    {

        try
        {
            if (!IsConnectedToDevice || _outgoingMessageCharacteristic == null)
                return -1;

            var commMessage = jsonModel.ToCommMessage(_outgoingMessageId);

            var currentIndex = 0;
            var numberOfPackets = commMessage.MessagePackets.Count;
            
            while (commMessage.MoveNext())
            {
                await _outgoingMessageCharacteristic.WriteAsync(commMessage.Current.SerializePacket());

                currentIndex++;
                
                // Raise the progress changed
                if (progressChangedCallback != null)
                {
                    var calculatedProgress = ((decimal)currentIndex / (decimal)numberOfPackets) * 100;
                    var intProgress = (int)Math.Ceiling(calculatedProgress);
                    progressChangedCallback.Invoke(intProgress);
                }
                
                await Task.Delay(200);
            }

            return _outgoingMessageId;
        }
        catch (Exception ex)
        {
            _logService.LogError($"An error occured writing the value to the device: {ex.Message}");
            return -1;
        }
        finally
        {
            _outgoingMessageId++;
            if (_outgoingMessageId >= short.MaxValue)
                _outgoingMessageId = 1;
        }
    }

    #endregion

    #region Private Methods
    private async void IncomingMessageCharacteristicOnValueUpdated(object? sender, CharacteristicUpdatedEventArgs e)
    {
        try
        {
            if (_incomingMessageCharacteristic == null)
                throw new Exception("The incoming message characteristic has been reset.");
            
            var (data, _) = await _incomingMessageCharacteristic!.ReadAsync();

            if (data != null && data.Length != 0)
                OnMessagePacketReceived?.Invoke(this, data);

        }
        catch (Exception ex)
        {
            _logService.LogError($"There was an error reading the incoming message characteristic. {ex.Message}");
        }
    }
    
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
                _logService.LogWarning("InitializeBluetooth: Could not get a reference to the Bluetooth adapter");
                return false;
            }
            
            // Set up scanner
            _bluetoothAdapter.ScanMode = ScanMode.Balanced;
            _bluetoothAdapter.ScanTimeout = 30000; // ms
            
            // Re-Attach the events if we have the adapter and manager objects
            AttachBluetoothEvents();
            
            BluetoothState = _bluetoothManager.State;
            BluetoothIsReady = (BluetoothState == BluetoothState.On);

            if (!BluetoothIsReady)
                await _alertService.ShowAlertAsync("Bluetooth", "Please check that the bluetooth is on before trying again");
        }
        catch (Exception ex)
        {
            _logService.LogError($"InitializeBluetooth: An error occured initializing the Bluetooth adapter. {ex.Message}");
            BluetoothIsReady = false;
            return false;
        }

        return BluetoothIsReady;
    }
    
    private void SetBluetoothStateText()
    {
        var result = "Unknown BLE state.";
        switch (BluetoothState)
        {
            case BluetoothState.Unknown:
                result = "Unknown BLE state.";
                break;
            case BluetoothState.Unavailable:
                result = "BLE is not available on this device.";
                break;
            case BluetoothState.Unauthorized:
                result = "You are not allowed to use BLE.";
                break;
            case BluetoothState.TurningOn:
                result = "BLE is warming up, please wait.";
                break;
            case BluetoothState.On:
                result = "BLE is on.";
                break;
            case BluetoothState.TurningOff:
                result = "BLE is turning off. That's sad!";
                break;
            case BluetoothState.Off:
                result = "BLE is off. Turn it on!";
                break;
        }
        BluetoothStateText = result;
    }

    private void DetachBluetoothEvents()
    {
        if (_bluetoothManager != null)
            _bluetoothManager.StateChanged -= BluetoothManagerOnStateChanged;

        if (_bluetoothAdapter == null) 
            return;
        
        _bluetoothAdapter.ScanTimeoutElapsed -= BluetoothAdapterOnScanTimeoutElapsed;
        _bluetoothAdapter.DeviceAdvertised -= BluetoothAdapterOnDeviceAdvertised;
        _bluetoothAdapter.DeviceDiscovered -= BluetoothAdapterOnDeviceDiscovered;
    }
    
    private void AttachBluetoothEvents()
    {

        if (_bluetoothManager == null || _bluetoothAdapter == null)
            return;
        
        _bluetoothManager.StateChanged += BluetoothManagerOnStateChanged;
        _bluetoothAdapter.ScanTimeoutElapsed += BluetoothAdapterOnScanTimeoutElapsed;
        _bluetoothAdapter.DeviceAdvertised += BluetoothAdapterOnDeviceAdvertised;
        _bluetoothAdapter.DeviceDiscovered += BluetoothAdapterOnDeviceDiscovered;
    }

    private async Task NotifyDeviceFoundIfMatch(DeviceEventArgs e)
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
        
        var scannedDevice = _scannedDevices.ToList().Find(d => d.Id.Equals(e.Device.Id.ToString()));

        if (scannedDevice == null)
        {
            
            var manufacturerParts = manufacturerDataValue.Split("|");
            var systemConfigured = false;
            var systemType = GXOVnTSystemType.UnInitialized;

            if (manufacturerParts.Length >= 2)
                systemType = Enumeration.FromValue<GXOVnTSystemType>(int.Parse(manufacturerParts[1])) ?? GXOVnTSystemType.UnInitialized;
            if (manufacturerParts.Length >= 3)
                systemConfigured = manufacturerParts[2] == "1";
            
            scannedDevice = new GXOVnTDevice(e.Device)
            {
                SystemConfigured = systemConfigured,
                SystemType = systemType
            };
            _scannedDevices.Add(scannedDevice);

            OnDeviceFound?.Invoke(this, new GXOVnTDeviceFoundArgs(e, systemConfigured, systemType));
            
            return;
        }
        
        await e.Device.UpdateRssiAsync();
        
    }
    
    #endregion

    #region Event Callbacks

    private async void BluetoothAdapterOnDeviceDiscovered(object? sender, DeviceEventArgs e)
    {
        await NotifyDeviceFoundIfMatch(e);
    }

    private void BluetoothAdapterOnDeviceAdvertised(object? sender, DeviceEventArgs e)
    {
        // Method intentionally left empty.
    }

    private void BluetoothAdapterOnScanTimeoutElapsed(object? sender, EventArgs e)
    {
        // Method intentionally left empty.
    }

    private void BluetoothManagerOnStateChanged(object? sender, BluetoothStateChangedArgs e)
    {
        _logService.LogInformation($"BluetoothManagerOnStateChanged: Current Bluetooth state changed to {e.NewState.ToString()}");
        BluetoothState = e.NewState;
        SetBluetoothStateText();
    }

    #endregion

    #region Dispose

    public async ValueTask DisposeAsync()
    {
        
        if (_bluetoothAdapter == null)
            return;
            
        if (_connectedDevice != null)
            await _bluetoothAdapter.DisconnectDeviceAsync(_connectedDevice);
    
        _connectedDevice?.Dispose();
        _connectedDeviceService?.Dispose();
        
        _incomingMessageCharacteristic = null;
        _connectedDeviceService = null;
        _connectedDevice = null;
        _scannedDevices.Clear();
      
        
    }

    #endregion
  
}