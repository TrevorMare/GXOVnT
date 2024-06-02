using System.ComponentModel;
using GXOVnT.Services.ViewModels;
using Microsoft.AspNetCore.Components;

namespace GXOVnT.Components.Shared;

public partial class LogViewer
{

    #region Properties

    [Inject] private LogViewModel LogViewModel { get; set; }

    private bool HasItems => LogViewModel.LogMessages.Any();

    #endregion

    #region Override

    protected override void OnInitialized()
    {
        base.OnInitialized();
        LogViewModel.PropertyChanged -= LogViewModelOnPropertyChanged;
        LogViewModel.PropertyChanged += LogViewModelOnPropertyChanged;
    }



    #endregion

    #region Event Callbacks

    private async void LogViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    #endregion
    
}