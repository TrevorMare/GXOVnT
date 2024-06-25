using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GXOVnT.Shared.Common;

public abstract class NotifyChanged : INotifyPropertyChanged
{

    #region Properties

    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion

    #region Methods

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

    #endregion
    
}