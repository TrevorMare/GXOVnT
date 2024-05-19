using System.ComponentModel;

namespace GXOVnT.Services.Interfaces;

public interface IBluetoothService : INotifyPropertyChanged
{

    Task<bool> InitializeService();

    bool IsBluetoothReady();
    
}