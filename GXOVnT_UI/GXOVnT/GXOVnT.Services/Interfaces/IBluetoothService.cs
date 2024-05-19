using System.ComponentModel;
using Plugin.BLE.Abstractions.Contracts;

namespace GXOVnT.Services.Interfaces;

public interface IBluetoothService : INotifyPropertyChanged
{
    event BluetoothService.OnDeviceFoundHandler? OnDeviceFound;
        
    Task<bool> InitializeService();

    BluetoothState BluetoothState { get; }
    
    bool BluetoothIsReady { get; }
    
    bool IsScanningDevices { get; }

    Task StartScanForDevicesAsync(CancellationTokenSource? cancellationTokenSource = default);

}