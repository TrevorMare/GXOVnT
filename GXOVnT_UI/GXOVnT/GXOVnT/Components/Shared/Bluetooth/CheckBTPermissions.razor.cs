using System.ComponentModel;
using GXOVnT.Services.ViewModels;
using GXOVnT.Shared;
using Microsoft.AspNetCore.Components;

namespace GXOVnT.Components.Shared.Bluetooth;

public partial class CheckBTPermissions : GXOVnTComponent
{

    #region Properties
    /// <summary>
    /// The parameter is to be used in the context when a wizard model passes down a specific view model and should
    /// not be relied on that it will be set. The correct property to use is the standard view model object <see cref="ViewModel"/>
    /// </summary>
    [Parameter]
    public CheckBluetoothPermissionsViewModel? InitialViewModel { get; set; }

    /// <summary>
    /// This is a calculated view model that will either be a new view model from the service provider or
    /// the view model parameters
    /// </summary>
    protected CheckBluetoothPermissionsViewModel ViewModel =>
        (CheckBluetoothPermissionsViewModel)AttachedViewModelStateObject!;
    
    #endregion

    #region Overrides

    protected override void InitializeViewModel()
    {
        // Set the internal view model object, this will either be from the wizard model or we should
        // initialize it from the service provider
        SetAttachedViewModelStateObject(InitialViewModel);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await ViewModel.CheckHasBluetoothPermission();
    }
    #endregion
   
    #region Event Callbacks

    private async Task RequestAccessClick()
    {
        await ViewModel.RequestBluetoothPermissions();
    }
    #endregion
    
}