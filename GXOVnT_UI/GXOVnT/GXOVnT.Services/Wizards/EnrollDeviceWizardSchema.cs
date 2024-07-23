using System.ComponentModel;
using GXOVnT.Services.ViewModels;
using GXOVnT.Shared.Common;

namespace GXOVnT.Services.Wizards;

public class EnrollDeviceWizardSchema : WizardSchema
{
    
    #region Properties

    public DeviceInfoViewModel DeviceInfoViewModel { get; }
    
    public CheckBluetoothPermissionsViewModel CheckBluetoothPermissionsViewModel { get; }
    
    public DeviceScannerViewModel DeviceScannerViewModel { get; }

    public DeviceEnrollConfigurationViewModel DeviceEnrollConfigurationViewModel { get; } 
    #endregion

    #region ctor

    public EnrollDeviceWizardSchema()
    {
        CheckBluetoothPermissionsViewModel = Services.GetRequiredService<CheckBluetoothPermissionsViewModel>();
        DeviceInfoViewModel = Services.GetRequiredService<DeviceInfoViewModel>();
        DeviceScannerViewModel = Services.GetRequiredService<DeviceScannerViewModel>();
        DeviceEnrollConfigurationViewModel = Services.GetRequiredService<DeviceEnrollConfigurationViewModel>();
        
        // Attach the busy state changed event handlers
        CheckBluetoothPermissionsViewModel.OnBusyStateChanged += ViewModelOnBusyStateChanged;
        DeviceInfoViewModel.OnBusyStateChanged += ViewModelOnBusyStateChanged;
        DeviceScannerViewModel.OnBusyStateChanged += ViewModelOnBusyStateChanged;
        DeviceEnrollConfigurationViewModel.OnBusyStateChanged += ViewModelOnBusyStateChanged;
        
        CheckBluetoothPermissionsViewModel.PropertyChanged += CheckBluetoothPermissionsViewModelOnPropertyChanged;
        DeviceScannerViewModel.PropertyChanged += DeviceScannerViewModelOnPropertyChanged;
        DeviceInfoViewModel.PropertyChanged += DeviceInfoViewModelOnPropertyChanged;
        DeviceEnrollConfigurationViewModel.PropertyChanged += DeviceEnrollConfigurationViewModelOnPropertyChanged;
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
        else if (CurrentStep.WizardSchemaStepType.Equals(WizardSchemaStepType.ConfirmDeviceInformation))
        {
            await GoToDeviceConfigurationStep();
        }
        else if (CurrentStep.WizardSchemaStepType.Equals(WizardSchemaStepType.SetDeviceConfiguration))
        {
            await GoToScanDevicesState();
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
        else if (CurrentStep.WizardSchemaStepType.Equals(WizardSchemaStepType.ScanBluetoothSystems))
        {
            await GoToDevicesInfoState();
        }
        else if (CurrentStep.WizardSchemaStepType.Equals(WizardSchemaStepType.ConfirmDeviceInformation))
        {
            await GoToDeviceConfigurationStep();
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
        
        // If we already got the permissions, no need to request again
        if (!CheckBluetoothPermissionsViewModel.HasBluetoothPermission)
        {
            await CheckBluetoothPermissionsViewModel.CheckHasBluetoothPermission();    
        }
        
        await UpdateCheckBluetoothPermissionStep();
        
        // If we are going forward, and we already have the permission, we don't have to wait for the next click.
        if (CheckBluetoothPermissionsViewModel.HasBluetoothPermission && skipIfAlreadyHasPermissions && StepDirection.Equals(WizardStepDirection.Next))
            await NextStep();
    }

    private Task UpdateCheckBluetoothPermissionStep()
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

        await UpdateScanDevicesStep();
    }

    private Task UpdateScanDevicesStep()
    {
        CurrentStep!.IsBackButtonVisible = true;
        CurrentStep.IsBackButtonEnabled = true;
        CurrentStep.IsCancelButtonEnabled = true;
        CurrentStep.IsNextButtonEnabled = DeviceScannerViewModel.SelectedSystemId != null;
        return Task.CompletedTask;
    }

    #endregion

    #region Device Info View Model

    private async Task GoToDevicesInfoState()
    {
        SetStepAsCurrent(WizardSchemaStepType.ConfirmDeviceInformation);
        
        if (!CurrentStepIsType(WizardSchemaStepType.ConfirmDeviceInformation))
            return;

        // We need to set the device of this view model to the previously selected device
        await DeviceInfoViewModel.GetDeviceInfo(DeviceScannerViewModel.SelectedSystemId);

        // If the device info could be found, we should check if the system has been previously initialized or configured
        if (DeviceInfoViewModel is { SelectedSystemId: not null, DeviceInfo: not null })
        {
            var shouldConfirm = DeviceInfoViewModel.IsSystemConfigured || DeviceInfoViewModel.IsSystemInitialized;
            // No need to confirm, just go to the next step
            if (!shouldConfirm)
                await GoToNextStep();
        }

        await UpdateDevicesInfoStep();
    }

    private Task UpdateDevicesInfoStep()
    {
        CurrentStep!.IsBackButtonVisible = true;
        CurrentStep.IsBackButtonEnabled = true;
        CurrentStep.IsCancelButtonEnabled = true;
        if (DeviceInfoViewModel is { SelectedSystemId: not null, DeviceInfo: not null })
        {
            var shouldConfirm = DeviceInfoViewModel.IsSystemConfigured || DeviceInfoViewModel.IsSystemInitialized;
            var hasConfirmed = DeviceInfoViewModel.DeviceOverwriteConfirmed;
            
            CurrentStep.IsNextButtonEnabled = !shouldConfirm || (shouldConfirm && hasConfirmed);
        }
        else
            CurrentStep.IsNextButtonEnabled = false;
        
        return Task.CompletedTask;
    }

    #endregion
    
    #region Device Info View Model

    private async Task GoToDeviceConfigurationStep()
    {
        SetStepAsCurrent(WizardSchemaStepType.SetDeviceConfiguration);
        
        if (!CurrentStepIsType(WizardSchemaStepType.SetDeviceConfiguration))
            return;

        // We need to set the device of this view model to the previously selected device
        await DeviceEnrollConfigurationViewModel.LoadDeviceConfiguration(DeviceScannerViewModel.SelectedSystemId);

        await UpdateDeviceConfigurationStep();
    }

    private Task UpdateDeviceConfigurationStep()
    {
        CurrentStep!.IsBackButtonVisible = true;
        CurrentStep.IsBackButtonEnabled = true;
        CurrentStep.IsCancelButtonEnabled = true;
        CurrentStep.IsNextButtonEnabled = DeviceEnrollConfigurationViewModel.IsValid;
        
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
            UpdateCheckBluetoothPermissionStep();
        }
    }

