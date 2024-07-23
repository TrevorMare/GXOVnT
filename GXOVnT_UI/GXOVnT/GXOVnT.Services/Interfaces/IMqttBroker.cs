namespace GXOVnT.Services.Interfaces;

public interface IMqttBroker
{

    Task StartServerAsync();
    
    Task StopServerAsync();

}