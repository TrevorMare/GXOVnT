using GXOVnT.Services.Interfaces;
using GXOVnT.ViewModels.Wizards;

namespace GXOVnT.ViewModels;

public class CheckBTPermissionsVM : WizardStepModel
{

    #region Members

    private readonly IRequestPermissionService _requestPermissionService;
    private bool _hasBluetoothPermission; 
    #endregion

    #region Properties

    public bool HasBluetoothPermission
    {
        get => _hasBluetoothPermission; 
        private set => SetField(ref _hasBluetoothPermission, value); 
    }

    #endregion
    
    #region ctor

    public CheckBTPermissionsVM(IRequestPermissionService requestPermissionService)
    {
        _requestPermissionService = requestPermissionService;
    }

    #endregion

    #region Methods

    public async Task CheckHasBluetoothPermission()
    {
        try
        {
            SetIsBusy(true, "Checking Bluetooth permissions");
            HasBluetoothPermission = await _requestPermissionService.ApplicationHasBluetoothPermission();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
        finally
        {
            SetIsBusy(false);   
        }
    }

    #endregion


}