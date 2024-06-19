using System.ComponentModel;
using GXOVnT.Models;
using GXOVnT.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using GXOVnT.Services.Models;
using GXOVnT.Services.ViewModels;
using MudBlazor;

namespace GXOVnT.Components.Pages;

public partial class EnrollDevice : ComponentBase
{

    #region Members

    private GXOVnTBleDevice? _selectedDeviceToEnroll;



    #endregion

    #region Properties

    private EnrollDeviceWizardModel _enrollDeviceWizardModel = new()
    {
        WizardStartStep = 0
    }; 

    [Inject]
    private IDialogService DialogService { get; set; } = default!;

    #endregion

    #region Override Methods


    #endregion

    #region Event Callbacks
    private void OnDeviceSelected(GXOVnTBleDevice? device)
    {
        _selectedDeviceToEnroll = device;
    }
    
    private async Task OnEnrollWizardCompleted()
    {
        
    }

    private async Task OnEnrollWizardCancelled()
    {
        
    }


    private void OnDeviceConfigurationCancelled()
    {
        _selectedDeviceToEnroll = null;
    }
    
    private async void EnrollDeviceSelected(GXOVnTBleDevice obj)
    {
        _selectedDeviceToEnroll = obj;
        
        if (_selectedDeviceToEnroll.SystemConfigured)
        {
            var confirmResult = await DialogService.ShowMessageBox("Confirm",
                "This device is already configured. Are you sure you want to continue, all configuration will be overwritten for this device",
                "Yes", "No");
        
            if (!(confirmResult ?? false))
            {
                _selectedDeviceToEnroll = null;
                return;
            }
        }
    }
    #endregion
    
}