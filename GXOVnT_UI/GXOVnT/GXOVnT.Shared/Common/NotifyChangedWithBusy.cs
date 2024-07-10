namespace GXOVnT.Shared.Common;

public class NotifyChangedWithBusy : NotifyChanged
{

    #region Events

    public struct BusyStateChangedArgs
    {
        public bool IsBusy { get; init; }
        public string? BusyStatus { get; init; }
    }

    public delegate void OnBusyStateChangedHandler(object sender, BusyStateChangedArgs args);

    public event OnBusyStateChangedHandler? OnBusyStateChanged; 

    #endregion
    
    #region Members

    private bool _isBusy;
    private string? _busyStatus;

    #endregion

    #region Properties

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

    #region Methods

    protected void SetBusyState(bool isBusy, string? busyStatus = null)
    {
        // Set the fields and if there is a change in either one of the values, we need to raise an event
        if (SetField(ref _isBusy, isBusy, nameof(IsBusy)) ||
            SetField(ref _busyStatus, isBusy ? busyStatus : null, nameof(BusyStatus)))
        {
            OnBusyStateChanged?.Invoke(this, new BusyStateChangedArgs() { BusyStatus = _busyStatus, IsBusy = isBusy });
        }
    }

    #endregion
    
}