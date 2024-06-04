using System.ComponentModel;

namespace GXOVnT.Services.Interfaces;

public interface IMessageOrchestrator : INotifyPropertyChanged
{

    event MessageAggregateReceivedHandler? MessageAggregateReceived;
    
    /// <summary>
    /// Sends a message in a fire and forget way
    /// </summary>
    /// <param name="message"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task SendMessage<T>(T message) where T : Shared.JsonModels.BaseModel;
    
    /// <summary>
    /// Sends a message and waits for a response from the device
    /// </summary>
    /// <param name="message"></param>
    /// <typeparam name="TOut"></typeparam>
    /// <typeparam name="TIn"></typeparam>
    /// <returns></returns>
    Task<TOut?> SendMessage<TIn, TOut>(TIn message, CancellationToken cancellationToken = default) where TOut : Shared.JsonModels.BaseModel where TIn : Shared.JsonModels.BaseModel;

}