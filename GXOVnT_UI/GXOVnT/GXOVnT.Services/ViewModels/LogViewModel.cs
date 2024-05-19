using GXOVnT.Services.Common;
using GXOVnT.Services.Models;

namespace GXOVnT.Services.ViewModels;

public class LogViewModel : NotifyChanged
{

    #region Members

    private List<Models.LogMessage> _logMessages = new();

    private LogMessageType _minLogLevel = LogMessageType.Information; 
    #endregion

    #region Properties

    public IReadOnlyCollection<Models.LogMessage> LogMessages
    {
        get => GetFilteredMessages();
        private set => SetField(ref _logMessages, value.ToList());
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
        LogMessages = new List<LogMessage>();
    }

    public void SetMinLogLevel(LogMessageType logMessageType)
    {
        MinLogLevel = logMessageType;
    }
    
    public void Log(string message, LogMessageType logMessageType = LogMessageType.Information,
        DateTime? timestamp = default)
    {
        var logMessage = new LogMessage(message, timestamp, logMessageType);
        
        System.Diagnostics.Debug.WriteLine(logMessage.ToString());
        
        _logMessages.Add(logMessage);
        OnPropertyChanged(nameof(LogMessages));
    }
    
    public void LogDebug(string message) => Log(message, LogMessageType.Debug, DateTime.Now);
    public void LogInformation(string message) => Log(message, LogMessageType.Information, DateTime.Now);
    public void LogWarning(string message) => Log(message, LogMessageType.Warning, DateTime.Now);
    public void LogError(string message) => Log(message, LogMessageType.Error, DateTime.Now);

    private IReadOnlyList<LogMessage> GetFilteredMessages()
    {
        // Return the last 50 messages of the logs 
        return _logMessages.Where(msg => (int)_minLogLevel >= (int)msg.LogMessageType)
            .OrderBy(msg => msg.TimeStamp)
            .Skip(_logMessages.Count - 50)
            .ToList()
            .AsReadOnly();
    }
    #endregion

}