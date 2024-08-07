﻿using GXOVnT.Services.Interfaces;
using GXOVnT.Services.Models;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.DeviceMessage;
using GXOVnT.Shared.DeviceMessage.Common;
using GXOVnT.Shared.Interfaces;
using Plugin.BLE.Abstractions;

namespace GXOVnT.Services.Services;

public delegate void MessageAggregateReceivedHandler(object sender, MessageAggregate messageAggregate);

internal class MessageOrchestrator : StateObject, IMessageOrchestrator
{

    #region Events
    public event MessageAggregateReceivedHandler? MessageAggregateReceived;
    #endregion
    
    #region Members
    private readonly IBluetoothService _bluetoothService;
    private int _progress;
    #endregion

    #region Properties
    public int Progress
    {
        get => _progress;
        private set => SetField(ref _progress, value);
    }
    #endregion

    #region ctor

    public MessageOrchestrator(IBluetoothService bluetoothService)
    {
        _bluetoothService = bluetoothService;
        
    }

    #endregion

    #region Methods
    public async Task SendMessage<T>(T message, Guid deviceId) where T : BaseMessageModel
    {
        try
        {
            var bleDevice = FindDevice(deviceId);
            if (bleDevice == null)
                throw new GXOVnTException(
                    "An error occured trying to send a device message, could not locate the device with the specified Id");

            await SendMessage(message, bleDevice);
        }
        catch (Exception ex)
        {
            LogService.LogError($"An unhandled error occured sending the message to the device, {ex.Message}");
        }
    }

    public async Task SendMessage<T>(T message, Models.System bleDevice) where T : BaseMessageModel
    {
        try
        {
            SetProgressStartSending();
            
            await bleDevice.SendJsonModelToDevice(message, p => Progress = p);
        }
        catch (Exception ex)
        {
            LogService.LogError($"An unhandled error occured sending the message to the device, {ex.Message}");
        }
        finally
        {
            SetProgressComplete();
        }
    }

    public async Task<TOut?> SendMessage<TIn, TOut>(TIn message, Guid deviceId, CancellationToken cancellationToken = default) 
        where TOut : BaseMessageModel where TIn : BaseMessageModel
    {
        
        try
        {
            var bleDevice = FindDevice(deviceId);
            if (bleDevice == null)
                throw new GXOVnTException(
                    "An error occured trying to send a device message, could not locate the device with the specified Id");

            return await SendMessage<TIn, TOut>(message, bleDevice, cancellationToken);

        }
        catch (Exception ex)
        {
            LogService.LogError($"An unhandled error occured sending the message to the device, {ex.Message}");
            return default(TOut);
        }
    }

    public async Task<TOut?> SendMessage<TIn, TOut>(TIn message, Models.System bleDevice, CancellationToken cancellationToken = default) where TIn : BaseMessageModel where TOut : BaseMessageModel
    {
        short outgoingMessageId;
        var messagesReceivedDuringWait = new List<MessageAggregate>();
        
        Models.System.CommMessageReceived commMessageReceivedHandler = (_, e) =>
        {
            messagesReceivedDuringWait.Add(e);
        };
        
        try
        {
            
            if (bleDevice == null)
                throw new GXOVnTException(
                    "An error occured trying to send a device message, could not locate the device with the specified Id");

            SetProgressStartSending();

            // Attach the message received handler to the device
            bleDevice.OnCommMessageReceived += commMessageReceivedHandler;
            
            // Now send the message to the device
            outgoingMessageId = await bleDevice.SendJsonModelToDevice(message, p => Progress = p);
            
            if (outgoingMessageId == -1)
            {
                LogService.LogWarning(
                    "An attempt was made to send a message to the device but it was not connected");
                return default(TOut);
            }
            
            SetProgressWaitingResponse();
            
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
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
                    var messageAggregate = messagesReceivedDuringWait.Find(x => x.ReplyToMessageId == outgoingMessageId);
                    if (messageAggregate != null)
                    {
                        result = System.Text.Json.JsonSerializer.Deserialize<TOut>(messageAggregate.CommMessage
                            .ToString());
                        receivedResponse = true;
                    }                        

                    await Task.Delay(300, cancellationToken);
                    
                    // Check if the device is still connected
                    if (bleDevice.DeviceState != DeviceState.Connected)
                        throw new GXOVnTException("Connection to device lost");
                }
            }, cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            LogService.LogError($"An unhandled error occured sending the message to the device, {ex.Message}");
            return default(TOut);
        }
        finally
        {
            // Remove the message received handler to the device
           if (bleDevice != null)
               bleDevice.OnCommMessageReceived -= commMessageReceivedHandler;

            SetProgressComplete();
        }
    }

    #endregion

    #region Private Methods

    private void SetProgressStartSending()
    {
        SetBusyState(true, "Sending message to system");
        Progress = 0;
        
    }
    
    private void SetProgressWaitingResponse()
    {
        SetBusyState(true, "Waiting for response from system");
        Progress = 0;
    }
    
    private void SetProgressComplete()
    {
        SetBusyState(false);
        Progress = 0;
    }
    
    private Models.System? FindDevice(Guid deviceId)
    {
        return _bluetoothService.FindDevice(deviceId);
    }

    #endregion
    
  
    
}