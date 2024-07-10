using System.ComponentModel;
using GXOVnT.Services.Interfaces;
using GXOVnT.ViewModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GXOVnT.Components.Shared;

public class ComponentBaseExtended<TViewModel> : ComponentBase  where TViewModel : ViewModelBase
{

    #region Properties
    /// <summary>
    /// An instance of the default view model, this is to be used if the parameter is not passed
    /// </summary>
    [Inject]
    private TViewModel DefaultViewModel { get; set; } = default!;

    /// <summary>
    /// Dialog service to provide dialogs
    /// </summary>
    [Inject]
    protected IDialogService DialogService { get; set; } = default!;
    
    /// <summary>
    /// Log service to capture all logs and display on screen
    /// </summary>
    [Inject]
    protected ILogService LogService { get; set; } = default!;

    /// <summary>
    /// The passed down view model. Views should not use this
    /// as the source of the display but rather the view model property. The view model
    /// property will return either a new view model or this passed down data model
    /// </summary>
    [Parameter]
    public TViewModel? DataModel { get; set; }

    /// <summary>
    /// The Model to use for binding the view information
    /// </summary>
    protected TViewModel ViewModel { get; private set; } = default!;

    /// <summary>
    /// Returns a proxy value if the view model is busy 
    /// </summary>
    public bool IsBusy => ViewModel.IsBusy;

    /// <summary>
    /// Returns a proxy busy description value 
    /// </summary>
    public string IsBusyText => ViewModel.BusyText;
    #endregion

    #region Override

    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        if (ViewModel != null)
            ViewModel.PropertyChanged -= ViewModelOnPropertyChanged; 
        
        ViewModel = DataModel ?? DefaultViewModel;
        ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        
    }
    #endregion

    #region Event Callbacks

    private async void ViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }    

    #endregion
    
}