using System.ComponentModel;
using GXOVnT.Services.Models;
using GXOVnT.Services.ViewModels;
using Microsoft.AspNetCore.Components;

namespace GXOVnT.Components.Shared;

public partial class DeviceScanner : ComponentBase
{

    #region Members

    public string SendText { get; set; } = string.Empty;

    #endregion

    #region Properties

    [Parameter] public bool Enabled { get; set; } = true;
    
    [Parameter] public EventCallback<GXOVnTDevice> DeviceSelected { get; set; }

    [Inject] private DeviceScannerViewModel DeviceScannerViewModel { get; set; } = default!;

    private bool IsScanningDevices => DeviceScannerViewModel.IsScanningDevices;

    private bool HasItems => DeviceScannerViewModel.ScannedDevices.Count > 0;

    private string ScanButtonText => (IsScanningDevices ? "Stop" : "Start") + " scan devices";
    
   
    #endregion

    #region Methods

    protected override void OnInitialized()
    {
        base.OnInitialized();
        DeviceScannerViewModel.PropertyChanged -= DeviceScannerViewModelOnPropertyChanged;
        DeviceScannerViewModel.PropertyChanged += DeviceScannerViewModelOnPropertyChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender) return;

        DeviceScannerViewModel.InitializeViewModel();
    }

    #endregion

    #region Event Callbacks

    private async Task OnDeviceListItemClick(GXOVnTDevice item)
    {
        await DeviceSelected.InvokeAsync(item);
        await DeviceScannerViewModel.ConnectToDevice(item);
    }
    
    public async Task ToggleScanDevices()
    {
        if (DeviceScannerViewModel.IsScanningDevices)
            await DeviceScannerViewModel.StopScanGXOVnTDevicesAsync();
        else
            await DeviceScannerViewModel.StartScanGXOVnTDevicesAsync();
    }
    
    public async Task SendMessage()
    {
        await DeviceScannerViewModel.SendProtoMessage(SendText);
    }
    

    private void DeviceScannerViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }
    #endregion

}