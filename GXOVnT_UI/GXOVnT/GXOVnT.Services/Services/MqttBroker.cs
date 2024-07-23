using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using GXOVnT.Services.Interfaces;
using GXOVnT.Shared.Common;
using MQTTnet;
using MQTTnet.Server;

namespace GXOVnT.Services.Services;

public class MqttBroker : StateObject, IMqttBroker, IAsyncDisposable 
{
    #region Members

    private bool _brokerIsRunning;
    private MqttServer? _mqttServer;

    #endregion

    #region Properties

    public bool BrokerIsRunning
    {
        get => _brokerIsRunning;
        private set => SetField(ref _brokerIsRunning, value);
    }

    #endregion

    #region ctor

    

    #endregion

    #region Methods

    public async Task StartServerAsync()
    {
        await RunTaskAsync(async () =>
        {
            
        
            
            if (_brokerIsRunning) return;

            BrokerIsRunning = false;
            
            LogService.LogDebug("Attempting to find the IP Address for creating Mqtt broker server");

            var platformServices = new PlatformServices();
            var ipAddress = platformServices.ReadIpAddress();

            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                LogService.LogDebug("Could not find the IP Address for the device. ");
                return;
            }

            
            LogService.LogDebug($"IP Address for creating Mqtt broker server: {ipAddress}");
           
            var mqttServerOptions = new MqttServerOptionsBuilder()
                .WithDefaultEndpointPort(1883)
                .WithDefaultEndpointBoundIPAddress(IPAddress.Parse(ipAddress))
                .WithDefaultEndpointBoundIPV6Address(IPAddress.None)
                .WithPersistentSessions()
                .WithKeepAlive()
                .WithoutEncryptedEndpoint()
                .Build();
            
            var mqttFactory = new MqttFactory();
            
            LogService.LogDebug($"Creating a new Mqtt broker server");
            
            _mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions);

            LogService.LogDebug($"Starting the Mqtt broker server");
            await _mqttServer.StartAsync();

            BrokerIsRunning = true;
        }, "Starting Mqtt broker services");
    }

    public async Task StopServerAsync()
    {
        await RunTaskAsync(async () =>
        {
            if (!_brokerIsRunning || _mqttServer == null) return;

            await _mqttServer.StopAsync();

        }, "Stopping Mqtt broker services");
    }
    

    #endregion

    #region Private Methods

    

    #endregion

    #region Dispose

    public async ValueTask DisposeAsync()
    {
        await StopServerAsync();
        _mqttServer?.Dispose();
    }

    #endregion
}