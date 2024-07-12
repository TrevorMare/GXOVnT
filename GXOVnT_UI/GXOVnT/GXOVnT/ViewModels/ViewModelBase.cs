using GXOVnT.Services.Interfaces;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.Interfaces;

namespace GXOVnT.ViewModels;

public abstract class ViewModelBase : StateObject
{

    #region Members

    private bool _isBusy;
    private string _busyText = string.Empty;
    protected readonly ILogService LogService;
    
    #endregion

    #region Properties

    public bool IsBusy
    {
        get => _isBusy;
        set => SetField(ref _isBusy, value);
    }
    
    public string BusyText
    {
        get => _busyText;
        set => SetField(ref _busyText, value);
    }

    #endregion

    #region ctor

    protected ViewModelBase(ILogService logService)
    {
        LogService = logService;
    }

    #endregion

    #region Methods

    public void SetStatus(bool isBusy, string busyText = "")
    {
        IsBusy = isBusy;
        BusyText = isBusy ? busyText : "";
    }

    protected void RunTask(Action action, string initialBusyText = "Busy...")
    {
        try
        {
            SetStatus(true, initialBusyText);
            
            action.Invoke();
        }
        catch (Exception ex)
        {
            LogService.LogError($"An unexpected error occured in the application. {ex.Message}");
        }
        finally
        {
            SetStatus(false);
        }
    }
    
    protected async Task RunTaskAsync(Func<Task> action, string initialBusyText = "Busy...")
    {
        try
        {
            SetStatus(true, initialBusyText);
            
            await action.Invoke();
        }
        catch (Exception ex)
        {
            LogService.LogError($"An unexpected error occured in the application. {ex.Message}");
        }
        finally
        {
            SetStatus(false);
        }
    }
    
    protected T? RunTask<T>(Func<T> action, string initialBusyText = "Busy...")
    {
        try
        {
            SetStatus(true, initialBusyText);
            
            return action.Invoke();
        }
        catch (Exception ex)
        {
            LogService.LogError($"An unexpected error occured in the application. {ex.Message}");
            return default;
        }
        finally
        {
            SetStatus(false);
        }
    }
    
    protected async Task<T?> RunTaskAsync<T>(Func<Task<T>> action, string initialBusyText = "Busy...")
    {
        try
        {
            SetStatus(true, initialBusyText);
            
            return await action.Invoke();
        }
        catch (Exception ex)
        {
            LogService.LogError($"An unexpected error occured in the application. {ex.Message}");
            return default;
        }
        finally
        {
            SetStatus(false);
        }
    }
    #endregion

}