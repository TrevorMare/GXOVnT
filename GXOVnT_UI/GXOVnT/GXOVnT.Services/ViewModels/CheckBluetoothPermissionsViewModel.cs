﻿using GXOVnT.Services.Interfaces;
using GXOVnT.Shared.Common;

namespace GXOVnT.Services.ViewModels;

public class CheckBluetoothPermissionsViewModel : StateObject
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

    public CheckBluetoothPermissionsViewModel()
    {
        _requestPermissionService = Services.GetRequiredService<IRequestPermissionService>();
    }

    #endregion

    #region Methods
    public async Task<bool> CheckHasBluetoothPermission()
    {
        return await RunTaskAsync(async () =>
        {
            HasCheckedPermissions = true;
            HasBluetoothPermission = await _requestPermissionService.ApplicationHasBluetoothPermission();
            return HasBluetoothPermission;
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