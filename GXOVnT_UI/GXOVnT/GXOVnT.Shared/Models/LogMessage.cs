using GXOVnT.Shared.Common;

namespace GXOVnT.Shared.Models;

public class LogMessage
{

    #region Properties

    public string Message { get; set; } = string.Empty;

    public DateTime TimeStamp { get; set; } = DateTime.Now;

    public LogMessageType LogMessageType { get; set; } = LogMessageType.Information;

    #endregion

    #region ctor

    public LogMessage() { }
    
    public LogMessage(string message, DateTime? timeStamp, LogMessageType? logMessageType = default)
    {
        Message = message;
        TimeStamp = timeStamp ?? DateTime.Now;
        LogMessageType = logMessageType ?? LogMessageType.Information;
    }

    #endregion

    #region Overrides

    public override string ToString()
    {
        return $"{TimeStamp:yyyy-MM-dd HH:mm:ss} [{LogMessageType.ToString()}] : {Message}";
    }

    #endregion


}