using GXOVnT.Services.ViewModels;

namespace GXOVnT.ViewModels.Wizards;

public class EnrollDeviceWizardModel : WizardComponentModel
{

    #region Members

    public CheckBluetoothPermissionsViewModel CheckBluetoothPermissionsViewModel { get ; private set; }

    #endregion

    #region ctor

    public EnrollDeviceWizardModel()
    {
        
    }

    #endregion
    
    
}