    private void DeviceScannerViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Validate the property name
        if (string.IsNullOrWhiteSpace(e.PropertyName)) return;
        // Validate the current step
        if (!CurrentStepIsType(WizardSchemaStepType.ScanBluetoothSystems)) return;
        
        // If it's either the selected system id or the all property that changed, update the current state
        if (e.PropertyName.Equals(nameof(DeviceScannerViewModel.SelectedSystemId)) ||
            e.PropertyName.Equals("*"))
        {
            DeviceInfoViewModel.SetSystemId(DeviceScannerViewModel.SelectedSystemId);
            UpdateScanDevicesStep();
        }
    }


    private void DeviceInfoViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Validate the property name
        if (string.IsNullOrWhiteSpace(e.PropertyName)) return;
        // Validate the current step
        if (!CurrentStepIsType(WizardSchemaStepType.ConfirmDeviceInformation)) return;
        UpdateDevicesInfoStep();
    }

    private void DeviceEnrollConfigurationViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Validate the property name
        if (string.IsNullOrWhiteSpace(e.PropertyName)) return;
        // Validate the current step
        if (!CurrentStepIsType(WizardSchemaStepType.SetDeviceConfiguration)) return;
        UpdateDeviceConfigurationStep();
    }
    #endregion
   
}