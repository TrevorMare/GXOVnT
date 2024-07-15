using System.ComponentModel;
using GXOVnT.Services.Interfaces;
using GXOVnT.Services.Models;
using Microsoft.AspNetCore.Components;

namespace GXOVnT.Components.Shared.Bluetooth;

public partial class DeviceScanner : GXOVnTComponent
{

    #region Members

    private string _selectedUUId = string.Empty; 

    #endregion
    
    #region Properties

    [Inject]
    private IBluetoothService BluetoothService { get; set; } = default!;
    
    [Parameter] 
    public EventCallback<Services.Models.System?> DeviceSelected { get; set; }
    
    private bool IsScanningDevices => BluetoothService.IsScanningDevices;

    private bool HasItems => BluetoothService.DiscoveredDevices.Count > 0;

    private string ScanButtonText => (IsScanningDevices ? "Stop" : "Start") + " scan devices";
   
    #endregion

    #region Methods

    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        
        BluetoothService.PropertyChanged -= DeviceScannerViewModelOnPropertyChanged;
        BluetoothService.PropertyChanged += DeviceScannerViewModelOnPropertyChanged;
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (firstRender) 
            _selectedUUId = string.Empty;
    }

    #endregion

    #region Event Callbacks
    
    private async Task OnDeviceListItemClick(Services.Models.System item)
    {
        _selectedUUId = item.UUID;
        
        await DeviceSelected.InvokeAsync(item);
        
        
    }
    
    public async Task ToggleScanDevices()
    {
        try
        {
            if (BluetoothService.IsScanningDevices)
                await BluetoothService.StopScanForDevicesAsync();
            else
            {
                // Clear the selected device when we start scanning
                await DeviceSelected.InvokeAsync(null);
                // Now perform the scan
                await BluetoothService.StartScanForDevicesAsync();
            }
        }
        catch (Exception)
        {
            LogService.LogError("There was an internal error scanning for the devices");
        }
        finally
        {
            
        }
    }

    private void DeviceScannerViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }
    #endregion

}