using GXOVnT.ViewModels;

namespace GXOVnT.Components.Shared.Bluetooth;

public partial class CheckBTPermissions : ComponentBaseExtended<CheckBTPermissionsVM>
{

    #region Overrides
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
            await ViewModel.CheckHasBluetoothPermission();
    }

    #endregion
   
    #region Event Callbacks

    private async Task RequestAccessClick()
    {
        
        var accessGranted = await ViewModel.RequestBluetoothPermissions();
        if (!accessGranted)
            await DialogService.ShowMessageBox("Permissions", "The request for Bluetooth permissions was not successful");
    }
    #endregion
    
}