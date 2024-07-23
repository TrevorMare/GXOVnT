using GXOVnT.Services.Interfaces;
using GXOVnT.Shared.Common;
using GXOVnT.Shared.DeviceMessage;
using GXOVnT.Shared.DeviceMessage.Common;
using GXOVnT.Shared.DeviceMessage.Extensions;
using GXOVnT.Shared.Interfaces;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;

namespace GXOVnT.Services.Models;

public class System : StateObject, IAsyncDisposable
{

    #region Events

    public delegate void CommMessageReceived(object sender, MessageAggregate args);

    public event CommMessageReceived? OnCommMessageReceived;

    #endregion
    
    #region Members

    private bool _isBusy;
    private short _outgoingMessageId = 1;
    private static readonly int DelayMillisBetweenPackages = 200;
    /// <summary>
    /// Unique id that the GXOVnT devices broadcast
    /// </summary>
    private static readonly Guid GXOVnTBluetoothServiceId = new ("05c1fba8-cc8b-4534-8787-0e6a0775c3de");
    /// <summary>
    /// This id is relative to the GXOVnT device and not the incoming for this service
    /// </summary>
    private static readonly Guid GXOVnTBluetoothIncomingCharacteristic = new ("4687b690-cd36-4a7c-9134-49ffe62d9e4f");
    /// <summary>
    /// This id is relative to the GXOVnT device and not the outgoing for this service
    /// </summary>
    private static readonly Guid GXOVnTBluetoothOutgoingCharacteristic = new ("4687b690-cd36-4a7c-9134-49ffe62d954f");
    private readonly IAdapter _bluetoothAdapter;
    private readonly ILogService _logService;
    private ICharacteristic? _incomingMessageCharacteristic;
    private ICharacteristic? _outgoingMessageCharacteristic;
    private bool _deviceServicesBound;
    private readonly string _uuid = Guid.NewGuid().ToString();

    private readonly List<CommMessage> _partialCommMessages = new();
    private readonly List<MessageAggregate> _historicCommMessages = new();
    private MessageAggregate? _lastReceivedMessage;
    
    private SystemType _systemType = Shared.Common.SystemType.UnInitialized;
    private bool _systemConfigured;
    private bool _deviceIsReconnecting;
    #endregion
    
    #region Properties
    public bool SystemConfigured
    {
        get => _systemConfigured;
        set => SetField(ref _systemConfigured, value);
    }
    
    public SystemType SystemType
    {
        get => _systemType;
        set => SetField(ref _systemType, value);
    }

    public Guid Id => Device.Id;
    
    public string UUID => _uuid;

    public bool DeviceServicesBound
    {
        get => _deviceServicesBound;
        private set => SetField(ref _deviceServicesBound, value);
    }

    public bool DeviceIsConnected => (Device.State == DeviceState.Connected && !DeviceIsReconnecting);
    
    public IReadOnlyList<MessageAggregate> HistoricCommMessages => _historicCommMessages.AsReadOnly();

