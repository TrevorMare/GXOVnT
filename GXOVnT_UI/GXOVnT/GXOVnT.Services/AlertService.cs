using GXOVnT.Services.Interfaces;

namespace GXOVnT.Services;

public class AlertService : IAlertService
{

    #region ctor

    public AlertService()
    {
    }

    #endregion

    #region Methods

    public Task ShowAlertAsync(string title, string message, string cancel = "OK")
    {
        return Application.Current?.MainPage == null ? 
            Task.CompletedTask : 
            Application.Current.MainPage.DisplayAlert(title, message, cancel);
    }

    public Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Yes", string cancel = "No")
    {
        return Application.Current?.MainPage == null ?
            Task.FromResult(false):
            Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
    }

    public void ShowAlert(string title, string message, string cancel = "OK")
    {
        
        Application.Current?.MainPage?.Dispatcher.Dispatch(async () =>
            await ShowAlertAsync(title, message, cancel)
        );
    }

    public void ShowConfirmation(string title, string message, Action<bool> callback,
        string accept = "Yes", string cancel = "No")
    {
        Application.Current?.MainPage?.Dispatcher.Dispatch(async () =>
        {
            var result = await ShowConfirmationAsync(title, message, accept, cancel);
            callback(result);
        });
    }

    #endregion
    
}