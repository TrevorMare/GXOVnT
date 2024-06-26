﻿using GXOVnT.Services.Interfaces;

namespace GXOVnT.Services.Services;

public class RequestPermissionService : IRequestPermissionService
{

    #region Members

    private readonly ILogService _logService;
    private readonly IAlertService _alertService;

    #endregion

    #region ctor

    public RequestPermissionService(ILogService logService,
        IAlertService alertService)
    {
        _logService = logService;
        _alertService = alertService;
    }

    #endregion

    #region Methods

    public async Task<bool> ApplicationHasBluetoothPermission() => await ApplicationHasPermission<Permissions.Bluetooth>();
    
    public async Task<bool> ApplicationHasPermission<TPermission>() where TPermission : Permissions.BasePermission, new()
    {
        _logService.LogDebug($"RequestPermissionService: Checking if access granted on type {typeof(TPermission)} ");
        var currentStatus = await Permissions.CheckStatusAsync<TPermission>();
        
        if (currentStatus == PermissionStatus.Granted)
        {
            _logService.LogDebug($"RequestPermissionService.ApplicationHasPermission: Permission {typeof(TPermission)} granted");
            return true;
        }

        _logService.LogDebug($"RequestPermissionService.ApplicationHasPermission: Permission {typeof(TPermission)} not granted");
        
        return false;
    }

    public async Task<bool> RequestBluetoothPermission() => await RequestApplicationPermission<Permissions.Bluetooth>();
    
    public async Task<bool> RequestApplicationPermission<TPermission>()
        where TPermission : Permissions.BasePermission, new()
    {

        try
        {
            if (await ApplicationHasPermission<TPermission>())
            {
                _logService.LogDebug($"RequestPermissionService.RequestApplicationPermission: Permission {typeof(TPermission)} already granted");
                return true;
            }
            
        
            if (Permissions.ShouldShowRationale<TPermission>())
            {
                _logService.LogDebug($"RequestPermissionService.RequestApplicationPermission: Showing rationale requesting permission type {typeof(TPermission)}");
                
                var confirmResult = await _alertService.ShowConfirmationAsync("Permission requirements",
                    "The application requires this permission to connect to the devices. You will now be prompted to accept.", "OK",
                    "Cancel");

                if (!confirmResult)
                {
                    _logService.LogDebug($"RequestPermissionService.RequestApplicationPermission: User did not accept the permission");
                    return false;
                }
            }
            
            
            _logService.LogDebug($"RequestPermissionService.RequestApplicationPermission: Starting the request permission task");
            var requestStatus = await Permissions.RequestAsync<Permissions.Bluetooth>();
            
            var result = (requestStatus == PermissionStatus.Granted);
            
            if (result)
                _logService.LogDebug($"RequestPermissionService.RequestApplicationPermission: User granted the permissions");
            else
            {
                _logService.LogDebug($"RequestPermissionService.RequestApplicationPermission: User did not grant the permissions");
                AppInfo.ShowSettingsUI();
            }

            return result;
            
        }
        catch (Exception ex)
        {
            _logService.LogError($"RequestPermissionService.RequestApplicationPermission: An error occured checking permissions. {ex.Message}");
            return false;
        }
    }
    

    #endregion
  
}