using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using GXOVnT.Services.Common;
using GXOVnT.Services.Interfaces;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Extensions;


namespace GXOVnT.Services.ViewModels;

public class BLEScannerViewModel : NotifyChanged
{


    #region Members

    private readonly IBluetoothService _bluetoothService;
    private readonly LogViewModel _logViewModel;
    
    private bool _viewModelIsValid;
    private CancellationTokenSource? _scanGXOVnTCancellationToken;

    #endregion

    #region Properties

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
        _logViewModel = logViewModel;
    }

    #endregion

    #region Methods

    public async Task InitializeViewModel()
    {
        ViewModelIsValid = await _bluetoothService.InitializeService();
    }

    public async Task StartScanGXOVnTDevices()
    {

        if (!_bluetoothService.IsBluetoothReady())
        {
            _logViewModel.LogError("Bluetooth service is not ready. Unable to scan for devices");
            return;
        }
            
        
        
        _scanGXOVnTCancellationToken = new CancellationTokenSource();

    }
    
    public void StopScanGXOVnTDevices()
    {
        if (_scanGXOVnTCancellationToken != null)
        {
            _scanGXOVnTCancellationToken.Cancel();
            _scanGXOVnTCancellationToken.Dispose();
        }
        
        _scanGXOVnTCancellationToken = null;
    }
    
    #endregion
    
    
    
    

    public ObservableCollection<BLEDeviceViewModel> BLEDevices { get; private init; } = [];

    private bool _isScanning = false;
    public bool IsScanning
    {
        get => _isScanning;
        protected set
        {
            if (_isScanning != value) {
                _isScanning = value;
                DebugMessage($"Set IsScanning to {value}");
                RaisePropertyChanged(nameof(IsScanning));
                RaisePropertyChanged(nameof(Waiting));
                RaisePropertyChanged(nameof(ScanState));
                RaisePropertyChanged(nameof(ToggleScanningCmdLabelText));
            }
        }
    }

    #region Derived properties
    public bool IsStateOn => _bluetoothManager.IsOn;
    public string StateText => GetStateText();
    public bool Waiting => !_isScanning;
    public string ScanState => IsScanning ? "Scanning" : "Waiting";
    public string ToggleScanningCmdLabelText => IsScanning ? "Cancel" : "Start Scan";
    #endregion Derived properties

    private string GetStateText()
    {
        var result = "Unknown BLE state.";
        switch (_bluetoothManager.State)
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
        return result;
    }

    private void ShowMessage(string message)
    {
        DebugMessage(message);
        Class1.AlertSvc.ShowAlert("BLE Scanner", message);
    }

    
    private void OnBluetoothStateChanged(object sender, BluetoothStateChangedArgs e)
    {
        DebugMessage("OnBluetoothStateChanged from " + e.OldState + " to " + e.NewState);
        RaisePropertyChanged(nameof(IsStateOn));
        RaisePropertyChanged(nameof(StateText));
    }

    #region Scan & Discover
    public ICommand ToggleScanning { get; init; }
    CancellationTokenSource _scanCancellationTokenSource = null;

    private void Adapter_ScanTimeoutElapsed(object sender, EventArgs e)
    {
        DebugMessage("Adapter_ScanTimeoutElapsed");
        // Cleanup will happen inside ScanForDevicesAsync
    }

    private void OnDeviceAdvertised(object sender, DeviceEventArgs args)
    {
        DebugMessage("OnDeviceAdvertised");
        AddOrUpdateDevice(args.Device);
        DebugMessage("OnDeviceAdvertised done");
    }
    private void OnDeviceDiscovered(object sender, DeviceEventArgs args)
    {
        DebugMessage("OnDeviceDiscovered");
        AddOrUpdateDevice(args.Device);
        DebugMessage("OnDeviceDiscovered done");
    }

    private void AddOrUpdateDevice(IDevice device)
    {
        MainThread.BeginInvokeOnMainThread(() => {
            var vm = BLEDevices.FirstOrDefault(d => d.DeviceId == device.Id);
            if (vm != null)
            {
                DebugMessage($"Update Device: {device.Id}");
                vm.Update(device);
            }
            else
            {
                DebugMessage($"Add Device: {device.Id}");
                vm = new BLEDeviceViewModel(device);
                BLEDevices.Add(vm);
            }
        });
    }

    private void ToggleScanForDevices()
    {
        if (!IsScanning)
        {
            IsScanning = true;
            DebugMessage($"Starting Scanning");
            ScanForDevicesAsync();
            DebugMessage($"Started Scan");
        }
        else
        {
            DebugMessage($"Request Stopping Scan");
            _scanCancellationTokenSource?.Cancel();
            DebugMessage($"Stop Scanning Requested");
        }
    }
    private async void ScanForDevicesAsync()
    {
        if (!IsStateOn)
        {
            ShowMessage("Bluetooth is not ON.\nPlease turn on Bluetooth and try again.");
            IsScanning = false;
            return;
        }
        if (!await HasCorrectPermissions())
        {
            DebugMessage("Aborting scan attempt");
            IsScanning = false;
            return;
        }
        DebugMessage("StartScanForDevices called");
        BLEDevices.Clear();
        await UpdateConnectedDevices();

        _scanCancellationTokenSource = new();

        DebugMessage("call Adapter.StartScanningForDevicesAsync");
        await Adapter.StartScanningForDevicesAsync(_scanCancellationTokenSource.Token);
        DebugMessage("back from Adapter.StartScanningForDevicesAsync");

        // Scanning stopped (for whichever reason) -> cleanup
        _scanCancellationTokenSource.Dispose();
        _scanCancellationTokenSource = null;
        IsScanning = false;
    }

    private async Task UpdateConnectedDevices()
    {
        foreach (var connectedDevice in Adapter.ConnectedDevices)
        {
            //update rssi for already connected devices (so that 0 is not shown in the list)
            try
            {
                await connectedDevice.UpdateRssiAsync();
            }
            catch (Exception ex)
            {
                ShowMessage($"Failed to update RSSI for {connectedDevice.Name}. Error: {ex.Message}");
            }

            AddOrUpdateDevice(connectedDevice);
        }
    }

    #endregion Scan & Discover
}