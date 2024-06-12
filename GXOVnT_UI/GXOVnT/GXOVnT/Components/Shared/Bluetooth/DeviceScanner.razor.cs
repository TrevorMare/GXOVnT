using System.ComponentModel;
using GXOVnT.Models;
using GXOVnT.Services.Models;
using GXOVnT.Services.ViewModels;
using Microsoft.AspNetCore.Components;

namespace GXOVnT.Components.Shared.Bluetooth;

public partial class DeviceScanner : ComponentBase
{

    #region Members

    private bool _isBusy;

    #endregion
    
    #region Properties
    [CascadingParameter]
    private WizardStepModel? WizardStepModel { get; set; }
    
    private bool IsBusy
    {
        get => WizardStepModel?.IsBusy ?? _isBusy;
        set
        {
            if (WizardStepModel != null)
                WizardStepModel.IsBusy = value;
            _isBusy = value;
        } 
    }
    
    [Parameter] 
    public bool Enabled { get; set; } = true;
    
    [Parameter] 
    public EventCallback<GXOVnTDevice> DeviceSelected { get; set; }

    [Inject]
    public DeviceScannerViewModel DeviceScannerViewModel { get; set; } = default!;

    private bool IsScanningDevices => DeviceScannerViewModel.IsScanningDevices;

    private bool HasItems => DeviceScannerViewModel.ScannedDevices.Count > 0;

    private string ScanButtonText => (IsScanningDevices ? "Stop" : "Start") + " scan devices";
   
    #endregion

    #region Methods

    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        
        if (WizardStepModel != null)
            WizardStepModel.ForwardEnabled = false;
        
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
        await DeviceScannerViewModel.ConnectToDevice(item, true);
    }
    
    public async Task ToggleScanDevices()
    {
        try
        {
            IsBusy = true;

            if (DeviceScannerViewModel.IsScanningDevices)
                await DeviceScannerViewModel.StopScanGXOVnTDevicesAsync();
            else
                await DeviceScannerViewModel.StartScanGXOVnTDevicesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
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