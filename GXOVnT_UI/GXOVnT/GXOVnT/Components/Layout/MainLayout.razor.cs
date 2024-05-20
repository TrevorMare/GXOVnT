using System.ComponentModel;
using GXOVnT.Services.ViewModels;
using Microsoft.AspNetCore.Components;

namespace GXOVnT.Components.Layout;

public partial class MainLayout
{


    [Inject] 
    private IServiceProvider ServiceProvider { get; set; } = default!;
    
    [Inject]
    private VMDeviceScanner VmDeviceScanner { get; set; } = default!;

    public MainLayout()
    {
        
    }
    
    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        VmDeviceScanner.PropertyChanged -= VmDeviceScannerOnPropertyChanged;
        VmDeviceScanner.PropertyChanged += VmDeviceScannerOnPropertyChanged;
    }

    private void VmDeviceScannerOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    bool _drawerOpen = true;

    void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }

    private async Task Test()
    {
        if (VmDeviceScanner.IsScanningDevices)
        {
            await VmDeviceScanner.StopScanGXOVnTDevicesAsync();    
        }
        else
        {
            await VmDeviceScanner.InitializeViewModel();
        
            await VmDeviceScanner.StartScanGXOVnTDevicesAsync();
        }
    }
}