namespace GXOVnT.Services.Interfaces;

public interface IMqttServer
{

    Task StartServerAsync();
    
    Task StopServerAsync();

}