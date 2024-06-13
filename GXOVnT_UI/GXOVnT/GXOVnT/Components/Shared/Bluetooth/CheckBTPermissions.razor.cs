using GXOVnT.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace GXOVnT.Components.Shared.Bluetooth;

public partial class CheckBTPermissions : GXOVnTComponent
{
    
    #region Properties
    
    [Inject]
    private IRequestPermissionService RequestPermissionService { get; set; } = default!;
    
    private bool HasBlueToothPermission { get; set; }
    
    #endregion


    #region Overrides

    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetWizardForwardEnabled(false);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
            await CheckIfApplicationHasBluetoothServiceAccess();
    }

    #endregion

    #region Methods

    private async Task CheckIfApplicationHasBluetoothServiceAccess()
    {
        try
        {
            IsBusy = true;

            HasBlueToothPermission = await RequestPermissionService.ApplicationHasBluetoothPermission();

            SetWizardForwardEnabled(HasBlueToothPermission);

            await InvokeAsync(StateHasChanged);
        }
        catch (Exception)
        {
            LogService.LogError("There was an internal error checking the Bluetooth permissions");
        }
        finally
        {
            IsBusy = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    #endregion

    #region Event Callbacks

    private async Task RequestAccessClick()
    {
        try
        {
            IsBusy = true;

            var permissionGranted = await RequestPermissionService.RequestBluetoothPermission();
            if (!permissionGranted)
            {
                await DialogService.ShowMessageBox("Permissions", "The request for Bluetooth permissions was not successful");
                return;
            }

            await CheckIfApplicationHasBluetoothServiceAccess();

        }
        catch (Exception)
        {
            LogService.LogError("There was an internal error requesting the Bluetooth permissions");
        }
        finally
        {
            IsBusy = false;
            await InvokeAsync(StateHasChanged);
        }
    }
    #endregion
    
}