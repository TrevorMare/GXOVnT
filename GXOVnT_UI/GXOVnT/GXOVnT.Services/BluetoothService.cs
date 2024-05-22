using GXOVnT.Services.Common;
using GXOVnT.Services.Interfaces;
using GXOVnT.Services.ViewModels;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Extensions;

namespace GXOVnT.Services;

public class BluetoothService : NotifyChanged, IBluetoothService 
{

    public record GXOVnTDeviceFoundArgs(
        DeviceEventArgs DeviceArgs,
        bool SystemConfigured,
        GXOVnTSystemType SystemType);
    
    #region Events

    private const string GXOVnTManufacturerValue = "GXOVnT";

    public delegate void OnDeviceFoundHandler(object sender, GXOVnTDeviceFoundArgs e);

    public event OnDeviceFoundHandler? OnDeviceFound;

    #endregion
    
    #region Members

    private readonly IAlertService _alertService;
    private readonly IRequestPermissionService _requestPermissionService;
    private readonly LogViewModel _logViewModel;
    private IBluetoothLE? _bluetoothManager;
    private IAdapter? _bluetoothAdapter;

    private bool _applicationHasBluetoothPermissions;
    private bool _bluetoothIsReady;
    private BluetoothState _bluetoothState = BluetoothState.Unknown;
    private string _bluetoothStateText = "";
    private bool _isScanningDevices;
    private CancellationTokenSource _scanCancellationTokenSource = default!;


    private IDevice? _connectedDevice;
    private IService? _connectedDeviceService;
    private ICharacteristic? _protoBleCharacteristic;
    private bool _isConnectedToDevice;
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
    public async Task<bool> StartScanForDevicesAsync(CancellationTokenSource? cancellationTokenSource = default)
    {
        try
        {
            IsScanningDevices = true;

            if (!await InitializeBluetooth())
                return false;
            
            _scanCancellationTokenSource = cancellationTokenSource ?? new CancellationTokenSource();
            
            await _bluetoothAdapter.StartScanningForDevicesAsync(_scanCancellationTokenSource.Token);

            IsScanningDevices = false;

            return true;

        }
        catch (Exception ex)
        {
            _logViewModel.LogError($"StartScanForDevicesAsync: An error occured scanning for devices. {ex.Message}");
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
                _protoBleCharacteristic = null;
                _connectedDeviceService = null;

                await _bluetoothAdapter.DisconnectDeviceAsync(_connectedDevice);
                _connectedDevice = null;
            }
            
            IsConnectedToDevice = false;
            return true;
        }
        catch (Exception ex)
        {
            _logViewModel.LogError($"DisConnectFromDevice: An error occured disconnecting from the device. {ex.Message}");
            return false;
        }
    }
    
    public async Task<bool> ConnectToDevice(Guid deviceId)
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
            
            _connectedDeviceService = await _connectedDevice.GetServiceAsync(Guid.Parse("05c1fba8-cc8b-4534-8787-0e6a0775c3de"));
            if (_connectedDeviceService == null)
                throw new ArgumentException(
                    "The device did not broadcast the expected service", nameof(deviceId));
            
            _protoBleCharacteristic = await _connectedDeviceService.GetCharacteristicAsync(Guid.Parse("4687b690-cd36-4a7c-9134-49ffe62d9e4f"));
            if (_protoBleCharacteristic == null)
                throw new ArgumentException(
                    "The device service did not broadcast the expected characteristic", nameof(deviceId));
            
            IsConnectedToDevice = true;
            return true;
        }
        catch (Exception ex)
        {
            await DisConnectFromDevice();
            _logViewModel.LogError($"StartScanForDevicesAsync: An error occured scanning for devices. {ex.Message}");
            return false;
        }
        
    }

    private Int16 _messageId = 1;
    
    public async Task SendProtoMessageToConnectedDevice(string message)
    {

        try
        {
            if (!IsConnectedToDevice)
                return;

            var bytes = System.Text.Encoding.UTF8.GetBytes(message).ToList();
            
            // Split the bytes into 10 byte packages
            var packets = bytes.Chunk(10).ToList();
    
            for (var iPacket = 0; iPacket < packets.Count; iPacket++)
            {
                var isFirstPacket = iPacket == 0;
                var isLastPacket = iPacket == packets.Count - 1;

                var packetInfo = 0;
                if (isFirstPacket)
                    packetInfo = 1;
                if (isLastPacket)
                    packetInfo += 2;

                var sendBuffer = new List<byte>()
                {
                    1, 1, (byte)iPacket, (byte)packetInfo
                };
                
                sendBuffer.AddRange(packets[iPacket]);

                await _protoBleCharacteristic.WriteAsync(sendBuffer.ToArray());

                await Task.Delay(200);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
        _messageId++;
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
                _logViewModel.LogWarning("InitializeBluetooth: Could not get a reference to the Bluetooth adapter");
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
            _logViewModel.LogError($"InitializeBluetooth: An error occured initializing the Bluetooth adapter. {ex.Message}");
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

    private void NotifyDeviceFoundIfMatch(DeviceEventArgs e)
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

        // It's in the format GXOVnT|X|Y where X and Y are 1 or 0. X represents the system type 
        // and Y represents if the system has been configured
        var manufacturerParts = manufacturerDataValue.Split("|");
        var systemConfigured = false;
        var systemType = GXOVnTSystemType.UnInitialized;

        if (manufacturerParts.Length >= 2)
            systemType = Enumeration.FromValue<GXOVnTSystemType>(int.Parse(manufacturerParts[1])) ?? GXOVnTSystemType.UnInitialized;
        if (manufacturerParts.Length >= 3)
            systemConfigured = manufacturerParts[2] == "1";
        
        OnDeviceFound?.Invoke(this, new GXOVnTDeviceFoundArgs(e, systemConfigured, systemType));        
    }
    
    #endregion

    #region Event Callbacks

    private void BluetoothAdapterOnDeviceDiscovered(object? sender, DeviceEventArgs e)
    {
        NotifyDeviceFoundIfMatch(e);
    }

    private async void BluetoothAdapterOnDeviceAdvertised(object? sender, DeviceEventArgs e)
    {
        // Method intentionally left empty.
    }

    private void BluetoothAdapterOnScanTimeoutElapsed(object? sender, EventArgs e)
    {
        // Method intentionally left empty.
    }

    private void BluetoothManagerOnStateChanged(object? sender, BluetoothStateChangedArgs e)
    {
        _logViewModel.LogInformation($"BluetoothManagerOnStateChanged: Current Bluetooth state changed to {e.NewState.ToString()}");
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
        
        _protoBleCharacteristic = null;
        _connectedDeviceService = null;
        _connectedDevice = null;
      
        
    }

    #endregion
  
}