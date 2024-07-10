using System.ComponentModel;
using GXOVnT.Services.Models;
using GXOVnT.Services.Services;
using GXOVnT.Shared.DeviceMessage;
using GXOVnT.Shared.DeviceMessage.Common;

namespace GXOVnT.Services.Interfaces;

public interface IMessageOrchestrator : INotifyPropertyChanged
{

    bool IsBusy { get; }
    
    string ProgressText { get; }
    
    int Progress { get; }
    
    event MessageAggregateReceivedHandler? MessageAggregateReceived;

    /// <summary>
    /// Sends a message in a fire and forget way
    /// </summary>
    /// <param name="message"></param>
    /// <param name="deviceId"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task SendMessage<T>(T message, Guid deviceId) where T : BaseMessageModel;
    
    Task SendMessage<T>(T message, Models.System bleDevice) where T : BaseMessageModel;

    /// <summary>
    /// Sends a message and waits for a response from the device
    /// </summary>
    /// <param name="message"></param>
    /// <param name="deviceId"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TIn"></typeparam>
    /// <returns></returns>
    Task<TOut?> SendMessage<TIn, TOut>(TIn message, Guid deviceId, CancellationToken cancellationToken = default) where TOut : BaseMessageModel where TIn : BaseMessageModel;
    
    Task<TOut?> SendMessage<TIn, TOut>(TIn message, Models.System bleDevice, CancellationToken cancellationToken = default) where TOut : BaseMessageModel where TIn : BaseMessageModel;

}