using System.Collections;
using GXOVnT.Shared.Common;

namespace GXOVnT.Shared.DeviceMessage;

/// <summary>
/// This container class is used to transfer large messages in smaller packages via the communications channel between
/// the device and the software
/// </summary>
public class CommMessage : IEnumerator<CommMessagePacket>
{

    #region Members

    private readonly List<CommMessagePacket> _messagePackets = new();
    private int _position = -1;
    
    #endregion
    
    #region Properties

    public Int16 MessageId { get; set; } 

    public IReadOnlyList<CommMessagePacket> MessagePackets => _messagePackets.AsReadOnly();

    public int Count => MessagePackets.Count;

    public bool MessageComplete => _messagePackets.Exists(x => x.CommMessageDetail.HasFlag(CommMessageDetail.IsEndPacket));
    #endregion

    #region ctor

    public CommMessage(CommMessagePacket commMessagePacket)
    {
        MessageId = commMessagePacket.CommMessageId;
        _messagePackets.Add(commMessagePacket);
    }

    public CommMessage(Int16 messageId, byte[] messageBuffer, int packetChunkSize = 20)
    {
        if (messageId <= 0)
            throw new ArgumentOutOfRangeException(nameof(messageId), "Message Id should be greater than 0");

        if (messageBuffer.Length == 0)
            throw new ArgumentException("Message buffer cannot be null or empty", nameof(messageBuffer));
    
        MessageId = messageId;
        
        var packetBuffers = messageBuffer.Chunk(packetChunkSize).ToList();

        for (byte iPacket = 0; iPacket < packetBuffers.Count; iPacket++)
        {
            var packetDetail = CommMessageDetail.None;
            if (iPacket == 0) packetDetail |= CommMessageDetail.IsStartPacket;
            if (iPacket == packetBuffers.Count - 1) packetDetail |= CommMessageDetail.IsEndPacket;
            
            _messagePackets.Add(new CommMessagePacket(messageId, iPacket, packetDetail, packetBuffers[iPacket]));
        }
    }

    #endregion

    #region Overrides

    public override string ToString()
    {
        var orderedMessagePackets = _messagePackets.OrderBy(p => p.CommMessagePacketId);
        var buffer = new List<byte>();

        foreach (var orderedMessagePacket in orderedMessagePackets)
        {
            buffer.AddRange(orderedMessagePacket.Buffer);
        }
        return System.Text.Encoding.UTF8.GetString(buffer.ToArray());
    }

    #endregion

    #region Methods
    public void AddCommMessagePacket(CommMessagePacket commMessagePacket)
    {
        _messagePackets.Add(commMessagePacket);
    }

    public bool MoveNext()
    {
        var updatedPosition = _position + 1;

        if (updatedPosition < 0 || updatedPosition >= _messagePackets.Count) 
            return false;
        
        _position = updatedPosition;
        return true;
    }

    public void Reset()
    {
        this._position = -1;
    }

    public CommMessagePacket Current => _messagePackets[_position];

    object IEnumerator.Current => Current;


    public void Dispose()
    {
        
    }
    #endregion

 
}