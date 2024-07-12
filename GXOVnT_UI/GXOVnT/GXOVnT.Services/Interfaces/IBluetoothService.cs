using GXOVnT.Shared.Interfaces;
using Plugin.BLE.Abstractions.Contracts;

namespace GXOVnT.Services.Interfaces;

public interface IBluetoothService : IStateObject, IAsyncDisposable
{
    
    
    BluetoothState BluetoothState { get; }
    
    bool BluetoothIsReady { get; }
    
    bool IsScanningDevices { get; }
    
    IReadOnlyList<Models.System> DiscoveredDevices { get; }

    Task<bool> StartScanForDevicesAsync(CancellationTokenSource? cancellationTokenSource = default);

    Task StopScanForDevicesAsync();

    Models.System? FindDevice(Guid deviceId);


}