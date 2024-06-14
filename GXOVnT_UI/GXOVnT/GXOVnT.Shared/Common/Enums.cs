namespace GXOVnT.Shared.Common;

[Flags]
public enum CommMessageDetail : byte
{
    None = 0,
    IsStartPacket = 1,
    IsEndPacket = 2
}

public enum JsonModelType 
{
    // Requests are from 1 to 100
    EchoRequest = 3,
    KeepAlive = 4,
    RequestGetSystemSettings = 5,
    RequestSetSystemSettings = 6,
    RequestTestWiFiSettings = 7,
    RequestLastTestWiFiSettingsResult = 8,

    RequestReboot = 98,
    RequestSaveConfiguration = 99,

    // Responses are from 101 to 200
    StatusResponse = 101,
    ResponseSystemSettings = 102,
    ResponseLastTestWiFiSettingsResult = 103,
    
    Unknown = 999
}