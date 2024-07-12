using System.ComponentModel;

namespace GXOVnT.Shared.Interfaces;

public struct BusyStateChangedArgs
{
    public bool IsBusy { get; init; }
    public string? BusyStatus { get; init; }
}

public delegate void OnStateBusyChangedHandler(object sender, BusyStateChangedArgs args);

public interface IStateObject : INotifyPropertyChanged
{
    event OnStateBusyChangedHandler? OnBusyStateChanged;  
    
    bool IsBusy { get; }
    
    string? BusyStatus { get; }

    void SetBusyState(bool isBusy, string? busyStatus = null);
}