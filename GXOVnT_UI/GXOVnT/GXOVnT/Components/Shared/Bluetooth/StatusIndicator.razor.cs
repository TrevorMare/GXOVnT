using GXOVnT.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace GXOVnT.Components.Shared.Bluetooth;

public partial class StatusIndicator : ComponentBase
{

    #region Properties

    [Inject]
    private IBluetoothService BluetoothService { get; set; } = default!;

    [Inject]
    private IMessageOrchestrator MessageOrchestrator { get; set; } = default!;

    #endregion

    #region Overrides

    

    #endregion
    



}