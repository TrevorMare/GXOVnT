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

    #region Events

    private const string GXOVnTManufacturerValue = "GXOVnT";

    public delegate void OnDeviceFoundHandler(object sender, DeviceEventArgs e);

    public event OnDeviceFoundHandler? OnDeviceFound;

    #endregion
    
    #region Members

    private readonly IAlertService _alertService;
    private readonly IRequestPermissionService _requestPermissionService;
    private readonly LogViewModel _logViewModel;
    private IBluetoothLE? _bluetoothManager;
    private IAdapter? _bluetoothAdapter;
    
    private bool _isInitialized;
    private bool _bluetoothIsReady;
    private BluetoothState _bluetoothState = BluetoothState.Unknown;
    private string _bluetoothStateText = "";
    private bool _isScanningDevices;
    private CancellationTokenSource _scanCancellationTokenSource = default!;
    #endregion

    #region Properties

    public string BluetoothStateText
    {
        get => _bluetoothStateText;
        private set => SetField(ref _bluetoothStateText, value);
    }
    
    public BluetoothState BluetoothState
    {
        get => _bluetoothState;
        private set => SetField(ref _bluetoothState, value);
    }

    public bool BluetoothIsReady
    {
        get => _bluetoothIsReady;
        private set => SetField(ref _bluetoothIsReady, value);
    }

    public bool IsScanningDevices
    {
        get => _isScanningDevices;
        private set => SetField(ref _isScanningDevices, value);
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

    public async Task<bool> InitializeService()
    {
        if (_isInitialized)
            return true;

        BluetoothIsReady = false;
        
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
        BluetoothIsReady = true;

        return true;
    }

    public async Task StartScanForDevicesAsync(CancellationTokenSource? cancellationTokenSource = default)
    {
        if (!BluetoothIsReady)
        {
            await _alertService.ShowAlertAsync("Bluetooth", "Bluetooth is not ON.\nPlease turn on Bluetooth and try again.");
            IsScanningDevices = false;
            return;
        }

        _scanCancellationTokenSource = cancellationTokenSource ?? new CancellationTokenSource();

        IsScanningDevices = true;

        await _bluetoothAdapter.StartScanningForDevicesAsync(_scanCancellationTokenSource.Token);

        IsScanningDevices = false;
    }

    public async Task StopScanForDevicesAsync()
    {
        await _scanCancellationTokenSource.CancelAsync();
    }
    
    #endregion

    #region Private Methods

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
        _bluetoothAdapter.ScanMode = ScanMode.Balanced;
        
        _bluetoothAdapter.ScanTimeout = 30000; // ms
        
    }

    #endregion

    #region Event Callbacks

    private void BluetoothAdapterOnDeviceDiscovered(object? sender, DeviceEventArgs e)
    {
        var allAdvertisementRecords = e.Device.AdvertisementRecords ?? new List<AdvertisementRecord>();
        if (!allAdvertisementRecords.Any())
            return;

        var manufacturerData = allAdvertisementRecords.ToList()
            .Find(r => r.Type == AdvertisementRecordType.ManufacturerSpecificData);

        if (manufacturerData == null)
            return;

        var dataString = System.Text.Encoding.UTF8.GetString(manufacturerData.Data);

        if (!dataString.Equals(GXOVnTManufacturerValue, StringComparison.CurrentCultureIgnoreCase))
            return;
        
        OnDeviceFound?.Invoke(this, e);
    }

    private async void BluetoothAdapterOnDeviceAdvertised(object? sender, DeviceEventArgs e)
    {
        

        // var services = await e.Device.GetServicesAsync();
        //
        // if (services.Count > 0)
        // {
        //     int breakPoint = 0;
        // }
        
        // if (e.Device.Name.Equals("MySystemName_34b7da63b5"))
        // {
        //     var allAdvertisementRecords = e.Device.AdvertisementRecords;
        //     if (allAdvertisementRecords != null)
        //     {
        //
        //         foreach (var record in allAdvertisementRecords)
        //         {
        //             
        //             Console.WriteLine(record.Type.ToString());
        //             
        //         }
        //         
        //         
        //         int breakPoint = 0;
        //         
        //         
        //     }
        // }
        
        //OnDeviceFound?.Invoke(this, e);
    }

    private void BluetoothAdapterOnScanTimeoutElapsed(object? sender, EventArgs e)
    {
        // Not sure what this is for now
    }

    private void BluetoothManagerOnStateChanged(object? sender, BluetoothStateChangedArgs e)
    {
        _logViewModel.LogInformation($"Current Bluetooth state changed to {e.NewState.ToString()}");
        BluetoothState = e.NewState;
        BluetoothIsReady = (_bluetoothState == BluetoothState.On);
        SetBluetoothStateText();
    }

    #endregion
    
}