using GXOVnT.Services.Interfaces;
using GXOVnT.Shared.Common;

namespace GXOVnT.Services.Services;

public class RequestPermissionService : StateObject, IRequestPermissionService
{

    #region Members
    private readonly IAlertService _alertService;
    #endregion

    #region ctor

    public RequestPermissionService(IAlertService alertService)
    {
        _alertService = alertService;
    }

    #endregion

    #region Methods
    public async Task<bool> ApplicationHasNetworkStatePermission() => await ApplicationHasPermission<Permissions.NetworkState>();
    
    public async Task<bool> ApplicationHasNearbyWiFiPermission() => await ApplicationHasPermission<Permissions.NearbyWifiDevices>();
    
    public async Task<bool> ApplicationHasBluetoothPermission() => await ApplicationHasPermission<Permissions.Bluetooth>();
    
    public async Task<bool> ApplicationHasPermission<TPermission>() where TPermission : Permissions.BasePermission, new()
    {

        return await RunTaskAsync(async () =>
        {
            LogService.LogDebug($"RequestPermissionService: Checking if access granted on type {typeof(TPermission)} ");
            var currentStatus = await Permissions.CheckStatusAsync<TPermission>();
        
            if (currentStatus == PermissionStatus.Granted)
            {
                LogService.LogDebug($"RequestPermissionService.ApplicationHasPermission: Permission {typeof(TPermission)} granted");
                return true;
            }

            LogService.LogDebug($"RequestPermissionService.ApplicationHasPermission: Permission {typeof(TPermission)} not granted");
        
            return false;
        }, "Checking application permissions");
    }

    public async Task<bool> RequestBluetoothPermission() => await RequestApplicationPermission<Permissions.Bluetooth>();
    
    public async Task<bool> RequestNearbyWiFiPermission() => await RequestApplicationPermission<Permissions.NearbyWifiDevices>();
    
    public async Task<bool> RequestNetworkStatePermission() => await RequestApplicationPermission<Permissions.NetworkState>();
    
    public async Task<bool> RequestApplicationPermission<TPermission>()
        where TPermission : Permissions.BasePermission, new()
    {
        return await RunTaskAsync(async () =>
        {
            try
            {
                if (await ApplicationHasPermission<TPermission>())
                {
                    LogService.LogDebug($"RequestPermissionService.RequestApplicationPermission: Permission {typeof(TPermission)} already granted");
                    return true;
                }

                if (Permissions.ShouldShowRationale<TPermission>())
                {
                    LogService.LogDebug($"RequestPermissionService.RequestApplicationPermission: Showing rationale requesting permission type {typeof(TPermission)}");
                    
                    var confirmResult = await _alertService.ShowConfirmationAsync("Permission requirements",
                        "The application requires this permission to connect to the devices. You will now be prompted to accept.", "OK",
                        "Cancel");

                    if (!confirmResult)
                    {
                        LogService.LogDebug($"RequestPermissionService.RequestApplicationPermission: User did not accept the permission");
                        return false;
                    }
                }
                
                
                LogService.LogDebug($"RequestPermissionService.RequestApplicationPermission: Starting the request permission task");
                var requestStatus = await Permissions.RequestAsync<Permissions.Bluetooth>();
                
                var result = (requestStatus == PermissionStatus.Granted);
                
                if (result)
                    LogService.LogDebug($"RequestPermissionService.RequestApplicationPermission: User granted the permissions");
                else
                {
                    LogService.LogDebug($"RequestPermissionService.RequestApplicationPermission: User did not grant the permissions");
                    AppInfo.ShowSettingsUI();
                }

                return result;
                
            }
            catch (Exception ex)
            {
                LogService.LogError($"RequestPermissionService.RequestApplicationPermission: An error occured checking permissions. {ex.Message}");
                return false;
            }
        }, "Requesting application permission");
        
    }
    

    #endregion
  
}