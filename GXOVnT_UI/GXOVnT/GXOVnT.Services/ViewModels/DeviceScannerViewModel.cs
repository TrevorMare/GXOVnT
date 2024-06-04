using System.ComponentModel;
using GXOVnT.Services.Common;
using GXOVnT.Services.Interfaces;
using GXOVnT.Services.Models;
using GXOVnT.Shared.DeviceMessage;
using GXOVnT.Shared.JsonModels;

namespace GXOVnT.Services.ViewModels;

public class DeviceScannerViewModel : NotifyChanged
{

    public delegate void OnCommMessageReceivedHandler(object sender, CommMessage commMessage);

    public event OnCommMessageReceivedHandler? OnCommMessageReceived;

    #region Members

    private readonly IBluetoothService _bluetoothService;
    private readonly IMessageOrchestrator _messageOrchestrator;
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
        LogViewModel logViewModel, IMessageOrchestrator messageOrchestrator)
    {
        _bluetoothService = bluetoothService;
        _messageOrchestrator = messageOrchestrator;
        _bluetoothService.PropertyChanged += BluetoothServiceOnPropertyChanged;
        _messageOrchestrator.PropertyChanged += MessageOrchestratorOnPropertyChanged;
        _messageOrchestrator.MessageAggregateReceived += MessageOrchestratorOnMessageAggregateReceived;
        
        
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

        var echoModel = new Shared.JsonModels.EchoModel()
        {
            EchoMessage = message
        };

        var response = await _messageOrchestrator.SendMessage<EchoModel, EchoModel>(echoModel);

        if (response != null)
        {
            _logViewModel.LogDebug($"Response received from send message: {response.EchoMessage}");
        }
        
        
       
    }
    #endregion

    #region Event Callbacks

    private void BluetoothServiceOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(_bluetoothService));
    }
    
    private void MessageOrchestratorOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(_messageOrchestrator));
    }

    private void MessageOrchestratorOnMessageAggregateReceived(object sender, MessageAggregate messageAggregate)
    {
        _logViewModel.LogDebug($"Received a message from device with Id {messageAggregate.MessageId} and message type {messageAggregate.BaseModel?.MessageTypeId}");
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