    public MessageAggregate? LastReceivedMessage
    {
        get => _lastReceivedMessage;
        private set => SetField(ref _lastReceivedMessage, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetField(ref _isBusy, value);
    }
    
    public bool DeviceIsReconnecting
    {
        get => _deviceIsReconnecting;
        private set => SetField(ref _deviceIsReconnecting, value);
    }

    public IDevice Device { get; private set; }

    public string DeviceName => Device.Name ;

    public int Rssi => Device.Rssi;

    public bool IsConnectable => Device.IsConnectable;

    public DeviceState DeviceState => Device.State;
    #endregion

    #region ctor

    public System(IAdapter bluetoothAdapter, IDevice device, ILogService logService)
    {
        _bluetoothAdapter = bluetoothAdapter;
        _logService = logService;
        Device = device;
        ParseManufacturerData();
    }

    #endregion

    #region Public Methods

    public void ConnectionStateChanged()
    {
        OnPropertyChanged(nameof(DeviceState));
    }

    public async Task<bool> ReconnectWhenAvailable(CancellationToken cancellationToken = default)
    {
        try
        {
            IsBusy = true;
            DeviceIsReconnecting = true;

            if (_bluetoothAdapter == null)
                throw new GXOVnTException(
                    "The Bluetooth connection cannot be made at this stage. The Adapter reference was lost");

            if (Device == null)
                throw new GXOVnTException(
                    "The Bluetooth connection cannot be made at this stage. The Device reference was lost");

            await _bluetoothAdapter.DisconnectDeviceAsync(Device);

            UnBindFromDeviceServicesAndCharacteristics();

            using var localCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            if (cancellationToken == default)
                cancellationToken = localCancellationTokenSource.Token;

            // First perform the connect to known device
            var deviceReconnected = false;
            while (!cancellationToken.IsCancellationRequested && !deviceReconnected)
            {
                try
                {
                    // The return value is the same device object that was created and returned from the bluetooth service 
                    Device = await _bluetoothAdapter.ConnectToKnownDeviceAsync(Device.Id, new ConnectParameters(),
                        cancellationToken);

                    deviceReconnected = true;
                }
                catch (Exception ex) when (ex is DeviceConnectionException)
                {
                    // Android will throw this exception after a few seconds if it could not connect
                    await Task.Delay(2000, cancellationToken);
                }
                catch (Exception ex) when (ex is OperationCanceledException)
                {
                    // IoS will not throw an exception but the cancellation token will expire
                }
            }

            // Next wait until the device status is back to connected
            if (deviceReconnected)
            {
                await Task.Delay(1000, cancellationToken);
                while (!cancellationToken.IsCancellationRequested && Device.State != DeviceState.Connected)
                {
                    try
                    {
                        await Task.Delay(200, cancellationToken);
                    }
                    catch (Exception ex) when (ex is OperationCanceledException)
                    {
                        // Wait until the device gives the connected state again 
                    }
                }
            }

            OnPropertyChanged(nameof(DeviceIsConnected));
            return deviceReconnected;
        }
        catch (TaskCanceledException)
        {
            OnPropertyChanged(nameof(DeviceIsConnected));
            return false;
        }
        catch (Exception ex)
        {
            _logService.LogError($"An error occured trying to connect to the device with Id {Id}: {ex.Message}");
            OnPropertyChanged(nameof(DeviceIsConnected));
            return false;
        }
        finally
        {
           
            await BindToDeviceServicesAndCharacteristics();
            
            DeviceIsReconnecting = false;
            IsBusy = false;
        }
    }
    
    public async Task<bool> ConnectToDeviceAsync(CancellationToken cancellationToken = default)
    {
        
        try
        {
            IsBusy = true;
            
            // Check if this device is already connected
            if (DeviceIsConnected)
            {
                _logService.LogInformation($"The device with Id {Id} is already connected");
                return true;
            }

            if (_bluetoothAdapter == null)
                throw new GXOVnTException(
                    "The Bluetooth connection cannot be made at this stage. The Adapter reference was lost");

            if (Device == null)
                throw new GXOVnTException(
                    "The Bluetooth connection cannot be made at this stage. The Device reference was lost");

            using var localCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            if (cancellationToken == default)
                cancellationToken = localCancellationTokenSource.Token;

            // Connect to the device
            await _bluetoothAdapter.ConnectToDeviceAsync(Device, new ConnectParameters(), cancellationToken);

            OnPropertyChanged(nameof(DeviceIsConnected));
            
            // Load the services and the characteristics
            await BindToDeviceServicesAndCharacteristics();
           
            return true;
        }
        catch (Exception ex)
        {
            _logService.LogError($"An error occured trying to connect to the device with Id {Id}: {ex.Message}");
            OnPropertyChanged(nameof(DeviceIsConnected));
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task<bool> DisconnectFromDeviceAsync()
    {
        try
        {
            IsBusy = true;
            if (!DeviceIsConnected)
            {
                _logService.LogInformation($"The device with Id {Id} is already disconnected");
                return true;
            }

            if (_bluetoothAdapter == null)
                throw new GXOVnTException(
                    "The Bluetooth connection cannot be made at this stage. The Adapter reference was lost");

            if (Device == null)
                throw new GXOVnTException(
                    "The Bluetooth connection cannot be made at this stage. The Device reference was lost");

            await _bluetoothAdapter.DisconnectDeviceAsync(Device);

            // Unbind the services and characteristics
            UnBindFromDeviceServicesAndCharacteristics();
            return true;
        }
        catch (Exception ex)
        {
            _logService.LogError($"An error occured trying to disconnect from the device with Id {Id}: {ex.Message}");
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task<short> SendJsonModelToDevice<T>(T jsonModel, Action<int>? progressChangedCallback = default) where T : BaseMessageModel
    {
        try
        {
            if (!DeviceIsConnected)
            {
                var connected = await ConnectToDeviceAsync();
                if (!connected)
                    throw new GXOVnTException(
                        "Unable to send the message to the device, could not connect to the device");
            }

            if (_outgoingMessageCharacteristic == null)
                await BindToDeviceServicesAndCharacteristics();
            
            if (_outgoingMessageCharacteristic == null)
                throw new GXOVnTException(
                    "Unable to send the message to the device, reference to the outgoing characteristic lost");

            var commMessage = jsonModel.ToCommMessage(_outgoingMessageId);

            var currentIndex = 0;
            var numberOfPackets = commMessage.MessagePackets.Count;
            
            while (commMessage.MoveNext())
            {
                await _outgoingMessageCharacteristic!.WriteAsync(commMessage.Current.SerializePacket());

                currentIndex++;
                
                // Raise the progress changed
                if (progressChangedCallback != null)
                {
                    var calculatedProgress = ((decimal)currentIndex / (decimal)numberOfPackets) * 100;
                    var intProgress = (int)Math.Ceiling(calculatedProgress);
                    progressChangedCallback.Invoke(intProgress);
                }
                
                await Task.Delay(DelayMillisBetweenPackages);
            }

            return _outgoingMessageId;
        }
        catch (Exception ex)
        {
            _logService.LogError($"An error occured writing the value to the device: {ex.Message}");
            return -1;
        }
        finally
        {
            _outgoingMessageId++;
            if (_outgoingMessageId >= short.MaxValue)
                _outgoingMessageId = 1;
        }
    }

    #endregion
    
    #region Private Methods

    private void ParseManufacturerData()
    {
        try
        {
            var allAdvertisementRecords = Device.AdvertisementRecords ?? new List<AdvertisementRecord>();
            if (!allAdvertisementRecords.Any())
                return;

            var manufacturerDataRecord = allAdvertisementRecords.ToList()
                .Find(r => r.Type == AdvertisementRecordType.ManufacturerSpecificData);
            
            if (manufacturerDataRecord == null)
                return;

            var manufacturerDataValue = global::System.Text.Encoding.UTF8.GetString(manufacturerDataRecord.Data);
            var manufacturerParts = manufacturerDataValue.Split("|");

            if (manufacturerParts.Length >= 2)
                SystemType = Enumeration.FromValue<SystemType>(int.Parse(manufacturerParts[1])) ?? SystemType.UnInitialized;
            if (manufacturerParts.Length >= 3)
                SystemConfigured = manufacturerParts[2] == "1";
        }
        catch (Exception ex)
        {
            _logService.LogError($"There was an error reading the manufacturer data on device with Id {Id}. {ex.Message}");
        }
    }
    
    private async Task BindToDeviceServicesAndCharacteristics()
    {
        var deviceServicesBound = false;
        
        try
        {
            IsBusy = true;
            
            if (!DeviceIsConnected)
                throw new GXOVnTException($"Unable to bind to device services, the device with Id  {Id} is not connected");
            
            var connectedDeviceService = await Device.GetServiceAsync(GXOVnTBluetoothServiceId);
            if (connectedDeviceService == null)
                throw new GXOVnTException("Could not locate the expected service on the GXOVnT device");

            // Map the incoming message characteristic to the outgoing of the device
            _incomingMessageCharacteristic =
                await connectedDeviceService.GetCharacteristicAsync(GXOVnTBluetoothOutgoingCharacteristic);
            
            if (_incomingMessageCharacteristic == null)
                throw new GXOVnTException("Could not locate the expected incoming message characteristic on the GXOVnT device");

            _incomingMessageCharacteristic.ValueUpdated += IncomingMessageCharacteristicOnValueUpdated;
            await _incomingMessageCharacteristic.StartUpdatesAsync();
            
            // Map the outgoing message characteristic to the incoming of the device
            _outgoingMessageCharacteristic =
                await connectedDeviceService.GetCharacteristicAsync(GXOVnTBluetoothIncomingCharacteristic);
            
            if (_outgoingMessageCharacteristic == null)
                throw new GXOVnTException("Could not locate the expected outgoing message characteristic on the GXOVnT device");
          
            
            deviceServicesBound = true;
        }
        catch (Exception ex)
        {
            _logService.LogError($"There was an error binding to the device with Id {Id}. {ex.Message}");
        }
        finally
        {
            // If there was an error during the binding, remove all the existing references and events
            if (!deviceServicesBound)
                UnBindFromDeviceServicesAndCharacteristics();

            DeviceServicesBound = deviceServicesBound;
            IsBusy = false;
        }
    }



    private void UnBindFromDeviceServicesAndCharacteristics()
    {
        try
        {
            IsBusy = true;
            
            _deviceServicesBound = false;
            _outgoingMessageCharacteristic = null;

            if (_incomingMessageCharacteristic != null)
                _incomingMessageCharacteristic.ValueUpdated -= IncomingMessageCharacteristicOnValueUpdated;

            _incomingMessageCharacteristic = null;
        }
        catch (Exception ex)
        {
            _logService.LogError($"There was an error unbinding from the device with Id {Id}. {ex.Message}");
        }
        finally
        {
            IsBusy = false;
            DeviceServicesBound = false;
        }
    }

    #endregion

    #region Event Callbacks

    private async void IncomingMessageCharacteristicOnValueUpdated(object? sender, CharacteristicUpdatedEventArgs e)
    {
        try
        {
   
            var (buffer, _) = await _incomingMessageCharacteristic!.ReadAsync();
            
            if (buffer != null && buffer.Length != 0)
                ProcessIncomingMessagePacket(buffer);
        }
        catch (Exception ex)
        {
            _logService.LogError($"There was an error reading the incoming message characteristic. {ex.Message}");
        }
    }

    private void ProcessIncomingMessagePacket(byte[] buffer)
    {
        try
        {
            _logService.LogDebug($"Message received from device. Inspecting packet");
        
            var commMessagePacket = new CommMessagePacket();
            
            commMessagePacket.DeSerializePacket(buffer);

            _logService.LogDebug($"Comm Message packet with Id {commMessagePacket.CommMessagePacketId} for message Id {commMessagePacket.CommMessageId}");
            _logService.LogDebug($"Packet start: {commMessagePacket.CommMessageDetail.HasFlag(CommMessageDetail.IsStartPacket)}, Packet end: {commMessagePacket.CommMessageDetail.HasFlag(CommMessageDetail.IsEndPacket)}");

            // Check if we already have 
            var commMessage = _partialCommMessages.Find(x => commMessagePacket.CommMessageId == x.MessageId);
            if (commMessage == null)
            {
                commMessage = new CommMessage(commMessagePacket);
                _partialCommMessages.Add(commMessage);
                return;
            }

            commMessage.AddCommMessagePacket(commMessagePacket);

            if (!commMessage.MessageComplete) 
                return;
            
            _logService.LogDebug($"Message is now complete, processing message");

         
            var messageAggregate = new MessageAggregate(commMessage);
            _historicCommMessages.Add(messageAggregate);
            _partialCommMessages.Remove(commMessage);

            LastReceivedMessage = messageAggregate;
            
            OnCommMessageReceived?.Invoke(this, messageAggregate);
        }
        catch (Exception ex)
        {
            _logService.LogError($"An unhandled error occured processing an incoming message packet, {ex.Message}");
        }
    }

    #endregion
    
    #region Dispose

    public async ValueTask DisposeAsync()
    {
        await DisconnectFromDeviceAsync();
    }
    #endregion

}