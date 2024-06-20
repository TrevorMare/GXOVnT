using GXOVnT.Services.Interfaces;
using GXOVnT.Services.Models;
using GXOVnT.Shared.Common;

namespace GXOVnT.Services.Services;

public class LogService : NotifyChanged, ILogService
{
    
    #region Members

    private readonly List<Models.LogMessage> _logMessages = new();

    private LogMessageType _minLogLevel = LogMessageType.Information; 
    #endregion

    #region Properties

    public IReadOnlyCollection<LogMessage> LogMessages
    {
        get => GetFilteredMessages();
    }

    public LogMessageType MinLogLevel
    {
        get => _minLogLevel;
        private set => SetField(ref _minLogLevel, value);
    }
    #endregion

    #region Methods

    public void ClearLogMessages()
    {
        lock (_logMessages)
        {
            _logMessages.Clear();    
        }
        OnPropertyChanged(nameof(LogMessages));
    }

    public void SetMinLogLevel(LogMessageType logMessageType)
    {
        MinLogLevel = logMessageType;
    }
    
    public void Log(string message, LogMessageType? logMessageType = default,
        DateTime? timestamp = default)
    {
        lock (_logMessages)
        {
            var logMessage = new LogMessage(message, timestamp, logMessageType ?? LogMessageType.Information);
            _logMessages.Add(logMessage);
        }
        
        OnPropertyChanged(nameof(LogMessages));
    }
    
    public void LogDebug(string message) => Log(message, LogMessageType.Debug, DateTime.Now);
    public void LogInformation(string message) => Log(message, LogMessageType.Information, DateTime.Now);
    public void LogWarning(string message) => Log(message, LogMessageType.Warning, DateTime.Now);
    public void LogError(string message) => Log(message, LogMessageType.Error, DateTime.Now);

    private IReadOnlyList<LogMessage> GetFilteredMessages()
    {
        // Return the last 50 messages of the logs 

        lock (_logMessages)
        {
            return _logMessages.Where(msg => _minLogLevel.Id >= msg.LogMessageType.Id)
                .OrderBy(msg => msg.TimeStamp)
                .TakeLast(50)
                .ToList()
                .AsReadOnly();
        }
    }
    #endregion
    
}