namespace GXOVnT.Shared.Common;

[Flags]
public enum CommMessageDetail : byte
{
    None = 0,
    IsStartPacket = 1,
    IsEndPacket = 2
}

public enum SystemBootMode
{
    SystemBleMode = 0,
    TestWiFiMode = 1,
    CheckFirmwareMode = 2
}

public enum JsonModelType 
{
    // Requests are from 1 to 100
    EchoRequest = 3,
    KeepAliveRequest = 4,
    GetSystemSettingsRequest = 5,
    SetSystemSettingsRequest = 6,
    TestWiFiSettingsRequest = 7,
    GetTestWiFiSettingsResultRequest = 8,
    CheckFirmwareUpdateRequest = 9,
    GetFirmwareUpdateResultRequest = 10,
    
    SetSystemBootModeRequest = 96,
    DeleteSystemSettingsRequest = 97,
    RebootRequest = 98,
    SaveConfigurationRequest = 99,

    // Responses are from 101 to 200
    StatusResponse = 101,
    GetSystemSettingsResponse = 102,
    GetTestWiFiSettingsResultResponse = 103,
    EchoResponse = 104,
    GetFirmwareUpdateResultResponse = 105,
    
    Unknown = 999
}