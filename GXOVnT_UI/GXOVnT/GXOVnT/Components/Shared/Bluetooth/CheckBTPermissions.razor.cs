using GXOVnT.Models;
using GXOVnT.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GXOVnT.Components.Shared.Bluetooth;

public partial class CheckBTPermissions : ComponentBase
{

    #region Members

    private bool _isBusy;

    #endregion
    
    #region Properties
    
    
    [Inject]
    private IDialogService DialogService { get; set; } = default!;
    
    [Inject]
    private IRequestPermissionService RequestPermissionService { get; set; } = default!;

    [Inject]
    private ILogService LogService { get; set; } = default!;

    [CascadingParameter]
    private WizardStepModel? WizardStepModel { get; set; }

    private bool IsBusy
    {
        get => WizardStepModel?.IsBusy ?? _isBusy;
        set
        {
            if (WizardStepModel != null)
                WizardStepModel.IsBusy = value;
            _isBusy = value;
        } 
    }
    
    private bool HasBlueToothPermission { get; set; }
    
    #endregion


    #region Overrides

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (WizardStepModel != null)
            WizardStepModel.ForwardEnabled = false;
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

            if (WizardStepModel != null)
                WizardStepModel.ForwardEnabled = HasBlueToothPermission;
            
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
            await InvokeAsync(StateHasChanged);

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