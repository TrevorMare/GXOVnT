using GXOVnT.Services.Interfaces;

namespace GXOVnT.ViewModels;

public class CheckBTPermissionsVM : ViewModelBase
{

    #region Members
    private readonly IRequestPermissionService _requestPermissionService;
    private bool _hasBluetoothPermission;
    private bool _hasCheckedPermissions;
    #endregion

    #region Properties

    public bool HasCheckedPermissions
    {
        get => _hasCheckedPermissions;
        private set => SetField(ref _hasCheckedPermissions, value);
    }
    
    public bool HasBluetoothPermission
    {
        get => _hasBluetoothPermission; 
        private set => SetField(ref _hasBluetoothPermission, value); 
    }

    #endregion
    
    #region ctor

    public CheckBTPermissionsVM(IRequestPermissionService requestPermissionService, 
        ILogService logService) : base(logService)
    {
        _requestPermissionService = requestPermissionService;
    }

    #endregion

    #region Methods

    public async Task CheckHasBluetoothPermission()
    {
        await RunTaskAsync(async () =>
        {
            HasCheckedPermissions = true;
            HasBluetoothPermission = await _requestPermissionService.ApplicationHasBluetoothPermission();
        }, "Checking Bluetooth permissions");
    }
    
    public async Task<bool> RequestBluetoothPermissions()
    {
        return await RunTaskAsync(async () =>
        {
            var permissionGranted = await _requestPermissionService.RequestBluetoothPermission();
            if (!permissionGranted)
                return false;
            await CheckHasBluetoothPermission();
            return _hasBluetoothPermission;
        }, "Requesting Bluetooth permissions");
    }

    #endregion


}