using GXOVnT.Services.Interfaces;
using GXOVnT.Shared.Common;
using MQTTnet;

namespace GXOVnT.Services.Services;

public class MqttServer : StateObject, IMqttServer 
{
    #region Members

    private bool _serverIsRunning;
    

    #endregion

    #region Properties

    public bool ServerIsRunning
    {
        get => _serverIsRunning;
        private set => SetField(ref _serverIsRunning, value);
    }

    #endregion

    #region ctor

    

    #endregion

    #region Methods

    public async Task StartServerAsync()
    {
        await RunTaskAsync(async () =>
        {
            if (_serverIsRunning) return;
            
            var mqttFactory = new MqttFactory();
            var mqttServerOptions = mqttFactory.CreateServerOptionsBuilder().WithDefaultEndpoint().Build();
            var server = mqttFactory.CreateMqttServer(mqttServerOptions);

        }, "Starting Mqtt server");
    }

    public async Task StopServerAsync()
    {
        await RunTaskAsync(async () =>
        {
            if (!_serverIsRunning) return;

            
        }, "Starting Mqtt server");
    }
    

    #endregion

}