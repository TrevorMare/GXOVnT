namespace GXOVnT.Services.Models;

/// <summary>
/// This container class is used to transfer large messages in smaller packages via the communications channel between
/// the device and the software
/// </summary>
internal class CommMessage
{

    #region Properties

    public Int16 MessageId { get; set; } = 1;
    

    #endregion

    #region ctor

    public CommMessage(Int16 messageId)
    {
        if (messageId <= 0)
            throw new ArgumentOutOfRangeException(nameof(messageId), "Message Id should be greater than 0");
        MessageId = messageId;

    }

    #endregion


}