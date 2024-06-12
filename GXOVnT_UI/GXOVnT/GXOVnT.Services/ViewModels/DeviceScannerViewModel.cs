using System.ComponentModel;
using GXOVnT.Services.Common;
using GXOVnT.Services.Interfaces;
using GXOVnT.Services.Models;

namespace GXOVnT.Services.ViewModels;

public class DeviceScannerViewModel : NotifyChanged, IAsyncDisposable
{

    #region Members
    private readonly IBluetoothService _bluetoothService;
    private readonly LogViewModel _logViewModel;
    
    private CancellationTokenSource? _scanGXOVnTCancellationToken;
    private readonly List<GXOVnTDevice> _scannedDevices = new();
    #endregion

    #region Properties

    public bool IsScanningDevices => _bluetoothService.IsScanningDevices;
    
    public bool IsConnectedToDevice => _bluetoothService.IsConnectedToDevice;
    
    public IReadOnlyList<GXOVnTDevice> ScannedDevices
    {
        get => _scannedDevices;
    }

    #endregion
    
    #region ctor

    public DeviceScannerViewModel(IBluetoothService bluetoothService, LogViewModel logViewModel)
    {
        _bluetoothService = bluetoothService;
        _bluetoothService.PropertyChanged += BluetoothServiceOnPropertyChanged;
        _logViewModel = logViewModel;
    }
    #endregion

    #region Methods

    public void InitializeViewModel()
    {
        _bluetoothService.OnDeviceFound -= BluetoothServiceOnDeviceFound;
        _bluetoothService.OnDeviceFound += BluetoothServiceOnDeviceFound;
    }

    public async Task StartScanGXOVnTDevicesAsync()
    {
        _scannedDevices.Clear();
        _scanGXOVnTCancellationToken = new CancellationTokenSource();
        
        _logViewModel.LogInformation("Starting scan for GXOVnT devices");
        
        await _bluetoothService.StartScanForDevicesAsync(_scanGXOVnTCancellationToken);

        _scanGXOVnTCancellationToken = null;
    }
    
    public async Task StopScanGXOVnTDevicesAsync()
    {
        _logViewModel.LogInformation("Stopping scan for GXOVnT devices");

        if (_scanGXOVnTCancellationToken != null)
        {
            await _scanGXOVnTCancellationToken.CancelAsync();
            _scanGXOVnTCancellationToken = null;
        }
    }
    
    public async Task ConnectToDevice(GXOVnTDevice device, bool keepConnectionAlive)
    {
        _logViewModel.LogInformation($"Attempting connection to device Id {device.Id}");
        await _bluetoothService.ConnectToDevice(Guid.Parse(device.Id), keepConnectionAlive);
    }
    
    public async Task DisConnectFromDevice()
    {
        _logViewModel.LogInformation($"Disconnecting from device");
        await _bluetoothService.DisConnectFromDevice();
    }

    
    #endregion

    #region Event Callbacks

    private void BluetoothServiceOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(_bluetoothService));
    }

    private void BluetoothServiceOnDeviceFound(object sender, BluetoothService.GXOVnTDeviceFoundArgs e)
    {
        OnPropertyChanged(nameof(ScannedDevices));
    }
    #endregion

    #region Dispose Async

    public async ValueTask DisposeAsync()
    {
        if (_scanGXOVnTCancellationToken != null)
        {
            await _scanGXOVnTCancellationToken.CancelAsync();
            _scanGXOVnTCancellationToken.Dispose();
        }
    }

    #endregion
}