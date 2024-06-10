using System.ComponentModel;
using GXOVnT.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Color = MudBlazor.Color;

namespace GXOVnT.Components.Shared.CommunicationStatus;

public partial class CommunicationStatusIndicator
{

    #region Properties

    [Inject]
    private IBluetoothService BluetoothService { get; set; } = default!;

    private bool BluetoothIsReady => BluetoothService.BluetoothIsReady;

    public string BluetoothConnectionStateIcon { get; set; } = Icons.Material.Outlined.Bluetooth;

    public Color BluetoothConnectionColor { get; set; } = Color.Warning;

    #endregion

    #region Overrides
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        BluetoothService.PropertyChanged -= BluetoothServiceOnPropertyChanged;
        BluetoothService.PropertyChanged += BluetoothServiceOnPropertyChanged;

        await RecalculateView();
    }

    #endregion


    #region Event Callbacks
    
    private async void BluetoothServiceOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        await RecalculateView();
    }
    
    #endregion

    #region Methods

    private async Task RecalculateView()
    {

        BluetoothConnectionStateIcon = BluetoothIsReady
            ? Icons.Material.Outlined.BluetoothConnected
            : Icons.Material.Outlined.Bluetooth;

        BluetoothConnectionColor = BluetoothIsReady
            ? Color.Primary
            : Color.Warning;
        
        
        await InvokeAsync(StateHasChanged);

    }

    #endregion

}