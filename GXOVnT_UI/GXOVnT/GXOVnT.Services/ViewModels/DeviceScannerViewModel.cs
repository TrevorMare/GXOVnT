using System.ComponentModel;
using GXOVnT.Services.Common;
using GXOVnT.Services.Interfaces;
using GXOVnT.Services.Models;


namespace GXOVnT.Services.ViewModels;

public class DeviceScannerViewModel : NotifyChanged
{


    #region Members

    private readonly IBluetoothService _bluetoothService;
    private readonly LogViewModel _logViewModel;
    
    private CancellationTokenSource _scanGXOVnTCancellationToken = default!;
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

    public DeviceScannerViewModel(IBluetoothService bluetoothService,
        LogViewModel logViewModel)
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
        
        await _bluetoothService.StartScanForDevicesAsync(_scanGXOVnTCancellationToken);
    }
    
    public async Task StopScanGXOVnTDevicesAsync()
    {
        await _scanGXOVnTCancellationToken.CancelAsync();
        _scanGXOVnTCancellationToken.Dispose();
    }
    
    public async Task ConnectToDevice(GXOVnTDevice device)
    {
        await _bluetoothService.ConnectToDevice(Guid.Parse(device.Id));
    }
    
    public async Task DisConnectFromDevice()
    {
        await _bluetoothService.DisConnectFromDevice();
    }

    public async Task SendProtoMessage(string message)
    {
        await _bluetoothService.SendProtoMessageToConnectedDevice(message);
    }
    #endregion

    #region Event Callbacks

    private void BluetoothServiceOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(_bluetoothService));
    }

    private async void BluetoothServiceOnDeviceFound(object sender, BluetoothService.GXOVnTDeviceFoundArgs e)
    {
        var gxovntDevice = _scannedDevices.Find(d => d.DeviceName.Equals(e.DeviceArgs.Device.Name));
        if (gxovntDevice == null)
        {
            lock (_scannedDevices)
            {
                gxovntDevice = new GXOVnTDevice()
                {
                    Id = e.DeviceArgs.Device.Id.ToString(),
                    Rssi = e.DeviceArgs.Device.Rssi,
                    DeviceName = e.DeviceArgs.Device.Name,
                    DeviceState = e.DeviceArgs.Device.State,
                    IsConnectable = e.DeviceArgs.Device.IsConnectable
                };

                _scannedDevices.Add(gxovntDevice);
            }
        }
        await e.DeviceArgs.Device.UpdateRssiAsync();
        gxovntDevice.Rssi = e.DeviceArgs.Device.Rssi;
        gxovntDevice.DeviceState = e.DeviceArgs.Device.State;
        gxovntDevice.IsConnectable = e.DeviceArgs.Device.IsConnectable;
        gxovntDevice.SystemConfigured = e.SystemConfigured;
        gxovntDevice.SystemType = e.SystemType;
        
        OnPropertyChanged(nameof(ScannedDevices));
    }

    #endregion

}