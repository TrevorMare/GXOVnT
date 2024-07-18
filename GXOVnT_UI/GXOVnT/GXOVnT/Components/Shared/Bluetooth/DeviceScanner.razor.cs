using System.ComponentModel;
using GXOVnT.Services.ViewModels;
using Microsoft.AspNetCore.Components;

namespace GXOVnT.Components.Shared.Bluetooth;

public partial class DeviceScanner : GXOVnTComponent
{
    
    #region Properties
    /// <summary>
    /// The parameter is to be used in the context when a wizard model passes down a specific view model and should
    /// not be relied on that it will be set. The correct property to use is the standard view model object <see cref="ViewModel"/>
    /// </summary>
    [Parameter]
    public DeviceScannerViewModel? InitialViewModel { get; set; }
    
    [Parameter] 
    public EventCallback<Services.Models.System?> DeviceSelected { get; set; }
    
    /// <summary>
    /// This is a calculated view model that will either be a new view model from the service provider or
    /// the view model parameters
    /// </summary>
    private DeviceScannerViewModel ViewModel =>
        (DeviceScannerViewModel)AttachedViewModelStateObject!;
    
    private bool IsScanningDevices => ViewModel.IsScanningDevices;

    private bool HasItems => ViewModel.DiscoveredDevices.Count > 0;

    private string ScanButtonText => (IsScanningDevices ? "Stop" : "Start") + " scan devices";
   
    #endregion

    #region Methods
    protected override void InitializeViewModel()
    {
        // Set the internal view model object, this will either be from the wizard model or we should
        // initialize it from the service provider
        SetAttachedViewModelStateObject(InitialViewModel);
    }
    #endregion

    #region Event Callbacks
    
    private void OnDeviceListItemClick(Services.Models.System item)
    {
        ViewModel.SetSystemId(item.Id);
    }
    
    private async Task ToggleScanDevices()
    {
        try
        {
            if (ViewModel.IsScanningDevices)
                await ViewModel.StopScanDevicesAsync();
            else
                await ViewModel.StartScanDevicesAsync();
        }
        catch (Exception)
        {
            LogService.LogError("There was an internal error scanning for the devices");
        }
    }

    protected override async Task OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(e.PropertyName) &&
            e.PropertyName.Equals(nameof(DeviceScannerViewModel.SelectedSystem)))
        {
            await DeviceSelected.InvokeAsync(ViewModel.SelectedSystem);
        }
    }

    #endregion

}