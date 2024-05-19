using System.ComponentModel;
using GXOVnT.Services.Common;
using GXOVnT.Services.Interfaces;
using GXOVnT.Services.Models;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;

namespace GXOVnT.Services.ViewModels;

public class BLEScannerViewModel : NotifyChanged
{


    #region Members

    private readonly IBluetoothService _bluetoothService;
    private readonly LogViewModel _logViewModel;
    
    private bool _viewModelIsValid;
    private CancellationTokenSource _scanGXOVnTCancellationToken;
    private readonly List<GXOVnTDevice> _scannedDevices = new();

    #endregion

    #region Properties

    public bool IsScanningDevices => _bluetoothService.IsScanningDevices;
    
    public IReadOnlyList<GXOVnTDevice> ScannedDevices
    {
        get => _scannedDevices;
    }
    
    public bool ViewModelIsValid
    {
        get => _viewModelIsValid;
        private set => SetField(ref _viewModelIsValid, value);
    }

    #endregion
    
    #region ctor

    public BLEScannerViewModel(IBluetoothService bluetoothService,
        LogViewModel logViewModel)
    {
        _bluetoothService = bluetoothService;
        _bluetoothService.PropertyChanged += BluetoothServiceOnPropertyChanged;
        _logViewModel = logViewModel;
    }

    #endregion

    #region Methods

    public async Task InitializeViewModel()
    {
        
        _bluetoothService.OnDeviceFound -= BluetoothServiceOnOnDeviceFound;
        _bluetoothService.OnDeviceFound += BluetoothServiceOnOnDeviceFound;
        
        ViewModelIsValid = await _bluetoothService.InitializeService();
    }

    public async Task StartScanGXOVnTDevicesAsync()
    {
        
        _scannedDevices.Clear();
        
        if (!_bluetoothService.BluetoothIsReady)
        {
            _logViewModel.LogError("Bluetooth service is not ready. Unable to scan for devices");
            return;
        }
        _scanGXOVnTCancellationToken = new CancellationTokenSource();
        
        await _bluetoothService.StartScanForDevicesAsync(_scanGXOVnTCancellationToken);
    }
    
    public async Task StopScanGXOVnTDevicesAsync()
    {
        await _scanGXOVnTCancellationToken.CancelAsync();
        _scanGXOVnTCancellationToken.Dispose();
    }
    
    #endregion

    #region Event Callbacks

    private void BluetoothServiceOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(_bluetoothService));
    }

    private void BluetoothServiceOnOnDeviceFound(object sender, DeviceEventArgs e)
    {
        AddOrUpdateDevice(e.Device);
    }

    #endregion
    
    

    #region Scan & Discover
    private async void AddOrUpdateDevice(IDevice device)
    {

     
        
        
        var gxovntDevice = _scannedDevices.Find(d => d.DeviceName.Equals(device.Name));
        if (gxovntDevice == null)
        {
            lock (_scannedDevices)
            {
                gxovntDevice = new GXOVnTDevice()
                {
                    Id = device.Id.ToString(),
                    Rssi = device.Rssi,
                    DeviceName = device.Name,
                    DeviceState = device.State,
                    IsConnectable = device.IsConnectable
                };

                _scannedDevices.Add(gxovntDevice);
            }
        }
        await device.UpdateRssiAsync();
        gxovntDevice.Rssi = device.Rssi;
        gxovntDevice.DeviceState = device.State;
        gxovntDevice.IsConnectable = device.IsConnectable;
        
        OnPropertyChanged(nameof(ScannedDevices));

    }

  
   

    // private async Task UpdateConnectedDevices()
    // {
    //     foreach (var connectedDevice in Adapter.ConnectedDevices)
    //     {
    //         //update rssi for already connected devices (so that 0 is not shown in the list)
    //         try
    //         {
    //             await connectedDevice.UpdateRssiAsync();
    //         }
    //         catch (Exception ex)
    //         {
    //             ShowMessage($"Failed to update RSSI for {connectedDevice.Name}. Error: {ex.Message}");
    //         }
    //
    //         AddOrUpdateDevice(connectedDevice);
    //     }
    // }

    #endregion Scan & Discover
}