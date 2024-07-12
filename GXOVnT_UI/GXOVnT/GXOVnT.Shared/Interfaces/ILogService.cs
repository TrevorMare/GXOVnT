using System.ComponentModel;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.Models;

namespace GXOVnT.Shared.Interfaces;

public interface ILogService : INotifyPropertyChanged
{
    IReadOnlyCollection<LogMessage> LogMessages { get; }
    
    LogMessageType MinLogLevel { get; }

    void ClearLogMessages();
    
    void SetMinLogLevel(LogMessageType logMessageType);

    void Log(string message, LogMessageType? logMessageType = default,
        DateTime? timestamp = default);

    void LogDebug(string message);
    
    void LogInformation(string message);
    
    void LogWarning(string message);
    
    void LogError(string message);

}