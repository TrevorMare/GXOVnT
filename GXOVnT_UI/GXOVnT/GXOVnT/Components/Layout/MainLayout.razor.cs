using System.ComponentModel;
using GXOVnT.Services.ViewModels;
using Microsoft.AspNetCore.Components;

namespace GXOVnT.Components.Layout;

public partial class MainLayout
{


    [Inject] 
    private IServiceProvider ServiceProvider { get; set; } = default!;
    
    [Inject]
    private BLEScannerViewModel BLEScannerViewModel { get; set; } = default!;

    public MainLayout()
    {
        
    }
    
    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        BLEScannerViewModel.PropertyChanged -= BLEScannerViewModelOnPropertyChanged;
        BLEScannerViewModel.PropertyChanged += BLEScannerViewModelOnPropertyChanged;
    }

    private void BLEScannerViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
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
        if (BLEScannerViewModel.IsScanningDevices)
        {
            await BLEScannerViewModel.StopScanGXOVnTDevicesAsync();    
        }
        else
        {
            await BLEScannerViewModel.InitializeViewModel();
        
            await BLEScannerViewModel.StartScanGXOVnTDevicesAsync();
        }
    }
}