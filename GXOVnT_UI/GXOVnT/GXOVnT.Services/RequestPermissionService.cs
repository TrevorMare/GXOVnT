using GXOVnT.Services.Interfaces;

namespace GXOVnT.Services;

public class RequestPermissionService : IRequestPermissionService
{

    #region Members

    private readonly ViewModels.LogViewModel _logViewModel;
    private readonly IAlertService _alertService;

    #endregion

    #region ctor

    public RequestPermissionService(ViewModels.LogViewModel logViewModel,
        IAlertService alertService)
    {
        _logViewModel = logViewModel;
        _alertService = alertService;
    }

    #endregion

    #region Methods

    public async Task<bool> CheckBLEPermissionRequirement()
    {
        try
        {
            _logViewModel.LogDebug("Checking permissions for Bluetooth");
            
            var currentStatus = await Permissions.CheckStatusAsync<Permissions.Bluetooth>();
            if (currentStatus == PermissionStatus.Granted)
            {
                _logViewModel.LogDebug("BLE permission granted");
                return true;
            }

            if (Permissions.ShouldShowRationale<Permissions.Bluetooth>())
            {
                _logViewModel.LogInformation("Querying user to continue with permissions");
                
                var confirmResult = await _alertService.ShowConfirmationAsync("BLE Requirements",
                    "The application requires BLE permissions to connect to the devices. You will now be prompted to accept.", "OK",
                    "Cancel");

                if (!confirmResult)
                {
                    _logViewModel.LogWarning("User did not want to continue with permissions");
                    return false;
                }
            }

            _logViewModel.LogInformation("Requesting BLE permissions");
            var requestStatus = await Permissions.RequestAsync<Permissions.Bluetooth>();
            
            var result = (requestStatus == PermissionStatus.Granted);
            
            if (result)
                _logViewModel.LogInformation("User granted BLE Permissions");
            else
            {
                _logViewModel.LogWarning("User did not grant BLE Permissions");
                AppInfo.ShowSettingsUI();
            }

            return result;
        }
        catch (Exception ex)
        {
            _logViewModel.LogError($"An error occured checking permissions. {ex.Message}");
            return false;
        }
    }

    #endregion
    
  
}