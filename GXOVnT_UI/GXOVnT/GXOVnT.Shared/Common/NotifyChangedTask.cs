using GXOVnT.Shared.Interfaces;

namespace GXOVnT.Shared.Common;

public abstract class NotifyChangedTask : NotifyChangedWithBusy, INotifyChangedTask
{

    #region Members

    protected readonly ILogService LogService;

    #endregion
    
    #region ctor

    public NotifyChangedTask(ILogService logService)
    {
        LogService = logService;
    }

    #endregion

    #region Methods

    protected void RunTask(Action action, string initialBusyText = "Busy...")
    {
        try
        {
            SetBusyState(true, initialBusyText);
            
            action.Invoke();
        }
        catch (Exception ex)
        {
            LogService.LogError($"An unexpected error occured in the application. {ex.Message}");
        }
        finally
        {
            SetBusyState(false);
        }
    }
    
    protected async Task RunTaskAsync(Func<Task> action, string initialBusyText = "Busy...")
    {
        try
        {
            SetBusyState(true, initialBusyText);
            
            await action.Invoke();
        }
        catch (Exception ex)
        {
            LogService.LogError($"An unexpected error occured in the application. {ex.Message}");
        }
        finally
        {
            SetBusyState(false);
        }
    }
    
    protected T? RunTask<T>(Func<T> action, string initialBusyText = "Busy...")
    {
        try
        {
            SetBusyState(true, initialBusyText);
            
            return action.Invoke();
        }
        catch (Exception ex)
        {
            LogService.LogError($"An unexpected error occured in the application. {ex.Message}");
            return default;
        }
        finally
        {
            SetBusyState(false);
        }
    }
    
    protected async Task<T?> RunTaskAsync<T>(Func<Task<T>> action, string initialBusyText = "Busy...")
    {
        try
        {
            SetBusyState(true, initialBusyText);
            
            return await action.Invoke();
        }
        catch (Exception ex)
        {
            LogService.LogError($"An unexpected error occured in the application. {ex.Message}");
            return default;
        }
        finally
        {
            SetBusyState(false);
        }
    }
    
    #endregion
    
}