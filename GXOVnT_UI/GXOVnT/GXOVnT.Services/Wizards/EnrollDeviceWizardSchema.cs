using System.ComponentModel;
using GXOVnT.Services.ViewModels;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.Interfaces;

namespace GXOVnT.Services.Wizards;

public class EnrollDeviceWizardSchema : WizardSchema
{
    
    #region Properties

    public DeviceInfoViewModel DeviceInfoViewModel { get; }
    
    public CheckBluetoothPermissionsViewModel CheckBluetoothPermissionsViewModel { get; }
    
    public DeviceScannerViewModel DeviceScannerViewModel { get; }

    #endregion

    #region ctor

    public EnrollDeviceWizardSchema()
    {
        CheckBluetoothPermissionsViewModel = Services.GetRequiredService<CheckBluetoothPermissionsViewModel>();
        DeviceInfoViewModel = Services.GetRequiredService<DeviceInfoViewModel>();
        DeviceScannerViewModel = Services.GetRequiredService<DeviceScannerViewModel>();
        
        // Attach the busy state changed event handlers
        CheckBluetoothPermissionsViewModel.OnBusyStateChanged += ViewModelOnBusyStateChanged;
        DeviceInfoViewModel.OnBusyStateChanged += ViewModelOnBusyStateChanged;
        DeviceScannerViewModel.OnBusyStateChanged += ViewModelOnBusyStateChanged;
        
        CheckBluetoothPermissionsViewModel.PropertyChanged += CheckBluetoothPermissionsViewModelOnPropertyChanged;
    }



    #endregion

    #region Methods
    
    protected override void OnWizardStepAdded(WizardSchemaStep wizardStepWizardSchemaStep)
    {
 
        var wizardStepType = wizardStepWizardSchemaStep.WizardSchemaStepType;
        if (wizardStepType == null) return;
        
        // The first step is the enroll device startup information
        if (wizardStepType.Equals(WizardSchemaStepType.EnrollDeviceStartupInformation))
        {
            GoToEnrollDeviceStartupInformationState();
        }
    }

    #endregion

    #region Navigation Steps

    protected override async Task PreviousStep()
    {
        
        if (CurrentStep?.WizardSchemaStepType == null) return;
        
        if (CurrentStep.WizardSchemaStepType.Equals(WizardSchemaStepType.CheckBluetoothPermissions))
        {
            await GoToEnrollDeviceStartupInformationState();
        }
        else if (CurrentStep.WizardSchemaStepType.Equals(WizardSchemaStepType.ScanBluetoothSystems))
        {
            await GoToCheckBluetoothPermissionState(false);
        }
    }

    protected override async Task NextStep()
    {
        if (CurrentStep?.WizardSchemaStepType == null) return;
        
        if (CurrentStep.WizardSchemaStepType.Equals(WizardSchemaStepType.EnrollDeviceStartupInformation))
        {
            await GoToCheckBluetoothPermissionState(true);
        }
        else if (CurrentStep.WizardSchemaStepType.Equals(WizardSchemaStepType.CheckBluetoothPermissions))
        {
            await GoToScanDevicesState();
        }
        
    }

    #endregion

    #region Enroll Device Startup Information State

    private Task GoToEnrollDeviceStartupInformationState()
    {
        SetStepAsCurrent(WizardSchemaStepType.EnrollDeviceStartupInformation);
        
        if (!CurrentStepIsType(WizardSchemaStepType.EnrollDeviceStartupInformation))
            return Task.CompletedTask;
        
        CurrentStep!.IsBackButtonVisible = false;
        CurrentStep.IsNextButtonEnabled = true;
        CurrentStep.IsCancelButtonEnabled = true;
        
        return Task.CompletedTask;
    }
    #endregion

    #region Check Bluetooth Permission View Model

    private async Task GoToCheckBluetoothPermissionState(bool skipIfAlreadyHasPermissions)
    {
        SetStepAsCurrent(WizardSchemaStepType.CheckBluetoothPermissions);
        
        if (!CurrentStepIsType(WizardSchemaStepType.CheckBluetoothPermissions))
            return;
        
        var hasBluetoothPermissions = await CheckBluetoothPermissionsViewModel.CheckHasBluetoothPermission();

        await UpdateGoToCheckBluetoothPermissionState();
        
        // If we are going forward, and we already have the permission, we don't have to wait for the next click.
        if (hasBluetoothPermissions && skipIfAlreadyHasPermissions && StepDirection.Equals(WizardStepDirection.Next))
            await NextStep();
    }

    private Task UpdateGoToCheckBluetoothPermissionState()
    {
        CurrentStep!.IsBackButtonVisible = true;
        CurrentStep.IsBackButtonEnabled = true;
        CurrentStep.IsCancelButtonEnabled = true;
        CurrentStep.IsNextButtonEnabled = CheckBluetoothPermissionsViewModel.HasBluetoothPermission;
        return Task.CompletedTask;
    }

    #endregion
    
    #region Scan Devices View Model

    private async Task GoToScanDevicesState()
    {
        SetStepAsCurrent(WizardSchemaStepType.ScanBluetoothSystems);
        
        if (!CurrentStepIsType(WizardSchemaStepType.ScanBluetoothSystems))
            return;

        await UpdateScanDevicesState();
    }

    private Task UpdateScanDevicesState()
    {
        CurrentStep!.IsBackButtonVisible = true;
        CurrentStep.IsBackButtonEnabled = true;
        CurrentStep.IsCancelButtonEnabled = true;
        CurrentStep.IsNextButtonEnabled = DeviceScannerViewModel.SelectedSystem != null;
        return Task.CompletedTask;
    }

    #endregion
  
    #region Event Callbacks

    private void CheckBluetoothPermissionsViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Validate the property name
        if (string.IsNullOrWhiteSpace(e.PropertyName)) return;
        // Validate the current step
        if (!CurrentStepIsType(WizardSchemaStepType.CheckBluetoothPermissions)) return;

        // If it's either the has bluetooth permissions or the all property that changed, update the current state
        if (e.PropertyName.Equals(nameof(CheckBluetoothPermissionsViewModel.HasBluetoothPermission)) ||
            e.PropertyName.Equals("*"))
        {
            UpdateGoToCheckBluetoothPermissionState();
        }
    }


    #endregion
    
   
}