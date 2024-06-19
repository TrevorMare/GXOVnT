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
    public EventCallback<GXOVnTBleDevice?> DeviceSelected { get; set; }
    
    private bool IsScanningDevices => BluetoothService.IsScanningDevices;

    private bool HasItems => BluetoothService.DiscoveredDevices.Count > 0;

    private string ScanButtonText => (IsScanningDevices ? "Stop" : "Start") + " scan devices";
   
    #endregion

    #region Methods

    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        SetWizardForwardEnabled(false);
        
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
    
    private async Task OnDeviceListItemClick(GXOVnTBleDevice item)
    {
        _selectedUUId = item.UUID;
        
        await DeviceSelected.InvokeAsync(item);
        SetWizardForwardEnabled(true);
    }
    
    public async Task ToggleScanDevices()
    {
        try
        {
            IsBusy = true;

            if (BluetoothService.IsScanningDevices)
                await BluetoothService.StopScanForDevicesAsync();
            else
            {
                SetWizardForwardEnabled(false);
                
                await DeviceSelected.InvokeAsync(null);
                await BluetoothService.StartScanForDevicesAsync();
            }
                
        }
        catch (Exception)
        {
            LogService.LogError("There was an internal error scanning for the devices");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void DeviceScannerViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }
    #endregion

}