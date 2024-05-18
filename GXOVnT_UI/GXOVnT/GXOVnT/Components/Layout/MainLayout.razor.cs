using GXOVnT.Services.ViewModels;

namespace GXOVnT.Components.Layout;

public partial class MainLayout
{
    private BLEScannerViewModel _bleScannerViewModel;
    
    protected override void OnInitialized()
    {
        base.OnInitialized();
        

    }

    private Task Test()
    {
        _bleScannerViewModel = new BLEScannerViewModel();
        _bleScannerViewModel.ToggleScanning.Execute(null);
        
        return Task.CompletedTask;
        
    }
}