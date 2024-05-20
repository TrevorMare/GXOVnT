using System.ComponentModel;
using GXOVnT.Services.ViewModels;
using Microsoft.AspNetCore.Components;

namespace GXOVnT.Components.Shared;

public partial class DeviceScanner : ComponentBase
{

    #region Properties

    [Parameter] public bool Enabled { get; set; } = true;

    [Inject] private VMDeviceScanner VMDeviceScanner { get; set; } = default!;

    
    
    #endregion

    #region Methods

    protected override void OnInitialized()
    {
        base.OnInitialized();
        VMDeviceScanner.PropertyChanged -= VMDeviceScannerOnPropertyChanged;
        VMDeviceScanner.PropertyChanged += VMDeviceScannerOnPropertyChanged;
    }


    #endregion

    #region Event Callbacks

    private void VMDeviceScannerOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }
    #endregion

}