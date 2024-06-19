using GXOVnT.Services.Models;
using GXOVnT.Shared.Common;

namespace GXOVnT.Services.Interfaces;

public interface ILogService
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