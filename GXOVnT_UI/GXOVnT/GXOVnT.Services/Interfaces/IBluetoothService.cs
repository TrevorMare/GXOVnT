using System.ComponentModel;
using Plugin.BLE.Abstractions.Contracts;

namespace GXOVnT.Services.Interfaces;

public interface IBluetoothService : INotifyPropertyChanged, IAsyncDisposable
{
    event BluetoothService.OnDeviceFoundHandler? OnDeviceFound;

    bool IsConnectedToDevice { get; }
    
    BluetoothState BluetoothState { get; }
    
    bool BluetoothIsReady { get; }
    
    bool IsScanningDevices { get; }

    Task<bool> StartScanForDevicesAsync(CancellationTokenSource? cancellationTokenSource = default);

    Task StopScanForDevicesAsync();

    Task<bool> DisConnectFromDevice();

    Task<bool> ConnectToDevice(Guid deviceId);
    
    Task SendProtoMessageToConnectedDevice(string message);
}