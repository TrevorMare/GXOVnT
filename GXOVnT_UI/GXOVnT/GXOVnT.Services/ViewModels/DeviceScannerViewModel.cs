using System.ComponentModel;
using GXOVnT.Services.Common;
using GXOVnT.Services.Interfaces;
using GXOVnT.Services.Models;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.DeviceMessage;


namespace GXOVnT.Services.ViewModels;

public class DeviceScannerViewModel : NotifyChanged
{

    public delegate void OnCommMessageReceivedHandler(object sender, CommMessage commMessage);

    public event OnCommMessageReceivedHandler? OnCommMessageReceived;

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
        _bluetoothService.OnMessagePacketReceived -= BluetoothServiceOnMessagePacketReceived;
        _bluetoothService.OnMessagePacketReceived += BluetoothServiceOnMessagePacketReceived;
    }

    public async Task StartScanGXOVnTDevicesAsync()
    {
        _scannedDevices.Clear();
        _scanGXOVnTCancellationToken = new CancellationTokenSource();
        
        _logViewModel.LogInformation("Starting scan for GXOVnT devices");
        
        await _bluetoothService.StartScanForDevicesAsync(_scanGXOVnTCancellationToken);
    }
    
    public async Task StopScanGXOVnTDevicesAsync()
    {
        _logViewModel.LogInformation("Stopping scan for GXOVnT devices");
        
        await _scanGXOVnTCancellationToken.CancelAsync();
        _scanGXOVnTCancellationToken.Dispose();
    }
    
    public async Task ConnectToDevice(GXOVnTDevice device)
    {
        _logViewModel.LogInformation($"Attempting connection to device Id {device.Id}");
        await _bluetoothService.ConnectToDevice(Guid.Parse(device.Id));
    }
    
    public async Task DisConnectFromDevice()
    {
        _logViewModel.LogInformation($"Disconnecting from device");
        await _bluetoothService.DisConnectFromDevice();
    }

    public async Task SendProtoMessage(string message)
    {
        _logViewModel.LogInformation($"Sending message to device");
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

    private readonly List<CommMessage> _receivedMessages = new List<CommMessage>();
    
    private void BluetoothServiceOnMessagePacketReceived(object sender, byte[]? buffer)
    {
        
        if (buffer == null) return;
        _logViewModel.LogInformation($"Message received from device. Inspecting packet");
        
        var commMessagePacket = new CommMessagePacket();
        commMessagePacket.DeSerializePacket(buffer);

        _logViewModel.LogInformation($"Comm Message packet with Id {commMessagePacket.CommMessagePacketId} for message Id {commMessagePacket.CommMessageId}");
        _logViewModel.LogInformation($"Packet start: {commMessagePacket.CommMessageDetail.HasFlag(CommMessageDetail.IsStartPacket)}, Packet end: {commMessagePacket.CommMessageDetail.HasFlag(CommMessageDetail.IsEndPacket)}");

        var bufferDataList = buffer.Select(b => b.ToString());
        _logViewModel.LogInformation($"BufferData: {string.Join(" ", bufferDataList)}");
        
        
        
        var commMessage = _receivedMessages.Find(x => commMessagePacket.CommMessageId == x.MessageId);
        if (commMessage == null)
        {
            _logViewModel.LogInformation($"Message does not exist, creating it");
            
            commMessage = new CommMessage(commMessagePacket);
            _receivedMessages.Add(commMessage);
            return;
        }

        commMessage.AddCommMessagePacket(commMessagePacket);

        if (commMessage.MessageComplete)
        {
            _logViewModel.LogInformation($"Message is now complete, processing message");
            OnCommMessageReceived?.Invoke(this, commMessage);
            _receivedMessages.Remove(commMessage);
        }
        
    }


    #endregion

}