using System.ComponentModel;
using System.Runtime.CompilerServices;
using GXOVnT.Shared.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GXOVnT.Shared.Common;

public class StateObject : IStateObject
{
    
    #region Members

    private bool _isBusy;
    private string? _busyStatus;
    protected readonly ILogService LogService;
    
    #endregion
    
    #region Properties
    public event OnStateBusyChangedHandler? OnBusyStateChanged; 
    
    public event PropertyChangedEventHandler? PropertyChanged;

    public IServiceProvider Services => AppService.ServiceProvider;
    
    public bool IsBusy
    {
        get => _isBusy;
        private set => SetField(ref _isBusy, value);
    }

    public string? BusyStatus
    {
        get => _busyStatus;
        private set => SetField(ref _busyStatus, value);
    }
    #endregion

    #region ctor

    public StateObject()
    {
        LogService = AppService.ServiceProvider.GetRequiredService<ILogService>();
    }

    #endregion
    
    #region Public Methods
    protected virtual void OnStateChanged()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("*"));
    }
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    public void SetBusyState(bool isBusy, string? busyStatus = null)
    {
        // Set the fields and if there is a change in either one of the values, we need to raise an event
        var changesMade = SetField(ref _isBusy, isBusy, nameof(IsBusy));
        changesMade |= SetField(ref _busyStatus, isBusy ? busyStatus : null, nameof(BusyStatus));
        
        if (changesMade)
            OnBusyStateChanged?.Invoke(this, new BusyStateChangedArgs() { BusyStatus = _busyStatus, IsBusy = _isBusy });
    }
    #endregion

    #region Protected Methods

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