using GXOVnT.Services.Common;
using GXOVnT.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using GXOVnT.Services.Models;
using MudBlazor;

namespace GXOVnT.Components.Pages;

public partial class EnrollDevice : ComponentBase
{

    #region Members

    private GXOVnTDevice? _selectedDeviceToEnroll;
    

    #endregion

    #region Properties

    [Inject]private IDialogService DialogService { get; set; } = default!;

    #endregion
    
    
   
    #region ctor

    public EnrollDevice()
    {
       
    }

    #endregion

    #region Event Callbacks

    private void OnDeviceConfigurationCancelled()
    {
        _selectedDeviceToEnroll = null;
    }
    
    private async void EnrollDeviceSelected(GXOVnTDevice obj)
    {
        // _selectedDeviceToEnroll = obj;
        //
        // if (_selectedDeviceToEnroll.SystemConfigured)
        // {
        //     var confirmResult = await DialogService.ShowMessageBox("Confirm",
        //         "This device is already configured. Are you sure you want to continue, all configuration will be overwritten for this device",
        //         "Yes", "No");
        //
        //     if (!(confirmResult ?? false))
        //     {
        //         _selectedDeviceToEnroll = null;
        //         return;
        //     }
        // }
        
        
        
        
        
        
        
    }
    #endregion
    
}