using System.ComponentModel;
using GXOVnT.Services.Models;
using Plugin.BLE.Abstractions.Contracts;

namespace GXOVnT.Services.Interfaces;

public interface IBluetoothService : INotifyPropertyChanged, IAsyncDisposable
{
    
    
    BluetoothState BluetoothState { get; }
    
    bool BluetoothIsReady { get; }
    
    bool IsScanningDevices { get; }
    
    IReadOnlyList<GXOVnTBleDevice> DiscoveredDevices { get; }

    Task<bool> StartScanForDevicesAsync(CancellationTokenSource? cancellationTokenSource = default);

    Task StopScanForDevicesAsync();

    GXOVnTBleDevice? FindDevice(Guid deviceId);


}