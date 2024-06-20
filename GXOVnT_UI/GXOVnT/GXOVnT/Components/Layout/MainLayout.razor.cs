using System.ComponentModel;
using Microsoft.AspNetCore.Components;

namespace GXOVnT.Components.Layout;

public partial class MainLayout
{


    [Inject] 
    private IServiceProvider ServiceProvider { get; set; } = default!;
    
    
    public MainLayout()
    {
        
    }
    
    protected override void OnInitialized()
    {
        base.OnInitialized();
        
    }

    private void DeviceScannerViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    bool _drawerOpen = true;

    void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }
  
}