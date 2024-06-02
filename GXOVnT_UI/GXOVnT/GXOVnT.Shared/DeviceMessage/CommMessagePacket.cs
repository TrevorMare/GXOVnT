using GXOVnT.Shared.Common;

namespace GXOVnT.Shared.DeviceMessage;

/// <summary>
/// Container class for the packet bytes within a larger message
/// </summary>
public sealed class CommMessagePacket
{

    #region Properties

    public Int16 CommMessageId { get; set; } = 0;

    public byte CommMessagePacketId { get; set; } = 0x0;

    public CommMessageDetail CommMessageDetail { get; set; }

    public byte[] Buffer { get; set; } = Array.Empty<byte>();
    #endregion

    #region ctor

    internal CommMessagePacket(Int16 commMessageId, byte commMessagePacketId, CommMessageDetail commMessageDetail, byte[] buffer)
    {
        if (buffer.Length < 1)
            throw new ArgumentNullException(nameof(buffer), "CommMessagePacket buffer cannot be null or empty");

        CommMessageId = commMessageId;
        CommMessagePacketId = commMessagePacketId;
        CommMessageDetail = commMessageDetail;
        Buffer = buffer;
    }

    public CommMessagePacket()
    {
        
    }

    #endregion
    
    #region Methods

    public byte[] SerializePacket()
    {
        var result = new List<byte>()
        {
            (byte)(CommMessageId >> 8),  (byte)(CommMessageId), CommMessagePacketId, (byte)CommMessageDetail
        };
        result.AddRange(Buffer);
        return result.ToArray();
    }

    public void DeSerializePacket(byte[] buffer)
    {
        CommMessageId = (short)((buffer[0] << 8) | buffer[1]);
        CommMessagePacketId = buffer[2];
        CommMessageDetail = (CommMessageDetail)buffer[3];
        Buffer = buffer[4..];
    }

    #endregion

}