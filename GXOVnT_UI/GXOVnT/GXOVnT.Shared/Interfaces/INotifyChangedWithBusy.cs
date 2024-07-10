using GXOVnT.Shared.Common;

namespace GXOVnT.Shared.Interfaces;

public interface INotifyChangedWithBusy : INotifyChanged
{
    event NotifyChangedWithBusy.OnBusyStateChangedHandler? OnBusyStateChanged; 
    
    bool IsBusy { get; }
    
    string? BusyStatus { get; }

    void SetBusyState(bool isBusy, string? busyStatus = null);

}