using System.ComponentModel;
using GXOVnT.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace GXOVnT.Components.Shared.Logs;

public partial class LogViewer
{

    #region Properties

    [Inject] 
    private ILogService LogService { get; set; }

    private bool HasItems => LogService.LogMessages.Any();

    #endregion

    #region Override

    protected override void OnInitialized()
    {
        base.OnInitialized();
        LogService.PropertyChanged -= LogViewModelOnPropertyChanged;
        LogService.PropertyChanged += LogViewModelOnPropertyChanged;
    }
    #endregion

    #region Event Callbacks

    private async void LogViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    #endregion
    
}