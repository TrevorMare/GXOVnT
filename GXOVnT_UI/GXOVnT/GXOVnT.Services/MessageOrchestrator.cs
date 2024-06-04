using GXOVnT.Services.Common;
using GXOVnT.Services.Interfaces;
using GXOVnT.Services.Models;
using GXOVnT.Services.ViewModels;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.DeviceMessage;
using GXOVnT.Shared.JsonModels;

namespace GXOVnT.Services;

public delegate void MessageAggregateReceivedHandler(object sender, MessageAggregate messageAggregate);

internal class MessageOrchestrator : NotifyChanged, IMessageOrchestrator
{

    #region Events
    public event MessageAggregateReceivedHandler? MessageAggregateReceived;
    #endregion
    
    #region Members
    private readonly IBluetoothService _bluetoothService;
    private readonly List<CommMessage> _receivedMessagesIncomplete = new();
    private readonly LogViewModel _logViewModel;
    private readonly List<MessageAggregate> _completedMessages = new();
    private readonly List<short> _messagesInWaitingState = new();
    private bool _isBusy;
    private string _progressText = string.Empty;
    private int _progress = 0;
    #endregion

    #region Properties
    
    public bool IsBusy
    {
        get => _isBusy;
        private set => SetField(ref _isBusy, value);
    }

    public string ProgressText
    {
        get => _progressText;
        private set => SetField(ref _progressText, value);
    }
    
    public int Progress
    {
        get => _progress;
        private set => SetField(ref _progress, value);
    }
    #endregion

    #region ctor

    public MessageOrchestrator(IBluetoothService bluetoothService, LogViewModel logViewModel)
    {
        _bluetoothService = bluetoothService;
        _logViewModel = logViewModel;
        _bluetoothService.OnMessagePacketReceived += BluetoothServiceOnOnMessagePacketReceived;
    }

    #endregion

    #region Methods
    public async Task SendMessage<T>(T message) where T : BaseModel
    {
        try
        {
            IsBusy = true;
            ProgressText = "Sending message to device";
            await _bluetoothService.SendJsonModelToDevice(message, p => Progress = p);
        }
        catch (Exception ex)
        {
            _logViewModel.LogError($"An unhandled error occured sending the message to the device, {ex.Message}");
        }
        finally
        {
            Progress = 0;
            ProgressText = string.Empty;
            IsBusy = false;
        }
        
    }

    public async Task<TOut?> SendMessage<TIn, TOut>(TIn message, CancellationToken cancellationToken = default) 
        where TOut : Shared.JsonModels.BaseModel where TIn : Shared.JsonModels.BaseModel
    {

        short outgoingMessageId = -1;
        
        try
        {
            IsBusy = true;
            ProgressText = "Sending message to device";
            outgoingMessageId = await _bluetoothService.SendJsonModelToDevice(message, p => Progress = p);
            
            if (outgoingMessageId == -1)
            {
                _logViewModel.LogWarning(
                    "An attempt was made to send a message to the device but it was not connected");
                return default(TOut);
            }

            lock (_messagesInWaitingState)
            {
                _messagesInWaitingState.Add(outgoingMessageId);    
            }
            
            
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(500));

            // Reset the progress
            Progress = 0;
            ProgressText = "Waiting for device response";
            
            // Check if we have a cancellation token passed, if not create our own with a default of 30 seconds timeout
            if (cancellationToken == CancellationToken.None)
                cancellationToken = cancellationTokenSource.Token;

            var result = default(TOut);

            // Now we need to wait for the message to return
            await Task.Run(async () =>
            {
                var receivedResponse = false;
                    
                while (!cancellationToken.IsCancellationRequested && !receivedResponse)
                {
                    // Check to see if we have a valid response
                    var messageAggregate = _completedMessages.Find(x => x.ReplyToMessageId == outgoingMessageId);
                    if (messageAggregate != null)
                    {
                        result = System.Text.Json.JsonSerializer.Deserialize<TOut>(messageAggregate.CommMessage
                            .ToString());

                        _completedMessages.Remove(messageAggregate);
                        receivedResponse = true;
                    }                        

                    await Task.Delay(300, cancellationToken);
                }
            }, cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            _logViewModel.LogError($"An unhandled error occured sending the message to the device, {ex.Message}");
            return default(TOut);
        }
        finally
        {
            // Remove the id from the waiting state
            if (outgoingMessageId > 0)
            {
                lock (_messagesInWaitingState)
                {
                    _messagesInWaitingState.Remove(outgoingMessageId);    
                }
            }
                
            
            Progress = 0;
            ProgressText = string.Empty;
            IsBusy = false;
        }
    }

    #endregion

    #region Event Callbacks

    private void BluetoothServiceOnOnMessagePacketReceived(object sender, byte[]? buffer)
    {
        try
        {
            if (buffer == null) return;
            _logViewModel.LogDebug($"Message received from device. Inspecting packet");
        
            var commMessagePacket = new CommMessagePacket();
            commMessagePacket.DeSerializePacket(buffer);

            _logViewModel.LogDebug($"Comm Message packet with Id {commMessagePacket.CommMessagePacketId} for message Id {commMessagePacket.CommMessageId}");
            _logViewModel.LogDebug($"Packet start: {commMessagePacket.CommMessageDetail.HasFlag(CommMessageDetail.IsStartPacket)}, Packet end: {commMessagePacket.CommMessageDetail.HasFlag(CommMessageDetail.IsEndPacket)}");

            var commMessage = _receivedMessagesIncomplete.Find(x => commMessagePacket.CommMessageId == x.MessageId);
            if (commMessage == null)
            {
                commMessage = new CommMessage(commMessagePacket);
                _receivedMessagesIncomplete.Add(commMessage);
                return;
            }

            commMessage.AddCommMessagePacket(commMessagePacket);

            if (!commMessage.MessageComplete) 
                return;
            
            _logViewModel.LogDebug($"Message is now complete, processing message");

            // At the message completion point we need to check if there is an outgoing message waiting for a response
            // If there is, we need to serve it to the waiting process
            var messageAggregate = new MessageAggregate(commMessage);
            if (messageAggregate.ReplyToMessageId > 0 && _messagesInWaitingState.Contains(messageAggregate.ReplyToMessageId))
            {
                // There is an outgoing message waiting for this response, add it to the completed messages
                lock (_completedMessages)
                {
                    _receivedMessagesIncomplete.Remove(commMessage);
                    _completedMessages.Add(messageAggregate);
                    return;
                }
            }
            
            // There is nothing waiting on this item, the device might have initiated the message e.g. progress.
            // The view models can be notified via an event
            _receivedMessagesIncomplete.Remove(commMessage);
            MessageAggregateReceived?.Invoke(this, messageAggregate);
        }
        catch (Exception ex)
        {
            _logViewModel.LogError($"An unhandled error occured processing an incoming message packet, {ex.Message}");
        }
    }


    #endregion
    
}