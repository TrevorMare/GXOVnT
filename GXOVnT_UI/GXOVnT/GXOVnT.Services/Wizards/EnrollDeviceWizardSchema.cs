using GXOVnT.Services.ViewModels;
using GXOVnT.Shared.Common;

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
        DeviceInfoViewModel = Services.GetRequiredService<DeviceInfoViewModel>();
        CheckBluetoothPermissionsViewModel = Services.GetRequiredService<CheckBluetoothPermissionsViewModel>();
        DeviceScannerViewModel = Services.GetRequiredService<DeviceScannerViewModel>();
    }

    #endregion

}